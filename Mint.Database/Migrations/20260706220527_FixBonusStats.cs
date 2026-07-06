using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mint.Database.Migrations
{
    /// <inheritdoc />
    public partial class FixBonusStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "referral_earnings",
                table: "user_stats");

            migrationBuilder.RenameColumn(
                name: "last_rating_bonus_claimed_at",
                table: "user_bonus_stats",
                newName: "last_streak_claimed_at");

            migrationBuilder.AlterColumn<decimal>(
                name: "rank_points",
                table: "user_stats",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<decimal>(
                name: "total_referral_bonuses_claimed",
                table: "user_bonus_stats",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalStartBonusesClaimed",
                table: "user_bonus_stats",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "last_rank_bonus_claimed_at",
                table: "user_bonus_stats",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "total_daily_bonuses_claimed",
                table: "user_bonus_stats",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "total_rank_bonus_claimed",
                table: "user_bonus_stats",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "total_streak_bonuses_claimed",
                table: "user_bonus_stats",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "ai_prompts",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 7, 6, 22, 5, 26, 531, DateTimeKind.Unspecified).AddTicks(4577), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 6, 22, 5, 26, 531, DateTimeKind.Unspecified).AddTicks(4141), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 6, 22, 5, 26, 531, DateTimeKind.Unspecified).AddTicks(4141), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 6, 22, 5, 26, 531, DateTimeKind.Unspecified).AddTicks(4143), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 6, 22, 5, 26, 531, DateTimeKind.Unspecified).AddTicks(4143), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 6, 22, 5, 26, 531, DateTimeKind.Unspecified).AddTicks(4145), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 6, 22, 5, 26, 531, DateTimeKind.Unspecified).AddTicks(4145), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 6, 22, 5, 26, 531, DateTimeKind.Unspecified).AddTicks(4147), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 6, 22, 5, 26, 531, DateTimeKind.Unspecified).AddTicks(4147), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 6, 22, 5, 26, 531, DateTimeKind.Unspecified).AddTicks(4148), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 6, 22, 5, 26, 531, DateTimeKind.Unspecified).AddTicks(4149), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 6, 22, 5, 26, 531, DateTimeKind.Unspecified).AddTicks(4150), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 6, 22, 5, 26, 531, DateTimeKind.Unspecified).AddTicks(4150), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "steps",
                keyColumn: "id",
                keyValue: 2L,
                column: "message",
                value: "👤 **Ваш игровой профиль**\n━━━━━━━━━━━━━━━━━━━━━━━\n🏆 Ранг: {{rank_emoji}} **{{rank_name}}**\n👑 Очки за все время: {{rank_points}}\n💰 Текущие очки: {{balance}} 🪙\n\n📊 **Статистика прогнозов**\n├ Всего: {{total_duels}}\n├ ✅ Успешно: {{wins}}\n├ ❌ Неудачно: {{losses}}\n└ 🎯 Точность: {{winrate}}%\n\n👥 **Рефералы**\n├ Приглашено: {{referral_count}}\n└ Всего получено: {{total_referral_bonus}} 🪙\n\n🎁 **Ежедневный бонус**\n├ Статус: {{bonus_status}}\n├ Дней подряд: {{streak_days}} 🔥\n└ Всего получено: {{total_daily_bonus}} 🪙\n\n📅 В игре с: {{member_since}}");

            migrationBuilder.UpdateData(
                table: "steps",
                keyColumn: "id",
                keyValue: 4L,
                column: "message",
                value: "👥 **РЕФЕРАЛЬНАЯ ПРОГРАММА**\n\nПригласи друга и получи **500 монет**, когда он сделает свои первые 3 ставки в любых дуэлях!\n\n🎁 Твоя ссылка:\n`https://t.me/opinion_bot?start={{referral_code}}`\n\n👥 Приглашено друзей: {{referral_count}}\n💰 Всего бонусов: {{total_referral_bonus}} 🪙");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalStartBonusesClaimed",
                table: "user_bonus_stats");

            migrationBuilder.DropColumn(
                name: "last_rank_bonus_claimed_at",
                table: "user_bonus_stats");

            migrationBuilder.DropColumn(
                name: "total_daily_bonuses_claimed",
                table: "user_bonus_stats");

            migrationBuilder.DropColumn(
                name: "total_rank_bonus_claimed",
                table: "user_bonus_stats");

            migrationBuilder.DropColumn(
                name: "total_streak_bonuses_claimed",
                table: "user_bonus_stats");

            migrationBuilder.RenameColumn(
                name: "last_streak_claimed_at",
                table: "user_bonus_stats",
                newName: "last_rating_bonus_claimed_at");

            migrationBuilder.AlterColumn<int>(
                name: "rank_points",
                table: "user_stats",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<decimal>(
                name: "referral_earnings",
                table: "user_stats",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "total_referral_bonuses_claimed",
                table: "user_bonus_stats",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.UpdateData(
                table: "ai_prompts",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 7, 5, 21, 25, 16, 2, DateTimeKind.Unspecified).AddTicks(4007), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 5, 21, 25, 16, 2, DateTimeKind.Unspecified).AddTicks(3549), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 5, 21, 25, 16, 2, DateTimeKind.Unspecified).AddTicks(3550), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 5, 21, 25, 16, 2, DateTimeKind.Unspecified).AddTicks(3552), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 5, 21, 25, 16, 2, DateTimeKind.Unspecified).AddTicks(3552), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 5, 21, 25, 16, 2, DateTimeKind.Unspecified).AddTicks(3554), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 5, 21, 25, 16, 2, DateTimeKind.Unspecified).AddTicks(3554), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 5, 21, 25, 16, 2, DateTimeKind.Unspecified).AddTicks(3555), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 5, 21, 25, 16, 2, DateTimeKind.Unspecified).AddTicks(3555), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 5, 21, 25, 16, 2, DateTimeKind.Unspecified).AddTicks(3557), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 5, 21, 25, 16, 2, DateTimeKind.Unspecified).AddTicks(3557), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 5, 21, 25, 16, 2, DateTimeKind.Unspecified).AddTicks(3558), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 5, 21, 25, 16, 2, DateTimeKind.Unspecified).AddTicks(3559), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "steps",
                keyColumn: "id",
                keyValue: 2L,
                column: "message",
                value: "👤 **Ваш игровой профиль**\n━━━━━━━━━━━━━━━━━━━━━━━\n🆔 ID: `{{user_id}}`\n🏆 Ранг: {{rank_emoji}} **{{rank_name}}**\n💰 Баланс: {{balance}} 🪙\n\n📊 **Статистика прогнозов**\n├ Всего: {{total_duels}}\n├ ✅ Успешно: {{wins}}\n├ ❌ Неудачно: {{losses}}\n└ 🎯 Точность: {{winrate}}%\n\n👥 **Рефералы**\n├ Приглашено: {{referral_count}}\n└ Заработано: {{referral_earnings}} 🪙\n\n🎁 **Ежедневный бонус**\n├ Статус: {{bonus_status}}\n├ Дней подряд: {{streak_days}} 🔥\n└ Всего получено: {{total_bonus}} 🪙\n\n📅 В игре с: {{member_since}}");

            migrationBuilder.UpdateData(
                table: "steps",
                keyColumn: "id",
                keyValue: 4L,
                column: "message",
                value: "👥 **РЕФЕРАЛЬНАЯ ПРОГРАММА**\n\nПригласи друга и получи **500 монет**, когда он сделает свои первые 3 ставки в любых дуэлях!\n\n🎁 Твоя ссылка:\n`https://t.me/opinion_bot?start={{referral_code}}`\n\n👥 Приглашено друзей: {{referral_count}}\n💰 Заработано монет: {{referral_earnings}} 🪙");
        }
    }
}
