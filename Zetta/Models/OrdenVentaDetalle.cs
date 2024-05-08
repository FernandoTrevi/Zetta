using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zetta.Models
{
    public class OrdenVentaDetalle
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrdenVentaId { get; set; }

        [ForeignKey("OrdenVentaId")]
        public OrdenVenta OrdenVenta { get; set; }

        [Required]
        public int ProductoId { get; set; }

        [ForeignKey("ProductoId")]
        public Producto Producto { get; set; }

        [Required]
        public string Codigo { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public int Cantidad { get; set; }

        [Required]
        public decimal Precio { get; set; }

        public decimal Iva {  get; set; }

        public decimal Descuento { get; set; }

        public decimal TotalDetalle => (Cantidad * Precio) + Iva - Descuento;
    }
}