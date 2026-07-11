using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mint.Database.Migrations
{
    /// <inheritdoc />
    public partial class LeaderBoardStepAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ai_prompts",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 7, 11, 13, 48, 24, 858, DateTimeKind.Unspecified).AddTicks(8880), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 11, 13, 48, 24, 858, DateTimeKind.Unspecified).AddTicks(8652), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 11, 13, 48, 24, 858, DateTimeKind.Unspecified).AddTicks(8653), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 11, 13, 48, 24, 858, DateTimeKind.Unspecified).AddTicks(8655), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 11, 13, 48, 24, 858, DateTimeKind.Unspecified).AddTicks(8655), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 11, 13, 48, 24, 858, DateTimeKind.Unspecified).AddTicks(8657), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 11, 13, 48, 24, 858, DateTimeKind.Unspecified).AddTicks(8657), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 11, 13, 48, 24, 858, DateTimeKind.Unspecified).AddTicks(8658), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 11, 13, 48, 24, 858, DateTimeKind.Unspecified).AddTicks(8659), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 11, 13, 48, 24, 858, DateTimeKind.Unspecified).AddTicks(8660), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 11, 13, 48, 24, 858, DateTimeKind.Unspecified).AddTicks(8660), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 11, 13, 48, 24, 858, DateTimeKind.Unspecified).AddTicks(8662), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 11, 13, 48, 24, 858, DateTimeKind.Unspecified).AddTicks(8662), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.InsertData(
                table: "scenarios",
                columns: new[] { "id", "created_at", "is_active", "name" },
                values: new object[] { 5L, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, "leaderboard" });

            migrationBuilder.UpdateData(
                table: "steps",
                keyColumn: "id",
                keyValue: 2L,
                column: "message",
                value: "👤 **Ваш игровой профиль**\n━━━━━━━━━━━━━━━━━━━━━━━\n🏆 Ранг: {{rank_emoji}} **{{rank_name}}**\n👑 Очки по дуэлям: {{rank_points}}\n💰 Текущие очки: {{balance}} 🪙\n\n📊 **Статистика прогнозов**\n├ Всего: {{total_duels}}\n├ Успешно: {{wins}}\n├ Неудачно: {{losses}}\n└ Точность: {{winrate}}%\n\n👥 **Рефералы**\n├ Приглашено: {{referral_count}}\n└ Всего получено: {{total_referral_bonus}} 🪙\n\n🎁 **Ежедневный бонус**\n├ Статус: {{bonus_status}}\n├ Дней подряд: {{streak_days}} 🔥\n└ Всего получено: {{total_daily_bonus}} 🪙\n\n📅 В игре с: {{member_since}}");

            migrationBuilder.InsertData(
                table: "steps",
                columns: new[] { "id", "data", "is_final", "message", "order_num", "scenario_id", "step_type_id" },
                values: new object[] { 6L, null, false, "🏆 **ТАБЛИЦА ЛИДЕРОВ**\n\nРейтинг строится на основе **Очков Ранга**.\n\n{{leaderboard_entries}}\n\n───────────────────────\n{{user_rank_info}}", (short)2, 2L, (short)4 });

            migrationBuilder.InsertData(
                table: "buttons",
                columns: new[] { "id", "action", "caption", "next_step_id", "order_num", "parent_step_id" },
                values: new object[] { 8L, "profile", "🔙 Вернуться в профиль", null, (short)1, 6L });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "buttons",
                keyColumn: "id",
                keyValue: 8L);

            migrationBuilder.DeleteData(
                table: "scenarios",
                keyColumn: "id",
                keyValue: 5L);

            migrationBuilder.DeleteData(
                table: "steps",
                keyColumn: "id",
                keyValue: 6L);

            migrationBuilder.UpdateData(
                table: "ai_prompts",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 7, 6, 23, 8, 34, 523, DateTimeKind.Unspecified).AddTicks(3412), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 6, 23, 8, 34, 523, DateTimeKind.Unspecified).AddTicks(3188), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 6, 23, 8, 34, 523, DateTimeKind.Unspecified).AddTicks(3189), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 6, 23, 8, 34, 523, DateTimeKind.Unspecified).AddTicks(3191), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 6, 23, 8, 34, 523, DateTimeKind.Unspecified).AddTicks(3192), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 6, 23, 8, 34, 523, DateTimeKind.Unspecified).AddTicks(3193), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 6, 23, 8, 34, 523, DateTimeKind.Unspecified).AddTicks(3194), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 6, 23, 8, 34, 523, DateTimeKind.Unspecified).AddTicks(3195), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 6, 23, 8, 34, 523, DateTimeKind.Unspecified).AddTicks(3196), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 6, 23, 8, 34, 523, DateTimeKind.Unspecified).AddTicks(3197), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 6, 23, 8, 34, 523, DateTimeKind.Unspecified).AddTicks(3197), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 6, 23, 8, 34, 523, DateTimeKind.Unspecified).AddTicks(3199), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 6, 23, 8, 34, 523, DateTimeKind.Unspecified).AddTicks(3199), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "steps",
                keyColumn: "id",
                keyValue: 2L,
                column: "message",
                value: "👤 **Ваш игровой профиль**\n━━━━━━━━━━━━━━━━━━━━━━━\n🏆 Ранг: {{rank_emoji}} **{{rank_name}}**\n👑 Очки по дуэлям: {{rank_points}}\n💰 Текущие очки: {{balance}} 🪙\n\n📊 **Статистика прогнозов**\n├ Всего: {{total_duels}}\n├ ✅ Успешно: {{wins}}\n├ ❌ Неудачно: {{losses}}\n└ 🎯 Точность: {{winrate}}%\n\n👥 **Рефералы**\n├ Приглашено: {{referral_count}}\n└ Всего получено: {{total_referral_bonus}} 🪙\n\n🎁 **Ежедневный бонус**\n├ Статус: {{bonus_status}}\n├ Дней подряд: {{streak_days}} 🔥\n└ Всего получено: {{total_daily_bonus}} 🪙\n\n📅 В игре с: {{member_since}}");
        }
    }
}
