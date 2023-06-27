using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class MovedSearchFilterColumnsToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilterAgeMax",
                table: "Profile");

            migrationBuilder.DropColumn(
                name: "FilterAgeMin",
                table: "Profile");

            migrationBuilder.DropColumn(
                name: "FilterDistance",
                table: "Profile");

            migrationBuilder.AddColumn<int>(
                name: "FilterAgeMax",
                table: "User",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FilterAgeMin",
                table: "User",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FilterDistance",
                table: "User",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilterAgeMax",
                table: "User");

            migrationBuilder.DropColumn(
                name: "FilterAgeMin",
                table: "User");

            migrationBuilder.DropColumn(
                name: "FilterDistance",
                table: "User");

            migrationBuilder.AddColumn<int>(
                name: "FilterAgeMax",
                table: "Profile",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FilterAgeMin",
                table: "Profile",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FilterDistance",
                table: "Profile",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
