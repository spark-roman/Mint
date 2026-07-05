using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Mint.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddInitTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ai_prompts",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 7, 5, 11, 33, 40, 914, DateTimeKind.Unspecified).AddTicks(2599), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.InsertData(
                table: "bonus_types",
                columns: new[] { "id", "code", "created_at", "description", "is_active", "name", "updated_at" },
                values: new object[,]
                {
                    { 1, "start", new DateTimeOffset(new DateTime(2026, 7, 5, 11, 33, 40, 914, DateTimeKind.Unspecified).AddTicks(2147), new TimeSpan(0, 0, 0, 0, 0)), "Бонус за первую регистрацию в боте", true, "Стартовый бонус", new DateTimeOffset(new DateTime(2026, 7, 5, 11, 33, 40, 914, DateTimeKind.Unspecified).AddTicks(2147), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 2, "daily", new DateTimeOffset(new DateTime(2026, 7, 5, 11, 33, 40, 914, DateTimeKind.Unspecified).AddTicks(2149), new TimeSpan(0, 0, 0, 0, 0)), "Бонус за ежедневный вход в бот", true, "Ежедневный бонус", new DateTimeOffset(new DateTime(2026, 7, 5, 11, 33, 40, 914, DateTimeKind.Unspecified).AddTicks(2150), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 3, "streak", new DateTimeOffset(new DateTime(2026, 7, 5, 11, 33, 40, 914, DateTimeKind.Unspecified).AddTicks(2151), new TimeSpan(0, 0, 0, 0, 0)), "Дополнительный бонус за непрерывный стрик 7+ дней", true, "Бонус за стрик", new DateTimeOffset(new DateTime(2026, 7, 5, 11, 33, 40, 914, DateTimeKind.Unspecified).AddTicks(2152), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 4, "referral", new DateTimeOffset(new DateTime(2026, 7, 5, 11, 33, 40, 914, DateTimeKind.Unspecified).AddTicks(2153), new TimeSpan(0, 0, 0, 0, 0)), "Бонус за приведённого друга", true, "Реферальный бонус", new DateTimeOffset(new DateTime(2026, 7, 5, 11, 33, 40, 914, DateTimeKind.Unspecified).AddTicks(2153), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 5, "rating", new DateTimeOffset(new DateTime(2026, 7, 5, 11, 33, 40, 914, DateTimeKind.Unspecified).AddTicks(2155), new TimeSpan(0, 0, 0, 0, 0)), "Бонус по итогам голосования", true, "Рейтинговый бонус", new DateTimeOffset(new DateTime(2026, 7, 5, 11, 33, 40, 914, DateTimeKind.Unspecified).AddTicks(2155), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 6, "admin", new DateTimeOffset(new DateTime(2026, 7, 5, 11, 33, 40, 914, DateTimeKind.Unspecified).AddTicks(2156), new TimeSpan(0, 0, 0, 0, 0)), "Ручное начисление администратором", true, "Административный бонус", new DateTimeOffset(new DateTime(2026, 7, 5, 11, 33, 40, 914, DateTimeKind.Unspecified).AddTicks(2157), new TimeSpan(0, 0, 0, 0, 0)) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 6);

            migrationBuilder.UpdateData(
                table: "ai_prompts",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 7, 4, 10, 24, 45, 830, DateTimeKind.Unspecified).AddTicks(9116), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
