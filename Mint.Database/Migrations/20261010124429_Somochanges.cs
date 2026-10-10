using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Mint.Database.Migrations
{
    /// <inheritdoc />
    public partial class Somochanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_payouts_accounts_account_id",
                table: "payouts");

            migrationBuilder.DropIndex(
                name: "idx_payouts_vote_id",
                table: "payouts");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "payouts",
                newName: "planned_at");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "processed_at",
                table: "payouts",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<int>(
                name: "RetryCount",
                table: "payouts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "bonus_type_id",
                table: "payouts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "debit_account_id",
                table: "payouts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "payouts",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "failure_reason",
                table: "payouts",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ai_prompts",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 10, 10, 12, 44, 28, 517, DateTimeKind.Unspecified).AddTicks(4731), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 10, 10, 12, 44, 28, 517, DateTimeKind.Unspecified).AddTicks(4501), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 10, 10, 12, 44, 28, 517, DateTimeKind.Unspecified).AddTicks(4502), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 10, 10, 12, 44, 28, 517, DateTimeKind.Unspecified).AddTicks(4504), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 10, 10, 12, 44, 28, 517, DateTimeKind.Unspecified).AddTicks(4504), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 10, 10, 12, 44, 28, 517, DateTimeKind.Unspecified).AddTicks(4506), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 10, 10, 12, 44, 28, 517, DateTimeKind.Unspecified).AddTicks(4506), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 10, 10, 12, 44, 28, 517, DateTimeKind.Unspecified).AddTicks(4507), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 10, 10, 12, 44, 28, 517, DateTimeKind.Unspecified).AddTicks(4508), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 10, 10, 12, 44, 28, 517, DateTimeKind.Unspecified).AddTicks(4509), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 10, 10, 12, 44, 28, 517, DateTimeKind.Unspecified).AddTicks(4509), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 10, 10, 12, 44, 28, 517, DateTimeKind.Unspecified).AddTicks(4511), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 10, 10, 12, 44, 28, 517, DateTimeKind.Unspecified).AddTicks(4511), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.InsertData(
                table: "bonus_types",
                columns: new[] { "id", "code", "created_at", "description", "is_active", "name", "updated_at" },
                values: new object[] { 7, "refund", new DateTimeOffset(new DateTime(2026, 10, 10, 12, 44, 28, 517, DateTimeKind.Unspecified).AddTicks(4512), new TimeSpan(0, 0, 0, 0, 0)), "Возврат средств пользователю", true, "Возврат", new DateTimeOffset(new DateTime(2026, 10, 10, 12, 44, 28, 517, DateTimeKind.Unspecified).AddTicks(4513), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "steps",
                keyColumn: "id",
                keyValue: 5L,
                column: "message",
                value: "🆘 **Помощь по боту \"Дуэль мнений\"**\n\n**Как это работает?**\n\n📊 **Дуэли дня**\nВыберите категорию и сделайте прогноз на актуальный спор.\nПобедите, если ваше мнение совпадет с мнением большинства!\n\n👤 **Мой профиль**\nСледите за своей статистикой, балансом и рангом.\nЗабирайте **ежедневный бонус** — он доступен раз в 24 часа.\nЧем дольше вы забираете бонус подряд, тем больше награда за серию!\n\n👥 **Пригласить**\nПриглашайте друзей по ссылке.\n\n💰 **Бонусы:**\n• **Ежедневный:** Доступен 1 раз в 24 часа.\n• **За серию:** Начисляется за 7 дней подряд получения бонуса.\n• **Реферальный:** За приглашенных друзей.\n\n🏆 **Ранги:**\nРанг начисляется исходя из очков, полученных за все время игры.\nЧем больше вы участвуете и выигрываете, тем выше ваш ранг.\nРанг отображается в профиле и дает уважение среди игроков!\n\nУдачи в спорах! 🍀");

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 1L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 10, 10, 12, 44, 28, 517, DateTimeKind.Unspecified).AddTicks(5095), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 2L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 10, 10, 12, 44, 28, 517, DateTimeKind.Unspecified).AddTicks(5097), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 3L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 10, 10, 12, 44, 28, 517, DateTimeKind.Unspecified).AddTicks(5098), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 4L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 10, 10, 12, 44, 28, 517, DateTimeKind.Unspecified).AddTicks(5098), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 5L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 10, 10, 12, 44, 28, 517, DateTimeKind.Unspecified).AddTicks(5099), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 6L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 10, 10, 12, 44, 28, 517, DateTimeKind.Unspecified).AddTicks(5100), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 7L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 10, 10, 12, 44, 28, 517, DateTimeKind.Unspecified).AddTicks(5105), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 8L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 10, 10, 12, 44, 28, 517, DateTimeKind.Unspecified).AddTicks(5106), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 9L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 10, 10, 12, 44, 28, 517, DateTimeKind.Unspecified).AddTicks(5107), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 10L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 10, 10, 12, 44, 28, 517, DateTimeKind.Unspecified).AddTicks(5108), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.InsertData(
                table: "system_settings",
                columns: new[] { "id", "description", "key", "updated_at", "value" },
                values: new object[,]
                {
                    { 11L, "Размер батчей", "BatchSize", new DateTimeOffset(new DateTime(2026, 10, 10, 12, 44, 28, 517, DateTimeKind.Unspecified).AddTicks(5109), new TimeSpan(0, 0, 0, 0, 0)), "100" },
                    { 12L, "Колличество батчей в одном запуске", "MaxBatchesPerRun", new DateTimeOffset(new DateTime(2026, 10, 10, 12, 44, 28, 517, DateTimeKind.Unspecified).AddTicks(5109), new TimeSpan(0, 0, 0, 0, 0)), "2000000" }
                });

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 10,
                column: "name",
                value: "Футбол & Спорт");

            migrationBuilder.CreateIndex(
                name: "idx_payouts_status_created_at",
                table: "payouts",
                columns: new[] { "status", "planned_at" });

            migrationBuilder.CreateIndex(
                name: "idx_payouts_vote_id",
                table: "payouts",
                column: "vote_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_payouts_status_created_at",
                table: "payouts");

            migrationBuilder.DropIndex(
                name: "idx_payouts_vote_id",
                table: "payouts");

            migrationBuilder.DeleteData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 11L);

            migrationBuilder.DeleteData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 12L);

            migrationBuilder.DropColumn(
                name: "RetryCount",
                table: "payouts");

            migrationBuilder.DropColumn(
                name: "bonus_type_id",
                table: "payouts");

            migrationBuilder.DropColumn(
                name: "debit_account_id",
                table: "payouts");

            migrationBuilder.DropColumn(
                name: "description",
                table: "payouts");

            migrationBuilder.DropColumn(
                name: "failure_reason",
                table: "payouts");

            migrationBuilder.RenameColumn(
                name: "planned_at",
                table: "payouts",
                newName: "created_at");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "processed_at",
                table: "payouts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "ai_prompts",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 9, 22, 21, 15, 8, 850, DateTimeKind.Unspecified).AddTicks(3094), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 9, 22, 21, 15, 8, 850, DateTimeKind.Unspecified).AddTicks(2818), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 9, 22, 21, 15, 8, 850, DateTimeKind.Unspecified).AddTicks(2819), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 9, 22, 21, 15, 8, 850, DateTimeKind.Unspecified).AddTicks(2821), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 9, 22, 21, 15, 8, 850, DateTimeKind.Unspecified).AddTicks(2821), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 9, 22, 21, 15, 8, 850, DateTimeKind.Unspecified).AddTicks(2823), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 9, 22, 21, 15, 8, 850, DateTimeKind.Unspecified).AddTicks(2823), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 9, 22, 21, 15, 8, 850, DateTimeKind.Unspecified).AddTicks(2824), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 9, 22, 21, 15, 8, 850, DateTimeKind.Unspecified).AddTicks(2825), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 9, 22, 21, 15, 8, 850, DateTimeKind.Unspecified).AddTicks(2826), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 9, 22, 21, 15, 8, 850, DateTimeKind.Unspecified).AddTicks(2826), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 9, 22, 21, 15, 8, 850, DateTimeKind.Unspecified).AddTicks(2828), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 9, 22, 21, 15, 8, 850, DateTimeKind.Unspecified).AddTicks(2828), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "steps",
                keyColumn: "id",
                keyValue: 5L,
                column: "message",
                value: "🆘 **Помощь по боту \"Дуэль мнений\"**\n\n**Как это работает?**\n\n📊 **Дуэли дня**\nВыберите категорию и сделайте прогноз на актуальный спор.\nПобедите, если ваше мнение совпадет с мнением большинства!\n\n👤 **Мой профиль**\nСледите за своей статистикой, балансом и рангом.\nЗабирайте **ежедневный бонус** — он доступен раз в 24 часа.\nЧем дольше вы забираете бонус подряд, тем больше награда за серию!\n\n👥 **Пригласить**\nПриглашайте друзей по ссылке.\nКогда друг сделает свои первые 3 ставки, вы оба получите бонус!\n\n💰 **Бонусы:**\n• **Ежедневный:** Доступен 1 раз в 24 часа.\n• **За серию:** Начисляется за 7 дней подряд получения бонуса.\n• **Реферальный:** За приглашенных друзей.\n\n🏆 **Ранги:**\nРанг начисляется исходя из очков, полученных за все время игры.\nЧем больше вы участвуете и выигрываете, тем выше ваш ранг.\nРанг отображается в профиле и дает уважение среди игроков!\n\nУдачи в спорах! 🍀");

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 1L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 9, 22, 21, 15, 8, 850, DateTimeKind.Unspecified).AddTicks(3468), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 2L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 9, 22, 21, 15, 8, 850, DateTimeKind.Unspecified).AddTicks(3470), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 3L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 9, 22, 21, 15, 8, 850, DateTimeKind.Unspecified).AddTicks(3471), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 4L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 9, 22, 21, 15, 8, 850, DateTimeKind.Unspecified).AddTicks(3472), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 5L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 9, 22, 21, 15, 8, 850, DateTimeKind.Unspecified).AddTicks(3477), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 6L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 9, 22, 21, 15, 8, 850, DateTimeKind.Unspecified).AddTicks(3478), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 7L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 9, 22, 21, 15, 8, 850, DateTimeKind.Unspecified).AddTicks(3479), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 8L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 9, 22, 21, 15, 8, 850, DateTimeKind.Unspecified).AddTicks(3480), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 9L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 9, 22, 21, 15, 8, 850, DateTimeKind.Unspecified).AddTicks(3480), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: 10L,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 9, 22, 21, 15, 8, 850, DateTimeKind.Unspecified).AddTicks(3481), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 10,
                column: "name",
                value: "Космос футбола & Спорт");

            migrationBuilder.CreateIndex(
                name: "idx_payouts_vote_id",
                table: "payouts",
                column: "vote_id");

            migrationBuilder.AddForeignKey(
                name: "FK_payouts_accounts_account_id",
                table: "payouts",
                column: "account_id",
                principalTable: "accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
