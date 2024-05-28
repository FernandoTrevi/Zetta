using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Zetta.Models.ViewModels;

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

        public decimal Alicuota { get; set; }

        public decimal Iva { get; set; }

        public decimal Descuento { get; set; }

        public decimal PrecioConIva => Math.Round(Precio + (Precio * (Alicuota / 100)), 2);

        public decimal Subtotal => Math.Round(Cantidad * Precio * (1 - (Descuento / 100)), 2);

        public decimal TotalDetalle => Math.Round(Subtotal + (Subtotal * (Alicuota / 100)),2);

        public decimal TotalDetalleCF => Math.Round(Cantidad * PrecioConIva * (1 - (Descuento / 100)), 2);



    }
}