using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Mint.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddNews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "rss_sources",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    category_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rss_sources", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "news",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    rss_source_id = table.Column<long>(type: "bigint", nullable: true),
                    title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    link = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    content = table.Column<string>(type: "text", nullable: true),
                    author = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    category_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    published_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_processed = table.Column<bool>(type: "boolean", nullable: false),
                    processed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_news", x => x.id);
                    table.ForeignKey(
                        name: "FK_news_rss_sources_rss_source_id",
                        column: x => x.rss_source_id,
                        principalTable: "rss_sources",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.UpdateData(
                table: "ai_prompts",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 7, 22, 21, 9, 49, 525, DateTimeKind.Unspecified).AddTicks(797), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 22, 21, 9, 49, 525, DateTimeKind.Unspecified).AddTicks(571), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 22, 21, 9, 49, 525, DateTimeKind.Unspecified).AddTicks(571), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 22, 21, 9, 49, 525, DateTimeKind.Unspecified).AddTicks(573), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 22, 21, 9, 49, 525, DateTimeKind.Unspecified).AddTicks(573), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 22, 21, 9, 49, 525, DateTimeKind.Unspecified).AddTicks(575), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 22, 21, 9, 49, 525, DateTimeKind.Unspecified).AddTicks(575), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 22, 21, 9, 49, 525, DateTimeKind.Unspecified).AddTicks(577), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 22, 21, 9, 49, 525, DateTimeKind.Unspecified).AddTicks(577), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 22, 21, 9, 49, 525, DateTimeKind.Unspecified).AddTicks(578), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 22, 21, 9, 49, 525, DateTimeKind.Unspecified).AddTicks(579), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 22, 21, 9, 49, 525, DateTimeKind.Unspecified).AddTicks(580), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 22, 21, 9, 49, 525, DateTimeKind.Unspecified).AddTicks(580), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.CreateIndex(
                name: "idx_news_category",
                table: "news",
                column: "category_code");

            migrationBuilder.CreateIndex(
                name: "idx_news_category_processed",
                table: "news",
                columns: new[] { "category_code", "is_processed" });

            migrationBuilder.CreateIndex(
                name: "idx_news_created_at",
                table: "news",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "idx_news_is_processed",
                table: "news",
                column: "is_processed");

            migrationBuilder.CreateIndex(
                name: "idx_news_link_unique",
                table: "news",
                column: "link",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_news_processed_published",
                table: "news",
                columns: new[] { "is_processed", "published_at" });

            migrationBuilder.CreateIndex(
                name: "idx_news_published_at_desc",
                table: "news",
                column: "published_at",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "idx_news_rss_source",
                table: "news",
                column: "rss_source_id");

            migrationBuilder.CreateIndex(
                name: "idx_rss_sources_active",
                table: "rss_sources",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "idx_rss_sources_category",
                table: "rss_sources",
                column: "category_code");

            migrationBuilder.CreateIndex(
                name: "IX_rss_sources_url",
                table: "rss_sources",
                column: "url",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "news");

            migrationBuilder.DropTable(
                name: "rss_sources");

            migrationBuilder.UpdateData(
                table: "ai_prompts",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTimeOffset(new DateTime(2026, 7, 19, 14, 55, 20, 94, DateTimeKind.Unspecified).AddTicks(8824), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 19, 14, 55, 20, 94, DateTimeKind.Unspecified).AddTicks(8548), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 19, 14, 55, 20, 94, DateTimeKind.Unspecified).AddTicks(8548), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 19, 14, 55, 20, 94, DateTimeKind.Unspecified).AddTicks(8550), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 19, 14, 55, 20, 94, DateTimeKind.Unspecified).AddTicks(8551), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 19, 14, 55, 20, 94, DateTimeKind.Unspecified).AddTicks(8552), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 19, 14, 55, 20, 94, DateTimeKind.Unspecified).AddTicks(8553), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 19, 14, 55, 20, 94, DateTimeKind.Unspecified).AddTicks(8554), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 19, 14, 55, 20, 94, DateTimeKind.Unspecified).AddTicks(8555), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 19, 14, 55, 20, 94, DateTimeKind.Unspecified).AddTicks(8556), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 19, 14, 55, 20, 94, DateTimeKind.Unspecified).AddTicks(8556), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "bonus_types",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 7, 19, 14, 55, 20, 94, DateTimeKind.Unspecified).AddTicks(8558), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 7, 19, 14, 55, 20, 94, DateTimeKind.Unspecified).AddTicks(8558), new TimeSpan(0, 0, 0, 0, 0)) });
        }
    }
}
