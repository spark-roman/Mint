using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mint.App.Services.System.DuelsGeneration.Dto;
using Mint.App.Services.System.DuelsGeneration.GigaChat.Dto;

namespace Mint.App.Services.System.DuelsGeneration.GigaChat;

/// <inheritdoc />
public class GigaChatService(
    IOptions<GigaChatSettings> settings,
    HttpClient httpClient,
    ILogger<GigaChatService> logger) : IGigaChatService, IDisposable
{
    private readonly GigaChatSettings _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
    private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    private readonly ILogger<GigaChatService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private string? _cachedAccessToken;
    private DateTimeOffset _tokenExpiresAt;

    /// <inheritdoc />
    public async Task<List<DuelGenerationDto>> GenerateDuelsAsync(string userPrompt, int count, CancellationToken cancellationToken)
    {
        var accessToken = await GetAccessTokenAsync(cancellationToken);

        var requestJson = BuildChatRequest(userPrompt, count);
        using var content = new StringContent(requestJson, Encoding.UTF8, MediaTypeNames.Application.Json);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, new Uri($"{_settings.BaseUrl}/chat/completions"))
        {
            Content = content
        };
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("GigaChat API error: {StatusCode} - {Error}", response.StatusCode, errorBody);
            throw new InvalidOperationException($"GigaChat API error: {response.StatusCode} - {errorBody}");
        }

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        var apiResponse = JsonSerializer.Deserialize<DeepSeekResponse>(responseJson);

        if (apiResponse?.Choices is null or { Count: 0 })
        {
            throw new InvalidOperationException("No choices returned from GigaChat");
        }

        var contentText = apiResponse.Choices.First().Message.Content;

        var duels = JsonSerializer.Deserialize<List<DuelGenerationDto>>(contentText);

        return duels ?? [];
    }

    private string BuildChatRequest(string userPrompt, int count)
    {
        var root = new JsonObject
        {
            ["model"] = _settings.Model,
            ["messages"] = new JsonArray(
                new JsonObject
                {
                    ["role"] = "user",
                    ["content"] = userPrompt
                }),
            ["temperature"] = 0.7,
            ["max_tokens"] = 4096
        };

        var requestJson = root.ToJsonString(new JsonSerializerOptions { WriteIndented = false });

        _logger.LogDebug("GigaChat request JSON:\n{RequestJson}", requestJson);

        return requestJson;
    }

    private async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(_cachedAccessToken) && DateTimeOffset.UtcNow < _tokenExpiresAt.AddMinutes(-5))
        {
            return _cachedAccessToken;
        }

        using var tokenRequest = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("scope", _settings.Scope)
        });

        var authBytes = Encoding.UTF8.GetBytes($"{_settings.ClientId}:{_settings.ClientSecret}");
        var authHeader = Convert.ToBase64String(authBytes);

        var rqUid = Guid.NewGuid().ToString();

        using var tokenHttpRequest = new HttpRequestMessage(HttpMethod.Post, _settings.AuthUrl)
        {
            Content = tokenRequest
        };
        tokenHttpRequest.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
        tokenHttpRequest.Headers.Add("RqUID", rqUid);

        using var tokenResponse = await _httpClient.SendAsync(tokenHttpRequest, cancellationToken);

        if (!tokenResponse.IsSuccessStatusCode)
        {
            var errorBody = await tokenResponse.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"GigaChat OAuth error: {tokenResponse.StatusCode} - {errorBody}");
        }

        var tokenJson = await tokenResponse.Content.ReadAsStringAsync(cancellationToken);
        var token = JsonSerializer.Deserialize<GigaChatTokenResponse>(tokenJson);

        if (token is null)
        {
            throw new InvalidOperationException("Failed to deserialize GigaChat token");
        }

        _cachedAccessToken = token.AccessToken;
        _tokenExpiresAt = DateTimeOffset.UtcNow.AddSeconds(token.ExpiresIn);

        return _cachedAccessToken;
    }

    private bool _disposed;

    /// <inheritdoc />
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _httpClient.Dispose();
        }

        _disposed = true;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
