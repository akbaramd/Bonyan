using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BonyanTemplate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_UserStatus_StatusId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "UserStatus");

            migrationBuilder.DropIndex(
                name: "IX_Users_StatusId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
