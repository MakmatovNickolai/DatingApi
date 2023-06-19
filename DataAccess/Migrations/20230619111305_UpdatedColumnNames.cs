using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedColumnNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vote_Profile_TargetId",
                table: "Vote");

            migrationBuilder.RenameColumn(
                name: "TargetId",
                table: "Vote",
                newName: "ReceiverId");

            migrationBuilder.RenameIndex(
                name: "IX_Vote_TargetId",
                table: "Vote",
                newName: "IX_Vote_ReceiverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vote_Profile_ReceiverId",
                table: "Vote",
                column: "ReceiverId",
                principalTable: "Profile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vote_Profile_ReceiverId",
                table: "Vote");

            migrationBuilder.RenameColumn(
                name: "ReceiverId",
                table: "Vote",
                newName: "TargetId");

            migrationBuilder.RenameIndex(
                name: "IX_Vote_ReceiverId",
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
    }
}
