using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mint.Database.Migrations
{
    /// <inheritdoc />
    public partial class ReferralLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "invited_by_user_id",
                table: "user_stats",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "caption",
                table: "buttons",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "action",
                table: "buttons",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<short>(
                name: "type",
                table: "buttons",
                type: "smallint",
                nullable: false,
                defaultValueSql: "1");

            migrationBuilder.UpdateData(
                table: "ai_prompts",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 8, 30, 16, 36, 25, 771, DateTimeKind.Unspecified).AddTicks(1115), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 30, 16, 36, 25, 771, DateTimeKind.Unspecified).AddTicks(878), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 30, 16, 36, 25, 771, DateTimeKind.Unspecified).AddTicks(878), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 30, 16, 36, 25, 771, DateTimeKind.Unspecified).AddTicks(881), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 30, 16, 36, 25, 771, DateTimeKind.Unspecified).AddTicks(881), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 30, 16, 36, 25, 771, DateTimeKind.Unspecified).AddTicks(884), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 30, 16, 36, 25, 771, DateTimeKind.Unspecified).AddTicks(884), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 30, 16, 36, 25, 771, DateTimeKind.Unspecified).AddTicks(886), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 30, 16, 36, 25, 771, DateTimeKind.Unspecified).AddTicks(886), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 30, 16, 36, 25, 771, DateTimeKind.Unspecified).AddTicks(887), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 30, 16, 36, 25, 771, DateTimeKind.Unspecified).AddTicks(888), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 30, 16, 36, 25, 771, DateTimeKind.Unspecified).AddTicks(890), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 30, 16, 36, 25, 771, DateTimeKind.Unspecified).AddTicks(890), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "buttons",
                keyColumn: "id",
                keyValue: 1L,
                column: "type",
                value: (short)1);

            migrationBuilder.UpdateData(
                table: "buttons",
                keyColumn: "id",
                keyValue: 2L,
                column: "type",
                value: (short)1);

            migrationBuilder.UpdateData(
                table: "buttons",
                keyColumn: "id",
                keyValue: 3L,
                column: "type",
                value: (short)1);

            migrationBuilder.UpdateData(
                table: "buttons",
                keyColumn: "id",
                keyValue: 4L,
                column: "type",
                value: (short)1);

            migrationBuilder.UpdateData(
                table: "buttons",
                keyColumn: "id",
                keyValue: 5L,
                column: "type",
                value: (short)1);

            migrationBuilder.UpdateData(
                table: "buttons",
                keyColumn: "id",
                keyValue: 6L,
                column: "type",
                value: (short)1);

            migrationBuilder.UpdateData(
                table: "buttons",
                keyColumn: "id",
                keyValue: 7L,
                columns: new[] { "action", "type" },
                values: new object[] { "Присоединяйся к \"Дуэли мнений\" по ссылке: https://t.me/{{bot_username}}?start={{referral_code}}", (short)2 });

            migrationBuilder.UpdateData(
                table: "buttons",
                keyColumn: "id",
                keyValue: 8L,
                column: "type",
                value: (short)1);

            migrationBuilder.UpdateData(
                table: "buttons",
                keyColumn: "id",
                keyValue: 9L,
                column: "type",
                value: (short)1);

            migrationBuilder.UpdateData(
                table: "buttons",
                keyColumn: "id",
                keyValue: 10L,
                column: "type",
                value: (short)1);

            migrationBuilder.UpdateData(
                table: "buttons",
                keyColumn: "id",
                keyValue: 11L,
                column: "type",
                value: (short)1);

            migrationBuilder.UpdateData(
                table: "buttons",
                keyColumn: "id",
                keyValue: 12L,
                column: "type",
                value: (short)1);

            migrationBuilder.InsertData(
                table: "buttons",
                columns: new[] { "id", "action", "caption", "next_step_id", "order_num", "parent_step_id", "type" },
                values: new object[] { 13L, "main_menu", "🔙 Назад в меню", null, (short)2, 4L, (short)1 });

            migrationBuilder.UpdateData(
                table: "steps",
                keyColumn: "id",
                keyValue: 4L,
                column: "message",
                value: "👥 **РЕФЕРАЛЬНАЯ ПРОГРАММА**\n\nПригласи друга и получи **{{referral_amount}} монет**!\n\n🎁 Твоя ссылка:\n`https://t.me/{{bot_username}}?start={{referral_code}}`\n\n👥 Приглашено друзей: {{referral_count}}");

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 1L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 8, 30, 16, 36, 25, 771, DateTimeKind.Unspecified).AddTicks(1488), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 2L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 8, 30, 16, 36, 25, 771, DateTimeKind.Unspecified).AddTicks(1490), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 3L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 8, 30, 16, 36, 25, 771, DateTimeKind.Unspecified).AddTicks(1491), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 4L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 8, 30, 16, 36, 25, 771, DateTimeKind.Unspecified).AddTicks(1491), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 5L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 8, 30, 16, 36, 25, 771, DateTimeKind.Unspecified).AddTicks(1492), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 6L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 8, 30, 16, 36, 25, 771, DateTimeKind.Unspecified).AddTicks(1493), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 7L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 8, 30, 16, 36, 25, 771, DateTimeKind.Unspecified).AddTicks(1494), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 8L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 8, 30, 16, 36, 25, 771, DateTimeKind.Unspecified).AddTicks(1495), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 9L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 8, 30, 16, 36, 25, 771, DateTimeKind.Unspecified).AddTicks(1495), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 10L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 8, 30, 16, 36, 25, 771, DateTimeKind.Unspecified).AddTicks(1496), new TimeSpan(0, 0, 0, 0, 0)));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "buttons",
                keyColumn: "id",
                keyValue: 13L);

            migrationBuilder.DropColumn(
                name: "invited_by_user_id",
                table: "user_stats");

            migrationBuilder.DropColumn(
                name: "type",
                table: "buttons");

            migrationBuilder.AlterColumn<string>(
                name: "caption",
                table: "buttons",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "action",
                table: "buttons",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.UpdateData(
                table: "ai_prompts",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 8, 11, 11, 56, 27, 163, DateTimeKind.Unspecified).AddTicks(7602), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 11, 11, 56, 27, 163, DateTimeKind.Unspecified).AddTicks(7416), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 11, 11, 56, 27, 163, DateTimeKind.Unspecified).AddTicks(7417), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 11, 11, 56, 27, 163, DateTimeKind.Unspecified).AddTicks(7418), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 11, 11, 56, 27, 163, DateTimeKind.Unspecified).AddTicks(7419), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 11, 11, 56, 27, 163, DateTimeKind.Unspecified).AddTicks(7420), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 11, 11, 56, 27, 163, DateTimeKind.Unspecified).AddTicks(7420), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 11, 11, 56, 27, 163, DateTimeKind.Unspecified).AddTicks(7422), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 11, 11, 56, 27, 163, DateTimeKind.Unspecified).AddTicks(7422), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 11, 11, 56, 27, 163, DateTimeKind.Unspecified).AddTicks(7423), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 11, 11, 56, 27, 163, DateTimeKind.Unspecified).AddTicks(7424), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 11, 11, 56, 27, 163, DateTimeKind.Unspecified).AddTicks(7425), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 11, 11, 56, 27, 163, DateTimeKind.Unspecified).AddTicks(7425), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "buttons",
                keyColumn: "id",
                keyValue: 7L,
                column: "action",
                value: "share_referral");

            migrationBuilder.UpdateData(
                table: "steps",
                keyColumn: "id",
                keyValue: 4L,
                column: "message",
                value: "👥 **РЕФЕРАЛЬНАЯ ПРОГРАММА**\n\nПригласи друга и получи **500 монет**, когда он сделает свои первые 3 ставки в любых дуэлях!\n\n🎁 Твоя ссылка:\n`https://t.me/opinion_bot?start={{referral_code}}`\n\n👥 Приглашено друзей: {{referral_count}}\n💰 Всего бонусов: {{total_referral_bonus}} 🪙");

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 1L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 8, 11, 11, 56, 27, 163, DateTimeKind.Unspecified).AddTicks(7953), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 2L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 8, 11, 11, 56, 27, 163, DateTimeKind.Unspecified).AddTicks(7955), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 3L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 8, 11, 11, 56, 27, 163, DateTimeKind.Unspecified).AddTicks(7956), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 4L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 8, 11, 11, 56, 27, 163, DateTimeKind.Unspecified).AddTicks(7957), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 5L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 8, 11, 11, 56, 27, 163, DateTimeKind.Unspecified).AddTicks(7958), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 6L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 8, 11, 11, 56, 27, 163, DateTimeKind.Unspecified).AddTicks(7958), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 7L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 8, 11, 11, 56, 27, 163, DateTimeKind.Unspecified).AddTicks(7959), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 8L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 8, 11, 11, 56, 27, 163, DateTimeKind.Unspecified).AddTicks(7960), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 9L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 8, 11, 11, 56, 27, 163, DateTimeKind.Unspecified).AddTicks(7961), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 10L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 8, 11, 11, 56, 27, 163, DateTimeKind.Unspecified).AddTicks(7961), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
