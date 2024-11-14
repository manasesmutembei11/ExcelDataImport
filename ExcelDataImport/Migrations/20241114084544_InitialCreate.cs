using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExcelDataImport.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Claims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateFiled = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateReceived = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CourtStation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rank = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CaseNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Year = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Plaintiff = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Defendant = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThirdPartyAdvocate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InjuryType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LossDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InsuredMV = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClaimNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Region = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OurAdvocate = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Claims", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Claims");
        }
    }
}
