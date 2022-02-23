using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimmeMqqt.Migrations
{
    public partial class AddBlogCreatedTimestamp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MachineDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MachineID = table.Column<int>(type: "int", nullable: false),
                    MachineName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdealCyclus = table.Column<int>(type: "int", nullable: false),
                    Break = table.Column<bool>(type: "bit", nullable: false),
                    Failure = table.Column<bool>(type: "bit", nullable: false),
                    TotalProduction = table.Column<int>(type: "int", nullable: false),
                    TotalGoodProduction = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MachineDatas", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MachineDatas");
        }
    }
}
