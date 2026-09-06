using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mint.Database.Migrations
{
    /// <inheritdoc />
    public partial class FixMoneyFormat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "rank_points",
                table: "user_stats",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.UpdateData(
                table: "ai_prompts",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 9, 6, 11, 13, 7, 548, DateTimeKind.Unspecified).AddTicks(5894), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 9, 6, 11, 13, 7, 548, DateTimeKind.Unspecified).AddTicks(5688), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 9, 6, 11, 13, 7, 548, DateTimeKind.Unspecified).AddTicks(5689), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 9, 6, 11, 13, 7, 548, DateTimeKind.Unspecified).AddTicks(5690), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 9, 6, 11, 13, 7, 548, DateTimeKind.Unspecified).AddTicks(5691), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 9, 6, 11, 13, 7, 548, DateTimeKind.Unspecified).AddTicks(5692), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 9, 6, 11, 13, 7, 548, DateTimeKind.Unspecified).AddTicks(5693), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 9, 6, 11, 13, 7, 548, DateTimeKind.Unspecified).AddTicks(5694), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 9, 6, 11, 13, 7, 548, DateTimeKind.Unspecified).AddTicks(5694), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 9, 6, 11, 13, 7, 548, DateTimeKind.Unspecified).AddTicks(5696), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 9, 6, 11, 13, 7, 548, DateTimeKind.Unspecified).AddTicks(5696), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 9, 6, 11, 13, 7, 548, DateTimeKind.Unspecified).AddTicks(5697), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 9, 6, 11, 13, 7, 548, DateTimeKind.Unspecified).AddTicks(5698), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 1L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 9, 6, 11, 13, 7, 548, DateTimeKind.Unspecified).AddTicks(6247), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 2L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 9, 6, 11, 13, 7, 548, DateTimeKind.Unspecified).AddTicks(6249), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 3L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 9, 6, 11, 13, 7, 548, DateTimeKind.Unspecified).AddTicks(6249), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 4L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 9, 6, 11, 13, 7, 548, DateTimeKind.Unspecified).AddTicks(6250), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 5L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 9, 6, 11, 13, 7, 548, DateTimeKind.Unspecified).AddTicks(6251), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 6L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 9, 6, 11, 13, 7, 548, DateTimeKind.Unspecified).AddTicks(6252), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 7L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 9, 6, 11, 13, 7, 548, DateTimeKind.Unspecified).AddTicks(6252), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 8L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 9, 6, 11, 13, 7, 548, DateTimeKind.Unspecified).AddTicks(6253), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 9L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 9, 6, 11, 13, 7, 548, DateTimeKind.Unspecified).AddTicks(6254), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 10L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 9, 6, 11, 13, 7, 548, DateTimeKind.Unspecified).AddTicks(6255), new TimeSpan(0, 0, 0, 0, 0)));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "rank_points",
                table: "user_stats",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,2)",
                oldPrecision: 12,
                oldScale: 2);

            migrationBuilder.UpdateData(
                table: "ai_prompts",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 9, 5, 18, 45, 45, 604, DateTimeKind.Unspecified).AddTicks(1774), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 9, 5, 18, 45, 45, 604, DateTimeKind.Unspecified).AddTicks(1546), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 9, 5, 18, 45, 45, 604, DateTimeKind.Unspecified).AddTicks(1546), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 9, 5, 18, 45, 45, 604, DateTimeKind.Unspecified).AddTicks(1548), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 9, 5, 18, 45, 45, 604, DateTimeKind.Unspecified).AddTicks(1548), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 9, 5, 18, 45, 45, 604, DateTimeKind.Unspecified).AddTicks(1550), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 9, 5, 18, 45, 45, 604, DateTimeKind.Unspecified).AddTicks(1550), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 9, 5, 18, 45, 45, 604, DateTimeKind.Unspecified).AddTicks(1552), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 9, 5, 18, 45, 45, 604, DateTimeKind.Unspecified).AddTicks(1552), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 9, 5, 18, 45, 45, 604, DateTimeKind.Unspecified).AddTicks(1554), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 9, 5, 18, 45, 45, 604, DateTimeKind.Unspecified).AddTicks(1554), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 9, 5, 18, 45, 45, 604, DateTimeKind.Unspecified).AddTicks(1555), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 9, 5, 18, 45, 45, 604, DateTimeKind.Unspecified).AddTicks(1556), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 1L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 9, 5, 18, 45, 45, 604, DateTimeKind.Unspecified).AddTicks(2149), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 2L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 9, 5, 18, 45, 45, 604, DateTimeKind.Unspecified).AddTicks(2151), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 3L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 9, 5, 18, 45, 45, 604, DateTimeKind.Unspecified).AddTicks(2152), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 4L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 9, 5, 18, 45, 45, 604, DateTimeKind.Unspecified).AddTicks(2153), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 5L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 9, 5, 18, 45, 45, 604, DateTimeKind.Unspecified).AddTicks(2154), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 6L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 9, 5, 18, 45, 45, 604, DateTimeKind.Unspecified).AddTicks(2155), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 7L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 9, 5, 18, 45, 45, 604, DateTimeKind.Unspecified).AddTicks(2156), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 8L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 9, 5, 18, 45, 45, 604, DateTimeKind.Unspecified).AddTicks(2157), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 9L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 9, 5, 18, 45, 45, 604, DateTimeKind.Unspecified).AddTicks(2158), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 10L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 9, 5, 18, 45, 45, 604, DateTimeKind.Unspecified).AddTicks(2158), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
