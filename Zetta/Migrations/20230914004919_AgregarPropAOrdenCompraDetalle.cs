using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zetta.Migrations
{
    public partial class AgregarPropAOrdenCompraDetalle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Codigo",
                table: "OrdenCompraDetalle",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "OrdenCompraDetalle",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Codigo",
                table: "OrdenCompraDetalle");

            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "OrdenCompraDetalle");
        }
    }
}
