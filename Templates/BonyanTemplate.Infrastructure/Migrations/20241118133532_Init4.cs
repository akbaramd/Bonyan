using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BonyanTemplate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status_Name",
                table: "Users",
                newName: "StatusName");

            migrationBuilder.RenameColumn(
                name: "Status_Id",
                table: "Users",
                newName: "StatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StatusName",
                table: "Users",
                newName: "Status_Name");

            migrationBuilder.RenameColumn(
                name: "StatusId",
                table: "Users",
                newName: "Status_Id");
        }
    }
}
