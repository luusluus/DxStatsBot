using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DXStats.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Coins",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Snapshots",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DateCreated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Snapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DayCompletedOrders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CoinId = table.Column<string>(nullable: true),
                    SnapshotId = table.Column<int>(nullable: true),
                    Count = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DayCompletedOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DayCompletedOrders_Coins_CoinId",
                        column: x => x.CoinId,
                        principalTable: "Coins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DayCompletedOrders_Snapshots_SnapshotId",
                        column: x => x.SnapshotId,
                        principalTable: "Snapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DayVolumes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CoinId = table.Column<string>(nullable: true),
                    SnapshotId = table.Column<int>(nullable: true),
                    NumberOfTrades = table.Column<int>(nullable: false),
                    USD = table.Column<decimal>(nullable: false),
                    BTC = table.Column<decimal>(nullable: false),
                    BLOCK = table.Column<decimal>(nullable: false),
                    CustomCoin = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DayVolumes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DayVolumes_Coins_CoinId",
                        column: x => x.CoinId,
                        principalTable: "Coins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DayVolumes_Snapshots_SnapshotId",
                        column: x => x.SnapshotId,
                        principalTable: "Snapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DayCompletedOrders_CoinId",
                table: "DayCompletedOrders",
                column: "CoinId");

            migrationBuilder.CreateIndex(
                name: "IX_DayCompletedOrders_SnapshotId",
                table: "DayCompletedOrders",
                column: "SnapshotId");

            migrationBuilder.CreateIndex(
                name: "IX_DayVolumes_CoinId",
                table: "DayVolumes",
                column: "CoinId");

            migrationBuilder.CreateIndex(
                name: "IX_DayVolumes_SnapshotId",
                table: "DayVolumes",
                column: "SnapshotId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DayCompletedOrders");

            migrationBuilder.DropTable(
                name: "DayVolumes");

            migrationBuilder.DropTable(
                name: "Coins");

            migrationBuilder.DropTable(
                name: "Snapshots");
        }
    }
}
