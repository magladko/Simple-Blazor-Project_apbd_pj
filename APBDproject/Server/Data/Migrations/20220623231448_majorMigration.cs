using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace APBDproject.Server.Data.Migrations
{
    public partial class majorMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublishedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArticleUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Daily",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    From = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Open = table.Column<double>(type: "float", nullable: false),
                    High = table.Column<double>(type: "float", nullable: false),
                    Low = table.Column<double>(type: "float", nullable: false),
                    Close = table.Column<double>(type: "float", nullable: false),
                    Volume = table.Column<double>(type: "float", nullable: false),
                    AfterHours = table.Column<double>(type: "float", nullable: false),
                    PreMarket = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Daily", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Daily_Companies_Symbol",
                        column: x => x.Symbol,
                        principalTable: "Companies",
                        principalColumn: "Symbol",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArticleCompany",
                columns: table => new
                {
                    ArticlesId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompaniesSymbol = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleCompany", x => new { x.ArticlesId, x.CompaniesSymbol });
                    table.ForeignKey(
                        name: "FK_ArticleCompany_Articles_ArticlesId",
                        column: x => x.ArticlesId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArticleCompany_Companies_CompaniesSymbol",
                        column: x => x.CompaniesSymbol,
                        principalTable: "Companies",
                        principalColumn: "Symbol",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleCompany_CompaniesSymbol",
                table: "ArticleCompany",
                column: "CompaniesSymbol");

            migrationBuilder.CreateIndex(
                name: "IX_Daily_Symbol",
                table: "Daily",
                column: "Symbol",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleCompany");

            migrationBuilder.DropTable(
                name: "Daily");

            migrationBuilder.DropTable(
                name: "Articles");
        }
    }
}
