using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zetta.Migrations
{
    public partial class AgregarPrecioOCDetalle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Precio",
                table: "OrdenCompraDetalle",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Precio",
                table: "OrdenCompraDetalle");
        }
    }
}
