using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Prezentex.Api.Migrations
{
    public partial class addRecipientstoGift : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Gifts_Recipients_RecipientId",
                table: "Gifts");

            migrationBuilder.DropIndex(
                name: "IX_Gifts_RecipientId",
                table: "Gifts");

            migrationBuilder.DropColumn(
                name: "RecipientId",
                table: "Gifts");

            migrationBuilder.AddColumn<Guid>(
                name: "GiftId",
                table: "Recipients",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recipients_GiftId",
                table: "Recipients",
                column: "GiftId");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipients_Gifts_GiftId",
                table: "Recipients",
                column: "GiftId",
                principalTable: "Gifts",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipients_Gifts_GiftId",
                table: "Recipients");

            migrationBuilder.DropIndex(
                name: "IX_Recipients_GiftId",
                table: "Recipients");

            migrationBuilder.DropColumn(
                name: "GiftId",
                table: "Recipients");

            migrationBuilder.AddColumn<Guid>(
                name: "RecipientId",
                table: "Gifts",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Gifts_RecipientId",
                table: "Gifts",
                column: "RecipientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Gifts_Recipients_RecipientId",
                table: "Gifts",
                column: "RecipientId",
                principalTable: "Recipients",
                principalColumn: "Id");
        }
    }
}
