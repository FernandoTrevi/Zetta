using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zetta.Migrations
{
    public partial class AgregarStockMinimoAProductos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AlertaStock",
                table: "Producto",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Publicado",
                table: "Producto",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "StockMinimo",
                table: "Producto",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlertaStock",
                table: "Producto");

            migrationBuilder.DropColumn(
                name: "Publicado",
                table: "Producto");

            migrationBuilder.DropColumn(
                name: "StockMinimo",
                table: "Producto");
        }
    }
}
