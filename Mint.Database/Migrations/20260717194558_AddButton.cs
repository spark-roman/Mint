using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mint.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddButton : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ai_prompts",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 7, 17, 19, 45, 58, 137, DateTimeKind.Unspecified).AddTicks(7075), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 17, 19, 45, 58, 137, DateTimeKind.Unspecified).AddTicks(6700), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 17, 19, 45, 58, 137, DateTimeKind.Unspecified).AddTicks(6700), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 17, 19, 45, 58, 137, DateTimeKind.Unspecified).AddTicks(6702), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 17, 19, 45, 58, 137, DateTimeKind.Unspecified).AddTicks(6702), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 17, 19, 45, 58, 137, DateTimeKind.Unspecified).AddTicks(6704), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 17, 19, 45, 58, 137, DateTimeKind.Unspecified).AddTicks(6704), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 17, 19, 45, 58, 137, DateTimeKind.Unspecified).AddTicks(6706), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 17, 19, 45, 58, 137, DateTimeKind.Unspecified).AddTicks(6706), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 17, 19, 45, 58, 137, DateTimeKind.Unspecified).AddTicks(6707), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 17, 19, 45, 58, 137, DateTimeKind.Unspecified).AddTicks(6708), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 17, 19, 45, 58, 137, DateTimeKind.Unspecified).AddTicks(6709), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 17, 19, 45, 58, 137, DateTimeKind.Unspecified).AddTicks(6709), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.InsertData(
                table: "buttons",
                columns: new[] { "id", "action", "caption", "next_step_id", "order_num", "parent_step_id" },
                values: new object[] { 9L, "duels", "🔙 Вернуться к дуэлям", null, (short)1, 8L });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "buttons",
                keyColumn: "id",
                keyValue: 9L);

            migrationBuilder.UpdateData(
                table: "ai_prompts",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 7, 15, 16, 59, 32, 428, DateTimeKind.Unspecified).AddTicks(739), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 15, 16, 59, 32, 428, DateTimeKind.Unspecified).AddTicks(462), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 15, 16, 59, 32, 428, DateTimeKind.Unspecified).AddTicks(463), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 15, 16, 59, 32, 428, DateTimeKind.Unspecified).AddTicks(464), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 15, 16, 59, 32, 428, DateTimeKind.Unspecified).AddTicks(465), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 15, 16, 59, 32, 428, DateTimeKind.Unspecified).AddTicks(466), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 15, 16, 59, 32, 428, DateTimeKind.Unspecified).AddTicks(467), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 15, 16, 59, 32, 428, DateTimeKind.Unspecified).AddTicks(468), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 15, 16, 59, 32, 428, DateTimeKind.Unspecified).AddTicks(468), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 15, 16, 59, 32, 428, DateTimeKind.Unspecified).AddTicks(470), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 15, 16, 59, 32, 428, DateTimeKind.Unspecified).AddTicks(470), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 15, 16, 59, 32, 428, DateTimeKind.Unspecified).AddTicks(471), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 15, 16, 59, 32, 428, DateTimeKind.Unspecified).AddTicks(471), new TimeSpan(0, 0, 0, 0, 0)) });
        }
    }
}
