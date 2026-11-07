using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mint.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCategoryKeywords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ai_prompts",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 7, 18, 2, 7, 582, DateTimeKind.Unspecified).AddTicks(6747), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 7, 18, 2, 7, 582, DateTimeKind.Unspecified).AddTicks(6486), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 7, 18, 2, 7, 582, DateTimeKind.Unspecified).AddTicks(6486), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 7, 18, 2, 7, 582, DateTimeKind.Unspecified).AddTicks(6495), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 7, 18, 2, 7, 582, DateTimeKind.Unspecified).AddTicks(6496), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 7, 18, 2, 7, 582, DateTimeKind.Unspecified).AddTicks(6498), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 7, 18, 2, 7, 582, DateTimeKind.Unspecified).AddTicks(6498), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 7, 18, 2, 7, 582, DateTimeKind.Unspecified).AddTicks(6499), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 7, 18, 2, 7, 582, DateTimeKind.Unspecified).AddTicks(6500), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 7, 18, 2, 7, 582, DateTimeKind.Unspecified).AddTicks(6501), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 7, 18, 2, 7, 582, DateTimeKind.Unspecified).AddTicks(6501), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 7, 18, 2, 7, 582, DateTimeKind.Unspecified).AddTicks(6503), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 7, 18, 2, 7, 582, DateTimeKind.Unspecified).AddTicks(6503), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 7,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 7, 18, 2, 7, 582, DateTimeKind.Unspecified).AddTicks(6505), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 7, 18, 2, 7, 582, DateTimeKind.Unspecified).AddTicks(6505), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 9,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 7, 18, 2, 7, 582, DateTimeKind.Unspecified).AddTicks(6506), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 7, 18, 2, 7, 582, DateTimeKind.Unspecified).AddTicks(6507), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 100,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 7, 18, 2, 7, 582, DateTimeKind.Unspecified).AddTicks(6508), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 7, 18, 2, 7, 582, DateTimeKind.Unspecified).AddTicks(6509), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 1L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 7, 18, 2, 7, 582, DateTimeKind.Unspecified).AddTicks(7533), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 2L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 7, 18, 2, 7, 582, DateTimeKind.Unspecified).AddTicks(7535), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 3L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 7, 18, 2, 7, 582, DateTimeKind.Unspecified).AddTicks(7535), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 4L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 7, 18, 2, 7, 582, DateTimeKind.Unspecified).AddTicks(7536), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 5L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 7, 18, 2, 7, 582, DateTimeKind.Unspecified).AddTicks(7537), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 6L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 7, 18, 2, 7, 582, DateTimeKind.Unspecified).AddTicks(7538), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 7L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 7, 18, 2, 7, 582, DateTimeKind.Unspecified).AddTicks(7539), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 8L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 7, 18, 2, 7, 582, DateTimeKind.Unspecified).AddTicks(7539), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 9L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 7, 18, 2, 7, 582, DateTimeKind.Unspecified).AddTicks(7540), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 10L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 7, 18, 2, 7, 582, DateTimeKind.Unspecified).AddTicks(7541), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 11L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 7, 18, 2, 7, 582, DateTimeKind.Unspecified).AddTicks(7542), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 12L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 7, 18, 2, 7, 582, DateTimeKind.Unspecified).AddTicks(7542), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 1,
                column: "search_keywords",
                value: "OpenAI ChatGPT Claude Gemini xAI ИИ нейросети release upgrade leak controversy lawsuit ban safety");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 2,
                column: "search_keywords",
                value: "Nvidia AMD Intel Apple Samsung hardware specs smartphone gpu release leak benchmark deficit prices");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 3,
                column: "search_keywords",
                value: "bitcoin crypto ethereum hack binance sec scam regulations market crash dump взлом");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 4,
                column: "search_keywords",
                value: "gaming news Twitch streamer banned drama controversy game release patch Steam взлом слив");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 5,
                column: "search_keywords",
                value: "esports cs2 dota2 tournament match drama disqualification Valve cheat софт скандал");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 6,
                column: "search_keywords",
                value: "премьера кинотеатры сериал кинопоиск трейлер нетфликс скандал критика провал режиссер актеры");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 7,
                column: "search_keywords",
                value: "вирусится тренд мем ркн блокировка телеграм вконтакте тикток сбой штраф");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 8,
                column: "search_keywords",
                value: "автоваз lada автомобили параллельный импорт цены zeekr lixiang электромобили авария утильсбор");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 9,
                column: "search_keywords",
                value: "space NASA discovery astronomy breakthrough SpaceX Марс ученые открытие аномалия сигнал");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 10,
                column: "search_keywords",
                value: "рпл футбол судейство скандал матч рфс лига чемпионов гол разгром трансфер drama var");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 11,
                column: "search_keywords",
                value: "pop music billboard fashion week celebrity scandal Грэмми показ моды тренд плагиат");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 12,
                column: "search_keywords",
                value: "wildberries ozon сбер вб штраф скандал комиссия финтех банки цб рф цифровой рубль сбой");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ai_prompts",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 7, 13, 39, 32, 205, DateTimeKind.Unspecified).AddTicks(5462), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 7, 13, 39, 32, 205, DateTimeKind.Unspecified).AddTicks(5246), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 7, 13, 39, 32, 205, DateTimeKind.Unspecified).AddTicks(5246), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 7, 13, 39, 32, 205, DateTimeKind.Unspecified).AddTicks(5248), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 7, 13, 39, 32, 205, DateTimeKind.Unspecified).AddTicks(5248), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 7, 13, 39, 32, 205, DateTimeKind.Unspecified).AddTicks(5250), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 7, 13, 39, 32, 205, DateTimeKind.Unspecified).AddTicks(5250), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 7, 13, 39, 32, 205, DateTimeKind.Unspecified).AddTicks(5251), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 7, 13, 39, 32, 205, DateTimeKind.Unspecified).AddTicks(5252), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 7, 13, 39, 32, 205, DateTimeKind.Unspecified).AddTicks(5253), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 7, 13, 39, 32, 205, DateTimeKind.Unspecified).AddTicks(5253), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 7, 13, 39, 32, 205, DateTimeKind.Unspecified).AddTicks(5255), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 7, 13, 39, 32, 205, DateTimeKind.Unspecified).AddTicks(5255), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 7,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 7, 13, 39, 32, 205, DateTimeKind.Unspecified).AddTicks(5257), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 7, 13, 39, 32, 205, DateTimeKind.Unspecified).AddTicks(5257), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 9,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 7, 13, 39, 32, 205, DateTimeKind.Unspecified).AddTicks(5258), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 7, 13, 39, 32, 205, DateTimeKind.Unspecified).AddTicks(5259), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 100,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 7, 13, 39, 32, 205, DateTimeKind.Unspecified).AddTicks(5260), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 7, 13, 39, 32, 205, DateTimeKind.Unspecified).AddTicks(5260), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 1L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 7, 13, 39, 32, 205, DateTimeKind.Unspecified).AddTicks(5843), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 2L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 7, 13, 39, 32, 205, DateTimeKind.Unspecified).AddTicks(5844), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 3L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 7, 13, 39, 32, 205, DateTimeKind.Unspecified).AddTicks(5845), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 4L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 7, 13, 39, 32, 205, DateTimeKind.Unspecified).AddTicks(5846), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 5L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 7, 13, 39, 32, 205, DateTimeKind.Unspecified).AddTicks(5847), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 6L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 7, 13, 39, 32, 205, DateTimeKind.Unspecified).AddTicks(5848), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 7L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 7, 13, 39, 32, 205, DateTimeKind.Unspecified).AddTicks(5849), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 8L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 7, 13, 39, 32, 205, DateTimeKind.Unspecified).AddTicks(5850), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 9L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 7, 13, 39, 32, 205, DateTimeKind.Unspecified).AddTicks(5850), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 10L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 7, 13, 39, 32, 205, DateTimeKind.Unspecified).AddTicks(5851), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 11L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 7, 13, 39, 32, 205, DateTimeKind.Unspecified).AddTicks(5852), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 12L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 7, 13, 39, 32, 205, DateTimeKind.Unspecified).AddTicks(5853), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 1,
                column: "search_keywords",
                value: "DeepSeek, ChatGPT, Midjourney, Claude, Sora AI, генерация видео, ИИ-ассистенты, LLM модели, ИИ в программировании, плагины, автоматизация рутины");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 2,
                column: "search_keywords",
                value: "Apple iPhone, Samsung Galaxy, процессоры Nvidia, видеокарты AMD, умные часы, VR/AR гарнитуры, портативные консоли, роутеры, утечки дизайнов, краш-тесты");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 3,
                column: "search_keywords",
                value: "Курс Bitcoin, экосистема TON, Telegram Wallet, Ethereum, новые аирдропы, мемкоины, стейкинг, криптобиржи, DeFi, аппаратные кошельки, майнинг в РФ");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 4,
                column: "search_keywords",
                value: "Steam тренды, PlayStation 5, Xbox, патчи и обновления, GTA 6 слухи, Epic Games Store, популярные стримеры, Twitch, новые трейлеры, инди-игры");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 5,
                column: "search_keywords",
                value: "The International, PGL Major, Dota 2 трансферы, Counter-Strike 2 матчи, призовые фонды, киберспортивные команды, аналитика матчей, Valorant, League of Legends");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 6,
                column: "search_keywords",
                value: "Премьеры кино, новые сериалы, Кинопоиск рейтинг, трейлеры Netflix, HBO, кассовые сборы, актеры, теории фанатов, сиквелы, аниме новинки");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 7,
                column: "search_keywords",
                value: "Вирусные видео, тренды YouTube, новые шоу, VK Видео, популярные блогеры рунета, интернет-мемы, челленджи, Telegram-тренды, обсуждения в сообществах");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 8,
                column: "search_keywords",
                value: "Электромобили, Zeekr, Xiaomi SU7, Tesla новости, китайские автобренды, спорткары, концепт-кары, автопилот, тюнинг, гибридные двигатели, тест-драйвы");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 9,
                column: "search_keywords",
                value: "Запуски SpaceX, телескоп Джеймс Уэбб, Марсианские миссии, квантовые компьютеры, археологические находки, генная инженерия, термоядерный синтез, физика");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 10,
                column: "search_keywords",
                value: "Лига Чемпионов, результаты матчей, футбольные трансферы, UFC бои, Формула-1, НХЛ, НБА, громкие рекорды, спортивные скандалы, медиафутбол");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 11,
                column: "search_keywords",
                value: "Дропы кроссовок, коллаборации брендов, музыкальные чарты, новые клипы, тренды одежды, рэп-релизы, поп-звезды, музыкальные стриминги, стритвир");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 12,
                column: "search_keywords",
                value: "Обновления Т-Банка, фичи Сбера, тренды Wildberries, Ozon, Мегамаркет, акции ИТ-компаний, новые стартапы, краудфандинг, сервисы доставки, СБП");
        }
    }
}
