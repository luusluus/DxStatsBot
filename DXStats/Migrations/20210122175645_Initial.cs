using DXStats.Interfaces;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace DXStats.Migrations
{
    public partial class Initial : Migration
    {
        private readonly ISeeder _seeder;

        public Initial(ISeeder seeder)
        {
            _seeder = seeder;
        }
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Coins",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    SupportedSince = table.Column<DateTime>(nullable: false),
                    NumberOfTrades = table.Column<int>(nullable: false),
                    Volume = table.Column<decimal>(nullable: false),
                    VolumeUSD = table.Column<decimal>(nullable: false),
                    VolumeBTC = table.Column<decimal>(nullable: false),
                    VolumeBLOCK = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Trades",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TradeId = table.Column<string>(nullable: true),
                    Timestamp = table.Column<long>(nullable: false),
                    FeeTxId = table.Column<string>(nullable: true),
                    NodePubKey = table.Column<string>(nullable: true),
                    TakerId = table.Column<string>(nullable: true),
                    TakerSize = table.Column<decimal>(nullable: false),
                    MakerId = table.Column<string>(nullable: true),
                    MakerSize = table.Column<decimal>(nullable: false),
                    PriceUSD = table.Column<decimal>(nullable: false),
                    PriceBTC = table.Column<decimal>(nullable: false),
                    PriceBLOCK = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trades_Coins_MakerId",
                        column: x => x.MakerId,
                        principalTable: "Coins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Trades_Coins_TakerId",
                        column: x => x.TakerId,
                        principalTable: "Coins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trades_MakerId",
                table: "Trades",
                column: "MakerId");

            migrationBuilder.CreateIndex(
                name: "IX_Trades_TakerId",
                table: "Trades",
                column: "TakerId");

            _seeder.Seed(migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Trades");

            migrationBuilder.DropTable(
                name: "Coins");
        }
    }
}
