using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Mint.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddSystemSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "system_settings",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    value = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_system_settings", x => x.id);
                });

            migrationBuilder.UpdateData(
                table: "ai_prompts",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 8, 10, 19, 10, 45, 837, DateTimeKind.Unspecified).AddTicks(1858), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 10, 19, 10, 45, 837, DateTimeKind.Unspecified).AddTicks(1660), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 10, 19, 10, 45, 837, DateTimeKind.Unspecified).AddTicks(1660), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 10, 19, 10, 45, 837, DateTimeKind.Unspecified).AddTicks(1662), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 10, 19, 10, 45, 837, DateTimeKind.Unspecified).AddTicks(1662), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 10, 19, 10, 45, 837, DateTimeKind.Unspecified).AddTicks(1664), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 10, 19, 10, 45, 837, DateTimeKind.Unspecified).AddTicks(1664), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 10, 19, 10, 45, 837, DateTimeKind.Unspecified).AddTicks(1665), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 10, 19, 10, 45, 837, DateTimeKind.Unspecified).AddTicks(1666), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 10, 19, 10, 45, 837, DateTimeKind.Unspecified).AddTicks(1668), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 10, 19, 10, 45, 837, DateTimeKind.Unspecified).AddTicks(1668), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 10, 19, 10, 45, 837, DateTimeKind.Unspecified).AddTicks(1670), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 10, 19, 10, 45, 837, DateTimeKind.Unspecified).AddTicks(1670), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.InsertData(
                table: "system_settings",
                columns: new[] { "id", "description", "key", "updated_at", "value" },
                values: new object[,]
                {
                    { 1L, "Стартовый бонус новому пользователю", "StartBonus", new DateTimeOffset(new DateTime(2026, 8, 10, 19, 10, 45, 837, DateTimeKind.Unspecified).AddTicks(2277), new TimeSpan(0, 0, 0, 0, 0)), "5000" },
                    { 2L, "Ежедневный бонус", "DailyBonus", new DateTimeOffset(new DateTime(2026, 8, 10, 19, 10, 45, 837, DateTimeKind.Unspecified).AddTicks(2285), new TimeSpan(0, 0, 0, 0, 0)), "1000" },
                    { 3L, "Бонус за стрик 7 дней", "StreakBonus", new DateTimeOffset(new DateTime(2026, 8, 10, 19, 10, 45, 837, DateTimeKind.Unspecified).AddTicks(2286), new TimeSpan(0, 0, 0, 0, 0)), "10000" },
                    { 4L, "Комиссия при расчете выплаты", "HouseCommission", new DateTimeOffset(new DateTime(2026, 8, 10, 19, 10, 45, 837, DateTimeKind.Unspecified).AddTicks(2286), new TimeSpan(0, 0, 0, 0, 0)), "0.05" },
                    { 5L, "Время жизни дуэли в часах", "DuelExpirationHours", new DateTimeOffset(new DateTime(2026, 8, 10, 19, 10, 45, 837, DateTimeKind.Unspecified).AddTicks(2287), new TimeSpan(0, 0, 0, 0, 0)), "24" },
                    { 6L, "Количество игроков в таблице лидеров", "LeaderboardSize", new DateTimeOffset(new DateTime(2026, 8, 10, 19, 10, 45, 837, DateTimeKind.Unspecified).AddTicks(2288), new TimeSpan(0, 0, 0, 0, 0)), "15" },
                    { 7L, "Максимальный процент от баланса для ставки", "MaxBetPercent", new DateTimeOffset(new DateTime(2026, 8, 10, 19, 10, 45, 837, DateTimeKind.Unspecified).AddTicks(2289), new TimeSpan(0, 0, 0, 0, 0)), "100" },
                    { 8L, "Бонус за приглашенного друга", "ReferralBonus", new DateTimeOffset(new DateTime(2026, 8, 10, 19, 10, 45, 837, DateTimeKind.Unspecified).AddTicks(2289), new TimeSpan(0, 0, 0, 0, 0)), "5000" },
                    { 9L, "Минимальная сумма ставки", "MinBetAmount", new DateTimeOffset(new DateTime(2026, 8, 10, 19, 10, 45, 837, DateTimeKind.Unspecified).AddTicks(2292), new TimeSpan(0, 0, 0, 0, 0)), "10" },
                    { 10L, "Максимальная сумма ставки", "MaxBetAmount", new DateTimeOffset(new DateTime(2026, 8, 10, 19, 10, 45, 837, DateTimeKind.Unspecified).AddTicks(2293), new TimeSpan(0, 0, 0, 0, 0)), "10000" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "system_settings");

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
        }
    }
}
