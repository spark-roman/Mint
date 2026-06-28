using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Mint.Database.Migrations
{
    /// <inheritdoc />
    public partial class BonusStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "bonus_type_id",
                table: "transactions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "bounus_type_id",
                table: "transactions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "bonus_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bonus_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_bonus_stats",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    is_start_bonus_claimed = table.Column<bool>(type: "boolean", nullable: false),
                    current_daily_streak = table.Column<int>(type: "integer", nullable: false),
                    last_daily_claimed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    next_daily_available_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    total_referral_bonuses_claimed = table.Column<int>(type: "integer", nullable: false),
                    last_rating_bonus_claimed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_bonus_stats", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_bonus_stats_users_user_id",
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
                value: new DateTimeOffset(new DateTime(2026, 6, 28, 18, 20, 16, 706, DateTimeKind.Unspecified).AddTicks(7274), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateIndex(
                name: "IX_transactions_bonus_type_id",
                table: "transactions",
                column: "bonus_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_bonus_stats_user_id",
                table: "user_bonus_stats",
                column: "user_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_bonus_types_bonus_type_id",
                table: "transactions",
                column: "bonus_type_id",
                principalTable: "bonus_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_transactions_bonus_types_bonus_type_id",
                table: "transactions");

            migrationBuilder.DropTable(
                name: "bonus_types");

            migrationBuilder.DropTable(
                name: "user_bonus_stats");

            migrationBuilder.DropIndex(
                name: "IX_transactions_bonus_type_id",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "bonus_type_id",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "bounus_type_id",
                table: "transactions");

            migrationBuilder.UpdateData(
                table: "ai_prompts",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 6, 28, 13, 19, 57, 399, DateTimeKind.Unspecified).AddTicks(4533), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
