using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beddin.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleJoinAndPasswordReset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PasswordResetTokens_UserId",
                table: "PasswordResetTokens");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetTokens_UserId",
                table: "PasswordResetTokens",
                column: "UserId",
                filter: "\"UsedAt\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PasswordResetTokens_UserId",
                table: "PasswordResetTokens");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetTokens_UserId",
                table: "PasswordResetTokens",
                column: "UserId");
        }
    }
}
