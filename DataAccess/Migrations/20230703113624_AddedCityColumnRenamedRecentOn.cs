using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddedCityColumnRenamedRecentOn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RecentAt",
                table: "Profile",
                newName: "RecentOn");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Profile",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Profile");

            migrationBuilder.RenameColumn(
                name: "RecentOn",
                table: "Profile",
                newName: "RecentAt");
        }
    }
}
