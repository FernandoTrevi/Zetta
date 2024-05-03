using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zetta.Migrations
{
    public partial class AgregarFilaAStock : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NroComprobante",
                table: "Stock",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NroComprobante",
                table: "Stock");
        }
    }
}
