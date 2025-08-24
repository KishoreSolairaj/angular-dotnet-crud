using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeRegistration.Server.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Country_Mst",
                columns: table => new
                {
                    CountryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Country_Mst", x => x.CountryId);
                });

            migrationBuilder.CreateTable(
                name: "State_Mst",
                columns: table => new
                {
                    StateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StateName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_State_Mst", x => x.StateId);
                    table.ForeignKey(
                        name: "FK_State_Mst_Country_Mst_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Country_Mst",
                        principalColumn: "CountryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Employee_Mst",
                columns: table => new
                {
                    EmployeeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    MobileNum = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Pincode = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    DOB = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AddressLine1 = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    AddressLine2 = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    StateId = table.Column<int>(type: "int", nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee_Mst", x => x.EmployeeId);
                    table.ForeignKey(
                        name: "FK_Employee_Mst_Country_Mst_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Country_Mst",
                        principalColumn: "CountryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employee_Mst_State_Mst_StateId",
                        column: x => x.StateId,
                        principalTable: "State_Mst",
                        principalColumn: "StateId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employee_Mst_CountryId",
                table: "Employee_Mst",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_Mst_MobileNum",
                table: "Employee_Mst",
                column: "MobileNum",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employee_Mst_StateId",
                table: "Employee_Mst",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_State_Mst_CountryId",
                table: "State_Mst",
                column: "CountryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employee_Mst");

            migrationBuilder.DropTable(
                name: "State_Mst");

            migrationBuilder.DropTable(
                name: "Country_Mst");
        }
    }
}
