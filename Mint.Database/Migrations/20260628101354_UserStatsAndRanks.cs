using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Mint.Database.Migrations
{
    /// <inheritdoc />
    public partial class UserStatsAndRanks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ranks_config",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    emoji = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    min_points = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ranks_config", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_stats",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    rank_points = table.Column<int>(type: "integer", nullable: false),
                    total_wins = table.Column<int>(type: "integer", nullable: false),
                    total_losses = table.Column<int>(type: "integer", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_stats", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_stats_users_user_id",
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
                value: new DateTimeOffset(new DateTime(2026, 6, 28, 10, 13, 54, 196, DateTimeKind.Unspecified).AddTicks(6182), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.InsertData(
                table: "ranks_config",
                columns: new[] { "id", "code", "emoji", "min_points", "name" },
                values: new object[,]
                {
                    { 1, "newbie", "🥚", 0, "Новичок" },
                    { 2, "analyst", "📈", 20, "Аналитик" },
                    { 3, "expert", "🧠", 100, "Эксперт" },
                    { 4, "seer", "🔮", 500, "Провидец" },
                    { 5, "oracle", "👁️", 2000, "Оракул" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ranks_config_code",
                table: "ranks_config",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_stats_user_id",
                table: "user_stats",
                column: "user_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ranks_config");

            migrationBuilder.DropTable(
                name: "user_stats");

            migrationBuilder.UpdateData(
                table: "ai_prompts",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 6, 28, 7, 37, 13, 886, DateTimeKind.Unspecified).AddTicks(6545), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
