using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mint.Database.Migrations
{
    /// <inheritdoc />
    public partial class GnewsPrompts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ai_prompts",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 10, 21, 30, 1, 951, DateTimeKind.Unspecified).AddTicks(3625), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 10, 21, 30, 1, 951, DateTimeKind.Unspecified).AddTicks(3386), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 10, 21, 30, 1, 951, DateTimeKind.Unspecified).AddTicks(3386), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 10, 21, 30, 1, 951, DateTimeKind.Unspecified).AddTicks(3389), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 10, 21, 30, 1, 951, DateTimeKind.Unspecified).AddTicks(3389), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 10, 21, 30, 1, 951, DateTimeKind.Unspecified).AddTicks(3390), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 10, 21, 30, 1, 951, DateTimeKind.Unspecified).AddTicks(3391), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 10, 21, 30, 1, 951, DateTimeKind.Unspecified).AddTicks(3392), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 10, 21, 30, 1, 951, DateTimeKind.Unspecified).AddTicks(3392), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 10, 21, 30, 1, 951, DateTimeKind.Unspecified).AddTicks(3394), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 10, 21, 30, 1, 951, DateTimeKind.Unspecified).AddTicks(3394), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 10, 21, 30, 1, 951, DateTimeKind.Unspecified).AddTicks(3396), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 10, 21, 30, 1, 951, DateTimeKind.Unspecified).AddTicks(3396), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 7,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 10, 21, 30, 1, 951, DateTimeKind.Unspecified).AddTicks(3397), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 10, 21, 30, 1, 951, DateTimeKind.Unspecified).AddTicks(3398), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 9,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 10, 21, 30, 1, 951, DateTimeKind.Unspecified).AddTicks(3399), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 10, 21, 30, 1, 951, DateTimeKind.Unspecified).AddTicks(3399), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 100,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 10, 21, 30, 1, 951, DateTimeKind.Unspecified).AddTicks(3401), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 10, 21, 30, 1, 951, DateTimeKind.Unspecified).AddTicks(3401), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 1L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 10, 21, 30, 1, 951, DateTimeKind.Unspecified).AddTicks(4014), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 2L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 10, 21, 30, 1, 951, DateTimeKind.Unspecified).AddTicks(4017), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 3L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 10, 21, 30, 1, 951, DateTimeKind.Unspecified).AddTicks(4018), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 4L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 10, 21, 30, 1, 951, DateTimeKind.Unspecified).AddTicks(4019), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 5L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 10, 21, 30, 1, 951, DateTimeKind.Unspecified).AddTicks(4019), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 6L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 10, 21, 30, 1, 951, DateTimeKind.Unspecified).AddTicks(4020), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 7L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 10, 21, 30, 1, 951, DateTimeKind.Unspecified).AddTicks(4021), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 8L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 10, 21, 30, 1, 951, DateTimeKind.Unspecified).AddTicks(4022), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 9L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 10, 21, 30, 1, 951, DateTimeKind.Unspecified).AddTicks(4023), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 10L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 10, 21, 30, 1, 951, DateTimeKind.Unspecified).AddTicks(4023), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 11L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 10, 21, 30, 1, 951, DateTimeKind.Unspecified).AddTicks(4024), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 12L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 10, 21, 30, 1, 951, DateTimeKind.Unspecified).AddTicks(4025), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 1,
                column: "search_keywords",
                value: "(нейросеть OR нейросети OR ии OR искусственный интеллект OR chatgpt OR midjourney)");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 2,
                column: "search_keywords",
                value: "(смартфон OR процессор OR видеокарта OR iphone OR nvidia OR apple OR intel OR amd)");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 3,
                column: "search_keywords",
                value: "(крипта OR криптовалюта OR биткоин OR эфириум OR майнинг OR блокчейн OR nft OR токен)");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 4,
                column: "search_keywords",
                value: "(гейминг OR стрим OR стример OR twitch OR steam OR playstation OR xbox OR релиз игры)");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 5,
                column: "search_keywords",
                value: "(киберспорт OR турнир OR дота OR контр страйк OR мажор OR киберспортивный)");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 6,
                column: "search_keywords",
                value: "(кинопремьера OR фильм OR сериал OR дорама OR кинотеатр OR нетфликс OR режиссер)");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 7,
                column: "search_keywords",
                value: "(мем OR мемы OR прикол OR тренд OR блогер OR тикток OR телеграм OR вирусный OR соцсети)");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 8,
                column: "search_keywords",
                value: "(электрокар OR электромобиль OR тесла OR авторынок OR кроссовер OR zeekr OR гибрид)");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 9,
                column: "search_keywords",
                value: "(космос OR наса OR роскосмос OR телескоп OR ракета OR запуск OR астрономы OR квантовый)");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 10,
                column: "search_keywords",
                value: "(футбол OR рпл OR лига чемпионов OR трансфер OR спартак OR зенит OR гол OR матч)");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 11,
                column: "search_keywords",
                value: "(музыка OR певец OR трек OR альбом OR подиум OR одежда OR фэшн)");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 12,
                column: "search_keywords",
                value: "(финтех OR маркетплейс OR маркетплейсы OR озон OR вайлдберриз)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ai_prompts",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(5370), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(4944), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(4944), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(4947), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(4947), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(4949), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(4949), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(4951), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(4951), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(4953), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(4953), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(4954), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(4955), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 7,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(4956), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(4957), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 9,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(4959), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(4959), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 100,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(4961), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(4961), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 1L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(5894), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 2L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(5897), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 3L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(5898), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 4L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(5898), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 5L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(5899), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 6L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(5900), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 7L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(5901), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 8L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(5902), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 9L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(5902), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 10L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(5903), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 11L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(5904), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 12L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(5905), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 1,
                column: "search_keywords",
                value: "OpenAI ChatGPT Claude Gemini DeepSeek Grok Anthropic xAI LLM ИИ нейросети ИИ-агенты text-to-video Sora анонс обновление release upgrade leak controversy lawsuit ban safety regulation");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 2,
                column: "search_keywords",
                value: "Nvidia AMD Intel Apple Samsung Qualcomm Snapdragon PlayStation Xbox Nintendo консоли гаджеты процессор видеокарта чипсет смартфон specs leak benchmark deficit prices overheating");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 3,
                column: "search_keywords",
                value: "bitcoin btc ethereum eth solana binance tether usdt ton blockchain крипта криптовалюта Web3 DeFi hack scam regulations SEC санкции market crash dump pump взлом кража листинг");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 4,
                column: "search_keywords",
                value: "gaming news Steam PlayStation Xbox Nintendo Switch Twitch YouTube Kick Epic Games релиз патч streamer блогер banned drama controversy scandal бойкот взлом denuvo слив глитч баг");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 5,
                column: "search_keywords",
                value: "esports киберспорт Counter-Strike 2 cs2 dota 2 Valorant League of Legends Worlds PGL ESL Valve турнир матч финал мажор major drama disqualification cheat софт скандал замена трансфер бан");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 6,
                column: "search_keywords",
                value: "премьера сериал фильм блокбастер тизер трейлер каст актер режиссер Кинопоиск Нетфликс Netflix HBO Кинотеатры дубляж скандал критика разгром провал забастовка цензура спойлер бойкот");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 7,
                column: "search_keywords",
                value: "вирусится тренд мем флешмоб паблик каналы блогеры Телеграм Telegram ВКонтакте vk TikTok YouTube рунет Роскомнадзор РКН блокировка замедление сбой штраф запрет скандал хит");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 8,
                column: "search_keywords",
                value: "АвтоВАЗ Lada Zeekr LiXiang Xiaomi SU7 Тесла Tesla Москвич автомобили электромобили авторынок дилеры цены параллельный импорт утильсбор авария отзывы брак штрафы дефицит поломка");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 9,
                column: "search_keywords",
                value: "space NASA discovery astronomy breakthrough Марс Луна открытие телескоп Джеймс Уэбб SpaceX Роскосмос МКС физика биология археология сигнал аномалия вспышка угроза сенсация опровержение ученые");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 10,
                column: "search_keywords",
                value: "спорт футбол хоккей кхл нхл теннис мма ufc бокс фигурные катания рпл лига чемпионов матч судейство скандал разгром драка вар var отмена бойкот пенальти удаление допинг дисквалификация");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 11,
                column: "search_keywords",
                value: "pop music fashion week тренд альбом клип показ моды селебрити шоу-бизнес инфлюенсеры Billboard Грэмми Неделя моды celebrity scandal плагиат бойкот отмена критика шок");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 12,
                column: "search_keywords",
                value: "wildberries wb ozon мегамаркет яндекс маркет сбер т-банк альфа втб цб рф маркетплейсы финтех банки цифровой рубль онлайн-банкинг штраф скандал комиссия блокировка забастовка сбой мошенники");
        }
    }
}
