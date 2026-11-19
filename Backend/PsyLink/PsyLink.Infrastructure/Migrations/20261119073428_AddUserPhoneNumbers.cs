using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PsyLink.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserPhoneNumbers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "number",
                table: "phone_numbers",
                type: "character(8)",
                maxLength: 8,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(8)",
                oldMaxLength: 8);

            migrationBuilder.DropIndex(
                name: "IX_phone_numbers_number",
                table: "phone_numbers");

            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                table: "phone_numbers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_phone_numbers_user_id",
                table: "phone_numbers",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_phone_numbers_users_user_id",
                table: "phone_numbers",
                column: "user_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "number",
                table: "phone_numbers",
                type: "character varying(8)",
                maxLength: 8,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(8)",
                oldMaxLength: 8);

            migrationBuilder.DropForeignKey(
                name: "FK_phone_numbers_users_user_id",
                table: "phone_numbers");

            migrationBuilder.DropIndex(
                name: "IX_phone_numbers_user_id",
                table: "phone_numbers");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "phone_numbers");

            migrationBuilder.CreateIndex(
                name: "IX_phone_numbers_number",
                table: "phone_numbers",
                column: "number",
                unique: true);
        }
    }
}
