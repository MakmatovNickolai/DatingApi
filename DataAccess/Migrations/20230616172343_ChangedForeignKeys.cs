using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations;

/// <inheritdoc />
public partial class ChangedForeignKeys : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Vote_Profile_ProfileId",
            table: "Vote");

        migrationBuilder.RenameColumn(
            name: "ProfileId",
            table: "Vote",
            newName: "TargetId");

        migrationBuilder.RenameIndex(
            name: "IX_Vote_ProfileId",
            table: "Vote",
            newName: "IX_Vote_TargetId");

        migrationBuilder.AddForeignKey(
            name: "FK_Vote_Profile_TargetId",
            table: "Vote",
            column: "TargetId",
            principalTable: "Profile",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Vote_Profile_TargetId",
            table: "Vote");

        migrationBuilder.RenameColumn(
            name: "TargetId",
            table: "Vote",
            newName: "ProfileId");

        migrationBuilder.RenameIndex(
            name: "IX_Vote_TargetId",
            table: "Vote",
            newName: "IX_Vote_ProfileId");

        migrationBuilder.AddForeignKey(
            name: "FK_Vote_Profile_ProfileId",
            table: "Vote",
            column: "ProfileId",
            principalTable: "Profile",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }
}
