using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chatbot_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddedResetPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ForgotPasswordToken",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ForgotPasswordTokenExpireTime",
                table: "Users",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ForgotPasswordToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ForgotPasswordTokenExpireTime",
                table: "Users");
        }
    }
}
