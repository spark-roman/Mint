using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mint.Database.Migrations
{
    /// <inheritdoc />
    public partial class FixBalanceOnStartPage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ai_prompts",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 8, 7, 18, 57, 13, 664, DateTimeKind.Unspecified).AddTicks(8029), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 7, 18, 57, 13, 664, DateTimeKind.Unspecified).AddTicks(7747), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 7, 18, 57, 13, 664, DateTimeKind.Unspecified).AddTicks(7747), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 7, 18, 57, 13, 664, DateTimeKind.Unspecified).AddTicks(7749), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 7, 18, 57, 13, 664, DateTimeKind.Unspecified).AddTicks(7750), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 7, 18, 57, 13, 664, DateTimeKind.Unspecified).AddTicks(7751), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 7, 18, 57, 13, 664, DateTimeKind.Unspecified).AddTicks(7751), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 7, 18, 57, 13, 664, DateTimeKind.Unspecified).AddTicks(7753), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 7, 18, 57, 13, 664, DateTimeKind.Unspecified).AddTicks(7753), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 7, 18, 57, 13, 664, DateTimeKind.Unspecified).AddTicks(7755), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 7, 18, 57, 13, 664, DateTimeKind.Unspecified).AddTicks(7755), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 7, 18, 57, 13, 664, DateTimeKind.Unspecified).AddTicks(7756), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 7, 18, 57, 13, 664, DateTimeKind.Unspecified).AddTicks(7757), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "steps",
                keyColumn: "id",
                keyValue: 1L,
                column: "message",
                value: "🎉 Добро пожаловать в \"Дуэль мнений\"!\n\nВаш игровой профиль создан!\n💰 Баланс: {{balance}} 🪙\n🏆 Ранг: {{rank_emoji}} **{{rank_name}}**\n\nИспользуйте кнопки ниже для навигации.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ai_prompts",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 8, 2, 19, 13, 58, 722, DateTimeKind.Unspecified).AddTicks(5331), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 2, 19, 13, 58, 722, DateTimeKind.Unspecified).AddTicks(5068), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 2, 19, 13, 58, 722, DateTimeKind.Unspecified).AddTicks(5068), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 2, 19, 13, 58, 722, DateTimeKind.Unspecified).AddTicks(5070), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 2, 19, 13, 58, 722, DateTimeKind.Unspecified).AddTicks(5071), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 2, 19, 13, 58, 722, DateTimeKind.Unspecified).AddTicks(5072), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 2, 19, 13, 58, 722, DateTimeKind.Unspecified).AddTicks(5073), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 2, 19, 13, 58, 722, DateTimeKind.Unspecified).AddTicks(5074), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 2, 19, 13, 58, 722, DateTimeKind.Unspecified).AddTicks(5074), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 2, 19, 13, 58, 722, DateTimeKind.Unspecified).AddTicks(5076), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 2, 19, 13, 58, 722, DateTimeKind.Unspecified).AddTicks(5076), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 2, 19, 13, 58, 722, DateTimeKind.Unspecified).AddTicks(5077), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 2, 19, 13, 58, 722, DateTimeKind.Unspecified).AddTicks(5078), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "steps",
                keyColumn: "id",
                keyValue: 1L,
                column: "message",
                value: "🎉 Добро пожаловать в \"Дуэль мнений\"!\n\nВаш игровой профиль создан!\n💰 Стартовый баланс: {{balance}} 🪙\n🏆 Ранг: {{rank_emoji}} **{{rank_name}}**\n\nИспользуйте кнопки ниже для навигации.");
        }
    }
}
