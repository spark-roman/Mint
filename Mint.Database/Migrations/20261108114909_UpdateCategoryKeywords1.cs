using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mint.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCategoryKeywords1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ai_prompts",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 453, DateTimeKind.Unspecified).AddTicks(9679), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 453, DateTimeKind.Unspecified).AddTicks(9434), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 453, DateTimeKind.Unspecified).AddTicks(9434), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 453, DateTimeKind.Unspecified).AddTicks(9437), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 453, DateTimeKind.Unspecified).AddTicks(9438), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 453, DateTimeKind.Unspecified).AddTicks(9439), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 453, DateTimeKind.Unspecified).AddTicks(9440), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 453, DateTimeKind.Unspecified).AddTicks(9441), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 453, DateTimeKind.Unspecified).AddTicks(9441), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 453, DateTimeKind.Unspecified).AddTicks(9443), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 453, DateTimeKind.Unspecified).AddTicks(9443), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 453, DateTimeKind.Unspecified).AddTicks(9445), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 453, DateTimeKind.Unspecified).AddTicks(9445), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 7,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 453, DateTimeKind.Unspecified).AddTicks(9446), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 453, DateTimeKind.Unspecified).AddTicks(9447), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 9,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 453, DateTimeKind.Unspecified).AddTicks(9448), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 453, DateTimeKind.Unspecified).AddTicks(9448), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 100,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 453, DateTimeKind.Unspecified).AddTicks(9450), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 453, DateTimeKind.Unspecified).AddTicks(9450), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 1L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 454, DateTimeKind.Unspecified).AddTicks(69), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 2L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 454, DateTimeKind.Unspecified).AddTicks(70), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 3L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 454, DateTimeKind.Unspecified).AddTicks(71), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 4L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 454, DateTimeKind.Unspecified).AddTicks(72), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 5L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 454, DateTimeKind.Unspecified).AddTicks(73), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 6L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 454, DateTimeKind.Unspecified).AddTicks(73), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 7L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 454, DateTimeKind.Unspecified).AddTicks(74), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 8L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 454, DateTimeKind.Unspecified).AddTicks(75), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 9L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 454, DateTimeKind.Unspecified).AddTicks(76), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 10L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 454, DateTimeKind.Unspecified).AddTicks(77), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 11L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 454, DateTimeKind.Unspecified).AddTicks(77), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 12L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 454, DateTimeKind.Unspecified).AddTicks(78), new TimeSpan(0, 0, 0, 0, 0)));

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
