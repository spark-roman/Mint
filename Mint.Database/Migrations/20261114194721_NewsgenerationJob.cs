using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mint.Database.Migrations
{
    /// <inheritdoc />
    public partial class NewsgenerationJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "news_id",
                table: "duels",
                type: "bigint",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ai_prompts",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 14, 19, 47, 20, 617, DateTimeKind.Unspecified).AddTicks(2395), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 14, 19, 47, 20, 617, DateTimeKind.Unspecified).AddTicks(2191), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 14, 19, 47, 20, 617, DateTimeKind.Unspecified).AddTicks(2192), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 14, 19, 47, 20, 617, DateTimeKind.Unspecified).AddTicks(2194), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 14, 19, 47, 20, 617, DateTimeKind.Unspecified).AddTicks(2194), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 14, 19, 47, 20, 617, DateTimeKind.Unspecified).AddTicks(2196), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 14, 19, 47, 20, 617, DateTimeKind.Unspecified).AddTicks(2196), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 14, 19, 47, 20, 617, DateTimeKind.Unspecified).AddTicks(2198), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 14, 19, 47, 20, 617, DateTimeKind.Unspecified).AddTicks(2198), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 14, 19, 47, 20, 617, DateTimeKind.Unspecified).AddTicks(2199), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 14, 19, 47, 20, 617, DateTimeKind.Unspecified).AddTicks(2200), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 14, 19, 47, 20, 617, DateTimeKind.Unspecified).AddTicks(2201), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 14, 19, 47, 20, 617, DateTimeKind.Unspecified).AddTicks(2201), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 7,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 14, 19, 47, 20, 617, DateTimeKind.Unspecified).AddTicks(2203), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 14, 19, 47, 20, 617, DateTimeKind.Unspecified).AddTicks(2203), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 9,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 14, 19, 47, 20, 617, DateTimeKind.Unspecified).AddTicks(2204), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 14, 19, 47, 20, 617, DateTimeKind.Unspecified).AddTicks(2205), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 100,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 14, 19, 47, 20, 617, DateTimeKind.Unspecified).AddTicks(2206), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 14, 19, 47, 20, 617, DateTimeKind.Unspecified).AddTicks(2206), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 1L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 14, 19, 47, 20, 617, DateTimeKind.Unspecified).AddTicks(2806), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 2L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 14, 19, 47, 20, 617, DateTimeKind.Unspecified).AddTicks(2808), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 3L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 14, 19, 47, 20, 617, DateTimeKind.Unspecified).AddTicks(2809), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 4L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 14, 19, 47, 20, 617, DateTimeKind.Unspecified).AddTicks(2810), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 5L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 14, 19, 47, 20, 617, DateTimeKind.Unspecified).AddTicks(2810), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 6L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 14, 19, 47, 20, 617, DateTimeKind.Unspecified).AddTicks(2811), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 7L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 14, 19, 47, 20, 617, DateTimeKind.Unspecified).AddTicks(2817), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 8L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 14, 19, 47, 20, 617, DateTimeKind.Unspecified).AddTicks(2818), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 9L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 14, 19, 47, 20, 617, DateTimeKind.Unspecified).AddTicks(2819), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 10L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 14, 19, 47, 20, 617, DateTimeKind.Unspecified).AddTicks(2820), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 11L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 14, 19, 47, 20, 617, DateTimeKind.Unspecified).AddTicks(2820), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 12L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 14, 19, 47, 20, 617, DateTimeKind.Unspecified).AddTicks(2821), new TimeSpan(0, 0, 0, 0, 0)));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "news_id",
                table: "duels");

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
        }
    }
}
