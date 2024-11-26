using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BonyanTemplate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Expiration",
                table: "UserTokens",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Expiration",
                table: "UserTokens");
        }
    }
}
