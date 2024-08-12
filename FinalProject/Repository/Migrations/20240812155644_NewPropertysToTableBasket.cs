using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    public partial class NewPropertysToTableBasket : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Baskets");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "BasketProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "BasketProducts");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Baskets",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
