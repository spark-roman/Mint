using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mint.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddStratCommandTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "bounus_type_id",
                table: "transactions");

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
                keyValue: 1L,
                column: "message",
                value: "🎉 Добро пожаловать в \"Дуэль мнений\"!\n\nВаш игровой профиль создан!\n💰 Стартовый баланс: {{balance}} 🪙\n🏆 Ранг: {{rank_emoji}} **{{rank_name}}**\n\nИспользуйте кнопки ниже для навигации.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "bounus_type_id",
                table: "transactions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "ai_prompts",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 7, 5, 11, 33, 40, 914, DateTimeKind.Unspecified).AddTicks(2599), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 5, 11, 33, 40, 914, DateTimeKind.Unspecified).AddTicks(2147), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 5, 11, 33, 40, 914, DateTimeKind.Unspecified).AddTicks(2147), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 5, 11, 33, 40, 914, DateTimeKind.Unspecified).AddTicks(2149), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 5, 11, 33, 40, 914, DateTimeKind.Unspecified).AddTicks(2150), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 5, 11, 33, 40, 914, DateTimeKind.Unspecified).AddTicks(2151), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 5, 11, 33, 40, 914, DateTimeKind.Unspecified).AddTicks(2152), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 5, 11, 33, 40, 914, DateTimeKind.Unspecified).AddTicks(2153), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 5, 11, 33, 40, 914, DateTimeKind.Unspecified).AddTicks(2153), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 5, 11, 33, 40, 914, DateTimeKind.Unspecified).AddTicks(2155), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 5, 11, 33, 40, 914, DateTimeKind.Unspecified).AddTicks(2155), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 5, 11, 33, 40, 914, DateTimeKind.Unspecified).AddTicks(2156), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 5, 11, 33, 40, 914, DateTimeKind.Unspecified).AddTicks(2157), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "steps",
                keyColumn: "id",
                keyValue: 1L,
                column: "message",
                value: "🎉 Добро пожаловать в Mint!\n\nВаш игровой профиль создан!\n💰 Стартовый баланс: 1000 🪙\n🏆 Ранг: 🌱 Новичок\n\nИспользуйте кнопки ниже для навигации.");
        }
    }
}
