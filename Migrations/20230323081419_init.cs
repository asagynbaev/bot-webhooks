using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace bot_webhooks.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BinanceFutureAccountInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CanDeposit = table.Column<bool>(type: "boolean", nullable: false),
                    CanTrade = table.Column<bool>(type: "boolean", nullable: false),
                    CanWithdraw = table.Column<bool>(type: "boolean", nullable: false),
                    FeeTier = table.Column<int>(type: "integer", nullable: false),
                    MaxWithdrawAmount = table.Column<double>(type: "double precision", nullable: false),
                    TotalInitialMargin = table.Column<double>(type: "double precision", nullable: false),
                    TotalMaintMargin = table.Column<double>(type: "double precision", nullable: false),
                    TotalMarginBalance = table.Column<double>(type: "double precision", nullable: false),
                    TotalOpenOrderInitialMargin = table.Column<double>(type: "double precision", nullable: false),
                    TotalPositionInitialMargin = table.Column<double>(type: "double precision", nullable: false),
                    TotalUnrealizedProfit = table.Column<double>(type: "double precision", nullable: false),
                    TotalWalletBalance = table.Column<double>(type: "double precision", nullable: false),
                    TotalCrossWalletBalance = table.Column<double>(type: "double precision", nullable: false),
                    TotalCrossUnPnl = table.Column<double>(type: "double precision", nullable: false),
                    AvailableBalance = table.Column<double>(type: "double precision", nullable: false),
                    updateTime = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BinanceFutureAccountInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "signals",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Symbol = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Direction = table.Column<int>(type: "integer", nullable: false),
                    SignalType = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_signals", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "trades",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SignalID = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Symbol = table.Column<string>(type: "text", nullable: true),
                    Direction = table.Column<int>(type: "integer", nullable: false),
                    SignalType = table.Column<string>(type: "text", nullable: true),
                    BoughtPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    BuyQuoteCommission = table.Column<decimal>(type: "numeric", nullable: false),
                    SellPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    SellUSDTCommission = table.Column<decimal>(type: "numeric", nullable: false),
                    PNL = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trades", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FullName = table.Column<string>(type: "text", nullable: true),
                    ApiKey = table.Column<string>(type: "text", nullable: true),
                    Secret = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<int>(type: "integer", nullable: false),
                    Spot = table.Column<int>(type: "integer", nullable: false),
                    Futures = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BinanceFutureAccountInfos");

            migrationBuilder.DropTable(
                name: "signals");

            migrationBuilder.DropTable(
                name: "trades");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
