using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Prezentex.Application.Persistence.Migrations
{
    public partial class AddUserIdToEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Gifts_Users_UserId",
                table: "Gifts");

            migrationBuilder.DropForeignKey(
                name: "FK_Recipients_Users_UserId",
                table: "Recipients");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Recipients",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Gifts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Gifts_Users_UserId",
                table: "Gifts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Recipients_Users_UserId",
                table: "Recipients",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Gifts_Users_UserId",
                table: "Gifts");

            migrationBuilder.DropForeignKey(
                name: "FK_Recipients_Users_UserId",
                table: "Recipients");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Recipients",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Gifts",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Gifts_Users_UserId",
                table: "Gifts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipients_Users_UserId",
                table: "Recipients",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
