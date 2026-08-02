using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mint.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddBackButtonToHelp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ai_prompts",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 8, 2, 19, 2, 33, 461, DateTimeKind.Unspecified).AddTicks(9944), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 2, 19, 2, 33, 461, DateTimeKind.Unspecified).AddTicks(9501), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 2, 19, 2, 33, 461, DateTimeKind.Unspecified).AddTicks(9502), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 2, 19, 2, 33, 461, DateTimeKind.Unspecified).AddTicks(9506), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 2, 19, 2, 33, 461, DateTimeKind.Unspecified).AddTicks(9507), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 2, 19, 2, 33, 461, DateTimeKind.Unspecified).AddTicks(9510), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 2, 19, 2, 33, 461, DateTimeKind.Unspecified).AddTicks(9511), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 2, 19, 2, 33, 461, DateTimeKind.Unspecified).AddTicks(9514), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 2, 19, 2, 33, 461, DateTimeKind.Unspecified).AddTicks(9515), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 2, 19, 2, 33, 461, DateTimeKind.Unspecified).AddTicks(9518), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 2, 19, 2, 33, 461, DateTimeKind.Unspecified).AddTicks(9519), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 2, 19, 2, 33, 461, DateTimeKind.Unspecified).AddTicks(9522), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 2, 19, 2, 33, 461, DateTimeKind.Unspecified).AddTicks(9523), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.InsertData(
                table: "buttons",
                columns: new[] { "id", "action", "caption", "next_step_id", "order_num", "parent_step_id" },
                values: new object[] { 12L, "main_menu", "🔙 Назад в меню", null, (short)3, 1L });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "buttons",
                keyColumn: "id",
                keyValue: 12L);

            migrationBuilder.UpdateData(
                table: "ai_prompts",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 8, 2, 18, 19, 38, 495, DateTimeKind.Unspecified).AddTicks(1864), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 2, 18, 19, 38, 495, DateTimeKind.Unspecified).AddTicks(1659), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 2, 18, 19, 38, 495, DateTimeKind.Unspecified).AddTicks(1659), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 2, 18, 19, 38, 495, DateTimeKind.Unspecified).AddTicks(1661), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 2, 18, 19, 38, 495, DateTimeKind.Unspecified).AddTicks(1662), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 2, 18, 19, 38, 495, DateTimeKind.Unspecified).AddTicks(1663), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 2, 18, 19, 38, 495, DateTimeKind.Unspecified).AddTicks(1664), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 2, 18, 19, 38, 495, DateTimeKind.Unspecified).AddTicks(1665), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 2, 18, 19, 38, 495, DateTimeKind.Unspecified).AddTicks(1665), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 2, 18, 19, 38, 495, DateTimeKind.Unspecified).AddTicks(1667), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 2, 18, 19, 38, 495, DateTimeKind.Unspecified).AddTicks(1667), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 2, 18, 19, 38, 495, DateTimeKind.Unspecified).AddTicks(1669), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 2, 18, 19, 38, 495, DateTimeKind.Unspecified).AddTicks(1669), new TimeSpan(0, 0, 0, 0, 0)) });
        }
    }
}
