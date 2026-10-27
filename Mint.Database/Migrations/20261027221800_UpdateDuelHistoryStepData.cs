using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mint.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDuelHistoryStepData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "buttons",
                keyColumn: "id",
                keyValue: 14L,
                column: "caption",
                value: "💼 Итог дня");

            migrationBuilder.UpdateData(
                table: "steps",
                keyColumn: "id",
                keyValue: 11L,
                column: "message",
                value: "💼 **ИТОГ ДНЯ**\n\n{{duel_history_entries}}\n\n───────────────────────\n👤 Всего дуэлей: {{total_duels}}\n✅ Побед: {{wins}} | ❌ Поражений: {{losses}} | 🤝 Ничьих: {{draws}}\n\n───────────────────────\n📈 RP: {{rp_change}}\n💰 Ставки: {{total_bets}} 🪙\n📥 Поступления: {{total_income}} 🪙\n💵 Итого: {{net_total}}");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "buttons",
                keyColumn: "id",
                keyValue: 14L,
                column: "caption",
                value: "📊 Статистика");

            migrationBuilder.UpdateData(
                table: "steps",
                keyColumn: "id",
                keyValue: 11L,
                column: "message",
                value: "📊 **СТАТИСТИКА**\n\n{{duel_history_entries}}\n\n───────────────────────\n👤 Всего дуэлей: {{total_duels}}\n✅ Побед: {{wins}} | ❌ Поражений: {{losses}} | 🤝 Ничьих: {{draws}}\n\n───────────────────────\n📈 RP: {{rp_change}}\n💰 Ставки: {{total_bets}} 🪙\n📥 Поступления: {{total_income}} 🪙\n💵 Итого: {{net_total}}");
        }
    }
}
