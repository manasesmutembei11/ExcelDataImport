using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExcelDataImport.Migrations
{
    /// <inheritdoc />
    public partial class cargotype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CargoTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Chapter = table.Column<double>(type: "float", nullable: false),
                    ChapterName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Heading = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HeadingName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HSCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CargoTypes", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CargoTypes");
        }
    }
}
