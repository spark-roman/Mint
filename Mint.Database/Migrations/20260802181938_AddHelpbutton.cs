using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mint.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddHelpbutton : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.InsertData(
                table: "buttons",
                columns: new[] { "id", "action", "caption", "next_step_id", "order_num", "parent_step_id" },
                values: new object[] { 11L, "help", "🆘 Помощь", null, (short)4, 1L });

            migrationBuilder.InsertData(
                table: "scenarios",
                columns: new[] { "id", "created_at", "is_active", "name" },
                values: new object[] { 6L, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, "help" });

            migrationBuilder.InsertData(
                table: "steps",
                columns: new[] { "id", "data", "is_final", "message", "order_num", "scenario_id", "step_type_id" },
                values: new object[] { 5L, null, true, "🆘 **Помощь по боту \"Дуэль мнений\"**\n\n**Как это работает?**\n\n📊 **Дуэли дня**\nВыберите категорию и сделайте прогноз на актуальный спор.\nПобедите, если ваше мнение совпадет с мнением большинства!\n\n👤 **Мой профиль**\nСледите за своей статистикой, балансом и рангом.\nЗабирайте **ежедневный бонус** — он доступен раз в 24 часа.\nЧем дольше вы забираете бонус подряд, тем больше награда за серию!\n\n👥 **Пригласить**\nПриглашайте друзей по ссылке.\nКогда друг сделает свои первые 3 ставки, вы оба получите бонус!\n\n💰 **Бонусы:**\n• **Ежедневный:** Доступен 1 раз в 24 часа.\n• **За серию:** Начисляется за 7 дней подряд получения бонуса.\n• **Реферальный:** За приглашенных друзей.\n\n🏆 **Ранги:**\nРанг начисляется исходя из очков, полученных за все время игры.\nЧем больше вы участвуете и выигрываете, тем выше ваш ранг.\nРанг отображается в профиле и дает уважение среди игроков!\n\nУдачи в спорах! 🍀", (short)1, 6L, (short)4 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "buttons",
                keyColumn: "id",
                keyValue: 11L);

            migrationBuilder.DeleteData(
                table: "steps",
                keyColumn: "id",
                keyValue: 5L);

            migrationBuilder.DeleteData(
                table: "scenarios",
                keyColumn: "id",
                keyValue: 6L);

            migrationBuilder.UpdateData(
                table: "ai_prompts",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 8, 2, 14, 19, 46, 301, DateTimeKind.Unspecified).AddTicks(4700), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 2, 14, 19, 46, 301, DateTimeKind.Unspecified).AddTicks(4500), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 2, 14, 19, 46, 301, DateTimeKind.Unspecified).AddTicks(4500), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 2, 14, 19, 46, 301, DateTimeKind.Unspecified).AddTicks(4502), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 2, 14, 19, 46, 301, DateTimeKind.Unspecified).AddTicks(4502), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 2, 14, 19, 46, 301, DateTimeKind.Unspecified).AddTicks(4504), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 2, 14, 19, 46, 301, DateTimeKind.Unspecified).AddTicks(4504), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 2, 14, 19, 46, 301, DateTimeKind.Unspecified).AddTicks(4506), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 2, 14, 19, 46, 301, DateTimeKind.Unspecified).AddTicks(4506), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 2, 14, 19, 46, 301, DateTimeKind.Unspecified).AddTicks(4508), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 2, 14, 19, 46, 301, DateTimeKind.Unspecified).AddTicks(4508), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 8, 2, 14, 19, 46, 301, DateTimeKind.Unspecified).AddTicks(4509), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 8, 2, 14, 19, 46, 301, DateTimeKind.Unspecified).AddTicks(4510), new TimeSpan(0, 0, 0, 0, 0)) });
        }
    }
}
