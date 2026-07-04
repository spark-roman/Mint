# Этап сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем файлы проектов и solution
COPY Mint.App.Services/Mint.App.Services.csproj Mint.App.Services/
COPY Mint.Bot/Mint.Bot.csproj Mint.Bot/
COPY Mint.Common.Contracts/Mint.Common.Contracts.csproj Mint.Common.Contracts/
COPY Mint.UnitTests/Mint.UnitTests.csproj Mint.UnitTests/
COPY Mint.Test.Console/Mint.Test.Console.csproj Mint.Test.Console/
COPY Mint.Database/Mint.Database.csproj Mint.Database/
COPY Directory.Packages.props .
COPY Mint.sln .

# Восстанавливаем зависимости
RUN dotnet restore

# Копируем все исходники
COPY . .

# Публикуем только AdvApi (ваш основной проект)
RUN dotnet publish Mint.Bot/Mint.Bot.csproj -c Release -o /app/publish

# Финальный образ
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Настройка порта
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Запускаем Mint.Bot.dll
ENTRYPOINT ["dotnet", "Mint.Bot.dll"]
