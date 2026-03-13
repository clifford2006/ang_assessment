using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ANG_Assessment.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AlertSubscribes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SubscribeEmail = table.Column<string>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    SubscribeDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertSubscribes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeatherRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    TemperatureLow = table.Column<double>(type: "REAL", nullable: false),
                    TemperatureHigh = table.Column<double>(type: "REAL", nullable: false),
                    HumidityLow = table.Column<double>(type: "REAL", nullable: false),
                    HumidityHigh = table.Column<double>(type: "REAL", nullable: false),
                    WindSpeedLow = table.Column<double>(type: "REAL", nullable: false),
                    WindSpeedHigh = table.Column<double>(type: "REAL", nullable: false),
                    WindDirection = table.Column<string>(type: "TEXT", nullable: false),
                    WeatherDesc = table.Column<string>(type: "TEXT", nullable: false),
                    RecordDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastUpdateTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherRecords", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlertSubscribes");

            migrationBuilder.DropTable(
                name: "WeatherRecords");
        }
    }
}
