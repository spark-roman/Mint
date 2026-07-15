using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Mint.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddDuels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AccountEntityId",
                table: "votes",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "transaction_id",
                table: "votes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

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

            migrationBuilder.InsertData(
                table: "steps",
                columns: new[] { "id", "data", "is_final", "message", "order_num", "scenario_id", "step_type_id" },
                values: new object[,]
                {
                    { 7L, null, false, "📊 **ДУЭЛИ ДНЯ**\n\nВыберите категорию для спора:{{categories_list}}", (short)1, 3L, (short)3 },
                    { 8L, null, false, "🤖 **ДУЭЛЬ №{{duel_id}}** (Категория: {{category_name}})\n───────────────────────\n❓ **Вопрос:** {{question}}\n\n📝 **Контекст:** {{description}}\n\n───────────────────────\n⏱ До закрытия спора: {{time_left}}\n👇 **Сделай свой прогноз:**", (short)2, 3L, (short)4 },
                    { 9L, null, false, "💰 **ВАШ ПРОГНОЗ: \"{{selected_option}}\"**\n\n💳 Ваш текущий баланс: {{balance}} 🪙\n\nВыберите сумму ставки из шаблонов ниже или введите любое число вручную:", (short)3, 3L, (short)2 },
                    { 10L, null, true, "✅ **СТАВКА УСПЕШНО ПРИНЯТА!**\n\n🎯 Ваш выбор: \"{{selected_option}}\"\n📉 Сумма спора: {{bet_amount}} 🪙\n⏳ Расчет дуэли: через {{time_left}}\n\nСчитаешь, что твои друзья в чатах думают иначе?\nОтправь им этот спор, и пускай они попробуют переубедить ИИ.", (short)4, 3L, (short)4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_votes_AccountEntityId",
                table: "votes",
                column: "AccountEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_votes_transaction_id",
                table: "votes",
                column: "transaction_id");

            migrationBuilder.AddForeignKey(
                name: "FK_votes_accounts_AccountEntityId",
                table: "votes",
                column: "AccountEntityId",
                principalTable: "accounts",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_votes_transactions_transaction_id",
                table: "votes",
                column: "transaction_id",
                principalTable: "transactions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_votes_accounts_AccountEntityId",
                table: "votes");

            migrationBuilder.DropForeignKey(
                name: "FK_votes_transactions_transaction_id",
                table: "votes");

            migrationBuilder.DropIndex(
                name: "IX_votes_AccountEntityId",
                table: "votes");

            migrationBuilder.DropIndex(
                name: "IX_votes_transaction_id",
                table: "votes");

            migrationBuilder.DeleteData(
                table: "steps",
                keyColumn: "id",
                keyValue: 7L);

            migrationBuilder.DeleteData(
                table: "steps",
                keyColumn: "id",
                keyValue: 8L);

            migrationBuilder.DeleteData(
                table: "steps",
                keyColumn: "id",
                keyValue: 9L);

            migrationBuilder.DeleteData(
                table: "steps",
                keyColumn: "id",
                keyValue: 10L);

            migrationBuilder.DropColumn(
                name: "AccountEntityId",
                table: "votes");

            migrationBuilder.DropColumn(
                name: "transaction_id",
                table: "votes");

            migrationBuilder.UpdateData(
                table: "ai_prompts",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 7, 11, 13, 48, 24, 858, DateTimeKind.Unspecified).AddTicks(8880), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 11, 13, 48, 24, 858, DateTimeKind.Unspecified).AddTicks(8652), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 11, 13, 48, 24, 858, DateTimeKind.Unspecified).AddTicks(8653), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 11, 13, 48, 24, 858, DateTimeKind.Unspecified).AddTicks(8655), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 11, 13, 48, 24, 858, DateTimeKind.Unspecified).AddTicks(8655), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 11, 13, 48, 24, 858, DateTimeKind.Unspecified).AddTicks(8657), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 11, 13, 48, 24, 858, DateTimeKind.Unspecified).AddTicks(8657), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 11, 13, 48, 24, 858, DateTimeKind.Unspecified).AddTicks(8658), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 11, 13, 48, 24, 858, DateTimeKind.Unspecified).AddTicks(8659), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 11, 13, 48, 24, 858, DateTimeKind.Unspecified).AddTicks(8660), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 11, 13, 48, 24, 858, DateTimeKind.Unspecified).AddTicks(8660), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 11, 13, 48, 24, 858, DateTimeKind.Unspecified).AddTicks(8662), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 11, 13, 48, 24, 858, DateTimeKind.Unspecified).AddTicks(8662), new TimeSpan(0, 0, 0, 0, 0)) });
        }
    }
}
