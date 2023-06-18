using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataAccess.Migrations;

/// <inheritdoc />
public partial class Initial : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Conversation",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Conversation", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Gender",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Gender", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "User",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Email = table.Column<string>(type: "text", nullable: false),
                PasswordHash = table.Column<string>(type: "text", nullable: true),
                IsConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                IsGoogleUser = table.Column<bool>(type: "boolean", nullable: false),
                IsFacebookUser = table.Column<bool>(type: "boolean", nullable: false),
                IsAppleUser = table.Column<bool>(type: "boolean", nullable: false),
                RefreshToken = table.Column<string>(type: "text", nullable: true),
                RefreshTokenExpiryTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                AppVersion = table.Column<string>(type: "text", nullable: false),
                Platform = table.Column<string>(type: "text", nullable: false),
                DeviceInfo = table.Column<string>(type: "text", nullable: false),
                DeviceUniqueId = table.Column<string>(type: "text", nullable: false),
                DeviceLocale = table.Column<string>(type: "text", nullable: false),
                DeviceGmtOffset = table.Column<string>(type: "text", nullable: false),
                IsRootedDevice = table.Column<bool>(type: "boolean", nullable: false),
                IsEmulator = table.Column<bool>(type: "boolean", nullable: false),
                GoogleId = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_User", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Profile",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Name = table.Column<string>(type: "text", nullable: false),
                GenderId = table.Column<Guid>(type: "uuid", nullable: false),
                BirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                Description = table.Column<string>(type: "text", nullable: false),
                Longitude = table.Column<double>(type: "double precision", nullable: false),
                Latitude = table.Column<double>(type: "double precision", nullable: false),
                RecentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                FilterDistance = table.Column<int>(type: "integer", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Profile", x => x.Id);
                table.ForeignKey(
                    name: "FK_Profile_Gender_GenderId",
                    column: x => x.GenderId,
                    principalTable: "Gender",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Profile_User_UserId",
                    column: x => x.UserId,
                    principalTable: "User",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "VerificationCode",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Type = table.Column<int>(type: "integer", nullable: false),
                Token = table.Column<string>(type: "text", nullable: false),
                ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_VerificationCode", x => x.Id);
                table.ForeignKey(
                    name: "FK_VerificationCode_User_UserId",
                    column: x => x.UserId,
                    principalTable: "User",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Image",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                ContentType = table.Column<string>(type: "text", nullable: false),
                FileData = table.Column<byte[]>(type: "bytea", nullable: false),
                ProfileId = table.Column<Guid>(type: "uuid", nullable: true),
                CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Image", x => x.Id);
                table.ForeignKey(
                    name: "FK_Image_Profile_ProfileId",
                    column: x => x.ProfileId,
                    principalTable: "Profile",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "Message",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Text = table.Column<string>(type: "text", nullable: false),
                SenderId = table.Column<Guid>(type: "uuid", nullable: false),
                ConversationId = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Message", x => x.Id);
                table.ForeignKey(
                    name: "FK_Message_Conversation_ConversationId",
                    column: x => x.ConversationId,
                    principalTable: "Conversation",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Message_Profile_SenderId",
                    column: x => x.SenderId,
                    principalTable: "Profile",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Participant",
            columns: table => new
            {
                ConversationId = table.Column<Guid>(type: "uuid", nullable: false),
                ProfileId = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Participant", x => new { x.ConversationId, x.ProfileId });
                table.ForeignKey(
                    name: "FK_Participant_Conversation_ConversationId",
                    column: x => x.ConversationId,
                    principalTable: "Conversation",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Participant_Profile_ProfileId",
                    column: x => x.ProfileId,
                    principalTable: "Profile",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Vote",
            columns: table => new
            {
                VoterId = table.Column<Guid>(type: "uuid", nullable: false),
                ProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                IsLiked = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Vote", x => new { x.VoterId, x.ProfileId });
                table.ForeignKey(
                    name: "FK_Vote_Profile_ProfileId",
                    column: x => x.ProfileId,
                    principalTable: "Profile",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Vote_Profile_VoterId",
                    column: x => x.VoterId,
                    principalTable: "Profile",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Image_ProfileId",
            table: "Image",
            column: "ProfileId");

        migrationBuilder.CreateIndex(
            name: "IX_Message_ConversationId",
            table: "Message",
            column: "ConversationId");

        migrationBuilder.CreateIndex(
            name: "IX_Message_SenderId",
            table: "Message",
            column: "SenderId");

        migrationBuilder.CreateIndex(
            name: "IX_Participant_ProfileId",
            table: "Participant",
            column: "ProfileId");

        migrationBuilder.CreateIndex(
            name: "IX_Profile_GenderId",
            table: "Profile",
            column: "GenderId");

        migrationBuilder.CreateIndex(
            name: "IX_Profile_UserId",
            table: "Profile",
            column: "UserId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_User_Email",
            table: "User",
            column: "Email",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_VerificationCode_UserId",
            table: "VerificationCode",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_Vote_ProfileId",
            table: "Vote",
            column: "ProfileId");

        migrationBuilder.Sql($"""
            INSERT INTO "Gender" ("Id", "Name")
                values
                ('{Guid.NewGuid()}', 'Male'),
                ('{Guid.NewGuid()}', 'Female')
            """);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Image");

        migrationBuilder.DropTable(
            name: "Message");

        migrationBuilder.DropTable(
            name: "Participant");

        migrationBuilder.DropTable(
            name: "VerificationCode");

        migrationBuilder.DropTable(
            name: "Vote");

        migrationBuilder.DropTable(
            name: "Conversation");

        migrationBuilder.DropTable(
            name: "Profile");

        migrationBuilder.DropTable(
            name: "Gender");

        migrationBuilder.DropTable(
            name: "User");
    }
}
