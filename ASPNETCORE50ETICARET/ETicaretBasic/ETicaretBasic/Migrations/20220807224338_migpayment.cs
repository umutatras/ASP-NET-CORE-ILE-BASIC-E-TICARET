using Microsoft.EntityFrameworkCore.Migrations;

namespace ETicaretBasic.Migrations
{
    public partial class migpayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CartName",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CartNumber",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Cvc",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExpMonth",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExpYear",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CartName",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "CartNumber",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "Cvc",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "ExpMonth",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "ExpYear",
                table: "OrderHeaders");
        }
    }
}
