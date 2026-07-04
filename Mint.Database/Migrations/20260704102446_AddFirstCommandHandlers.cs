using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Mint.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddFirstCommandHandlers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_transactions_accounts_account_id",
                table: "transactions");

            migrationBuilder.RenameColumn(
                name: "account_id",
                table: "transactions",
                newName: "debet_account_id");

            migrationBuilder.RenameIndex(
                name: "IX_transactions_account_id",
                table: "transactions",
                newName: "IX_transactions_debet_account_id");

            migrationBuilder.AlterColumn<string>(
                name: "user_name",
                table: "users",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "last_name",
                table: "users",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "first_name",
                table: "users",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "user_categories",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "creadit_account_id",
                table: "transactions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "scenarios",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scenarios", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "step_types",
                columns: table => new
                {
                    id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_step_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "steps",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    scenario_id = table.Column<long>(type: "bigint", nullable: false),
                    order_num = table.Column<short>(type: "smallint", nullable: false),
                    step_type_id = table.Column<short>(type: "smallint", nullable: false),
                    message = table.Column<string>(type: "text", nullable: false),
                    is_final = table.Column<bool>(type: "boolean", nullable: false),
                    data = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_steps", x => x.id);
                    table.ForeignKey(
                        name: "FK_steps_scenarios_scenario_id",
                        column: x => x.scenario_id,
                        principalTable: "scenarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_steps_step_types_step_type_id",
                        column: x => x.step_type_id,
                        principalTable: "step_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "buttons",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    parent_step_id = table.Column<long>(type: "bigint", nullable: false),
                    order_num = table.Column<short>(type: "smallint", nullable: false),
                    caption = table.Column<string>(type: "text", nullable: false),
                    action = table.Column<string>(type: "text", nullable: false),
                    next_step_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_buttons", x => x.id);
                    table.ForeignKey(
                        name: "FK_buttons_steps_next_step_id",
                        column: x => x.next_step_id,
                        principalTable: "steps",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_buttons_steps_parent_step_id",
                        column: x => x.parent_step_id,
                        principalTable: "steps",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_sessions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    scenario_id = table.Column<long>(type: "bigint", nullable: false),
                    current_step_id = table.Column<long>(type: "bigint", nullable: false),
                    data = table.Column<string>(type: "text", nullable: false),
                    started_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ScenarioEntityId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_sessions", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_sessions_scenarios_ScenarioEntityId",
                        column: x => x.ScenarioEntityId,
                        principalTable: "scenarios",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_user_sessions_scenarios_scenario_id",
                        column: x => x.scenario_id,
                        principalTable: "scenarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_sessions_steps_current_step_id",
                        column: x => x.current_step_id,
                        principalTable: "steps",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_sessions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "ai_prompts",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 7, 4, 10, 24, 45, 830, DateTimeKind.Unspecified).AddTicks(9116), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.InsertData(
                table: "scenarios",
                columns: new[] { "id", "created_at", "is_active", "name" },
                values: new object[,]
                {
                    { 1L, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, "start" },
                    { 2L, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, "profile" },
                    { 3L, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, "duels" },
                    { 4L, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, "referral" }
                });

            migrationBuilder.InsertData(
                table: "step_types",
                columns: new[] { "id", "description", "name" },
                values: new object[,]
                {
                    { (short)1, "Expects a text input from the user", "text" },
                    { (short)2, "Expects a numeric input (bet amount)", "number" },
                    { (short)3, "Selection from suggested options (buttons)", "choice" },
                    { (short)4, "Information message without input", "info" }
                });

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 1,
                column: "Code",
                value: "ai");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 2,
                column: "Code",
                value: "hardware");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 3,
                column: "Code",
                value: "crypto");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 4,
                column: "Code",
                value: "games");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 5,
                column: "Code",
                value: "sports");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 6,
                column: "Code",
                value: "movies");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 7,
                column: "Code",
                value: "memes");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 8,
                column: "Code",
                value: "cars");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 9,
                column: "Code",
                value: "science");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 10,
                column: "Code",
                value: "soccer");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 11,
                column: "Code",
                value: "fashion");

            migrationBuilder.UpdateData(
                table: "user_categories",
                keyColumn: "id",
                keyValue: 12,
                column: "Code",
                value: "finance");

            migrationBuilder.InsertData(
                table: "steps",
                columns: new[] { "id", "data", "is_final", "message", "order_num", "scenario_id", "step_type_id" },
                values: new object[,]
                {
                    { 1L, null, false, "🎉 Добро пожаловать в Mint!\n\nВаш игровой профиль создан!\n💰 Стартовый баланс: 1000 🪙\n🏆 Ранг: 🌱 Новичок\n\nИспользуйте кнопки ниже для навигации.", (short)1, 1L, (short)3 },
                    { 2L, null, false, "👤 **Ваш игровой профиль**\n━━━━━━━━━━━━━━━━━━━━━━━\n🆔 ID: `{{user_id}}`\n🏆 Ранг: {{rank_emoji}} **{{rank_name}}**\n💰 Баланс: {{balance}} 🪙\n\n📊 **Статистика прогнозов**\n├ Всего: {{total_duels}}\n├ ✅ Успешно: {{wins}}\n├ ❌ Неудачно: {{losses}}\n└ 🎯 Точность: {{winrate}}%\n\n👥 **Рефералы**\n├ Приглашено: {{referral_count}}\n└ Заработано: {{referral_earnings}} 🪙\n\n🎁 **Ежедневный бонус**\n├ Статус: {{bonus_status}}\n├ Дней подряд: {{streak_days}} 🔥\n└ Всего получено: {{total_bonus}} 🪙\n\n📅 В игре с: {{member_since}}", (short)1, 2L, (short)4 },
                    { 3L, null, false, "📊 **Выберите категорию споров:**", (short)1, 3L, (short)3 },
                    { 4L, null, false, "👥 **РЕФЕРАЛЬНАЯ ПРОГРАММА**\n\nПригласи друга и получи **500 монет**, когда он сделает свои первые 3 ставки в любых дуэлях!\n\n🎁 Твоя ссылка:\n`https://t.me/opinion_bot?start={{referral_code}}`\n\n👥 Приглашено друзей: {{referral_count}}\n💰 Заработано монет: {{referral_earnings}} 🪙", (short)1, 4L, (short)4 }
                });

            migrationBuilder.InsertData(
                table: "buttons",
                columns: new[] { "id", "action", "caption", "next_step_id", "order_num", "parent_step_id" },
                values: new object[,]
                {
                    { 1L, "duels", "📊 Дуэли дня", null, (short)1, 1L },
                    { 2L, "profile", "👤 Мой профиль", null, (short)2, 1L },
                    { 3L, "referral", "👥 Пригласить", null, (short)3, 1L },
                    { 4L, "claim_bonus", "🎁 Забрать бонус", null, (short)1, 2L },
                    { 5L, "leaderboard", "📈 Таблица лидеров", null, (short)2, 2L },
                    { 6L, "main_menu", "⬅️ Назад в меню", null, (short)3, 2L },
                    { 7L, "share_referral", "✉️ Переслать другу", null, (short)1, 4L }
                });

            migrationBuilder.CreateIndex(
                name: "IX_transactions_creadit_account_id",
                table: "transactions",
                column: "creadit_account_id");

            migrationBuilder.CreateIndex(
                name: "IX_buttons_next_step_id",
                table: "buttons",
                column: "next_step_id");

            migrationBuilder.CreateIndex(
                name: "IX_buttons_parent_step_id",
                table: "buttons",
                column: "parent_step_id");

            migrationBuilder.CreateIndex(
                name: "IX_steps_scenario_id",
                table: "steps",
                column: "scenario_id");

            migrationBuilder.CreateIndex(
                name: "IX_steps_step_type_id",
                table: "steps",
                column: "step_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_sessions_current_step_id",
                table: "user_sessions",
                column: "current_step_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_sessions_scenario_id",
                table: "user_sessions",
                column: "scenario_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_sessions_ScenarioEntityId",
                table: "user_sessions",
                column: "ScenarioEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_user_sessions_user_id",
                table: "user_sessions",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_accounts_creadit_account_id",
                table: "transactions",
                column: "creadit_account_id",
                principalTable: "accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_accounts_debet_account_id",
                table: "transactions",
                column: "debet_account_id",
                principalTable: "accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_transactions_accounts_creadit_account_id",
                table: "transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_transactions_accounts_debet_account_id",
                table: "transactions");

            migrationBuilder.DropTable(
                name: "buttons");

            migrationBuilder.DropTable(
                name: "user_sessions");

            migrationBuilder.DropTable(
                name: "steps");

            migrationBuilder.DropTable(
                name: "scenarios");

            migrationBuilder.DropTable(
                name: "step_types");

            migrationBuilder.DropIndex(
                name: "IX_transactions_creadit_account_id",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "user_categories");

            migrationBuilder.DropColumn(
                name: "creadit_account_id",
                table: "transactions");

            migrationBuilder.RenameColumn(
                name: "debet_account_id",
                table: "transactions",
                newName: "account_id");

            migrationBuilder.RenameIndex(
                name: "IX_transactions_debet_account_id",
                table: "transactions",
                newName: "IX_transactions_account_id");

            migrationBuilder.AlterColumn<string>(
                name: "user_name",
                table: "users",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "last_name",
                table: "users",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "first_name",
                table: "users",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "ai_prompts",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 6, 28, 18, 20, 16, 706, DateTimeKind.Unspecified).AddTicks(7274), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_accounts_account_id",
                table: "transactions",
                column: "account_id",
                principalTable: "accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
