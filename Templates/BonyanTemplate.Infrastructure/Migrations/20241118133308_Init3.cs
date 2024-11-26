using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BonyanTemplate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_UserStatus_StatusId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "UserStatus");

            migrationBuilder.DropIndex(
                name: "IX_Users_StatusId",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "StatusId",
                table: "Users",
                newName: "Status_Id");

            migrationBuilder.AddColumn<string>(
                name: "Status_Name",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status_Name",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Status_Id",
                table: "Users",
                newName: "StatusId");

            migrationBuilder.CreateTable(
                name: "UserStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStatus", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_StatusId",
                table: "Users",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_UserStatus_StatusId",
                table: "Users",
                column: "StatusId",
                principalTable: "UserStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
