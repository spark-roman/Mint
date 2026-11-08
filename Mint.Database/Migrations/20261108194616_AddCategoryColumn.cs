using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mint.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "is_active_for_ai",
                table: "user_categories",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddColumn<bool>(
                name: "is_active_for_users",
                table: "user_categories",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.UpdateData(
                table: "ai_prompts",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(5370), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(4944), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(4944), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(4947), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(4947), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(4949), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(4949), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(4951), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(4951), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(4953), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(4953), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(4954), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(4955), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 7,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(4956), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(4957), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 9,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(4959), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(4959), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 100,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(4961), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(4961), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 1L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(5894), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 2L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(5897), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 3L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(5898), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 4L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(5898), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 5L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(5899), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 6L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(5900), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 7L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(5901), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 8L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(5902), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 9L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(5902), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 10L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(5903), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 11L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(5904), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 12L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 19, 46, 16, 440, DateTimeKind.Unspecified).AddTicks(5905), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 1,
                column: "is_active_for_users",
                value: true);

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 2,
                column: "is_active_for_users",
                value: true);

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 3,
                column: "is_active_for_users",
                value: true);

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 4,
                column: "is_active_for_users",
                value: true);

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 5,
                column: "is_active_for_users",
                value: true);

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 6,
                column: "is_active_for_users",
                value: true);

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 7,
                column: "is_active_for_users",
                value: true);

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 8,
                column: "is_active_for_users",
                value: true);

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 9,
                column: "is_active_for_users",
                value: true);

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 10,
                column: "is_active_for_users",
                value: true);

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 11,
                column: "is_active_for_users",
                value: true);

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 12,
                column: "is_active_for_users",
                value: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_active_for_users",
                table: "user_categories");

            migrationBuilder.AlterColumn<bool>(
                name: "is_active_for_ai",
                table: "user_categories",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.UpdateData(
                table: "ai_prompts",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 453, DateTimeKind.Unspecified).AddTicks(9679), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 453, DateTimeKind.Unspecified).AddTicks(9434), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 453, DateTimeKind.Unspecified).AddTicks(9434), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 453, DateTimeKind.Unspecified).AddTicks(9437), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 453, DateTimeKind.Unspecified).AddTicks(9438), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 453, DateTimeKind.Unspecified).AddTicks(9439), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 453, DateTimeKind.Unspecified).AddTicks(9440), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 453, DateTimeKind.Unspecified).AddTicks(9441), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 453, DateTimeKind.Unspecified).AddTicks(9441), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 453, DateTimeKind.Unspecified).AddTicks(9443), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 453, DateTimeKind.Unspecified).AddTicks(9443), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 453, DateTimeKind.Unspecified).AddTicks(9445), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 453, DateTimeKind.Unspecified).AddTicks(9445), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 7,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 453, DateTimeKind.Unspecified).AddTicks(9446), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 453, DateTimeKind.Unspecified).AddTicks(9447), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 9,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 453, DateTimeKind.Unspecified).AddTicks(9448), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 453, DateTimeKind.Unspecified).AddTicks(9448), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 100,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 453, DateTimeKind.Unspecified).AddTicks(9450), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 453, DateTimeKind.Unspecified).AddTicks(9450), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 1L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 454, DateTimeKind.Unspecified).AddTicks(69), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 2L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 454, DateTimeKind.Unspecified).AddTicks(70), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 3L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 454, DateTimeKind.Unspecified).AddTicks(71), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 4L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 454, DateTimeKind.Unspecified).AddTicks(72), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 5L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 454, DateTimeKind.Unspecified).AddTicks(73), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 6L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 454, DateTimeKind.Unspecified).AddTicks(73), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 7L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 454, DateTimeKind.Unspecified).AddTicks(74), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 8L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 454, DateTimeKind.Unspecified).AddTicks(75), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 9L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 454, DateTimeKind.Unspecified).AddTicks(76), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 10L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 454, DateTimeKind.Unspecified).AddTicks(77), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 11L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 454, DateTimeKind.Unspecified).AddTicks(77), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 12L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 11, 8, 11, 49, 9, 454, DateTimeKind.Unspecified).AddTicks(78), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
