using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zetta.Models
{
    public class OrdenVenta
    {
        [Key]
        public int Id { get; set; }

        public int NroOrdenVenta { get; set; }

        public DateTime Fecha { get; set; }

        public DateTime Vencimiento { get; set; }


        public string CondicionPago { get; set; }

        // Relación con el Cliente
        [Required(ErrorMessage = "El Cliente es Obligatorio.")]
        public int ClienteId { get; set; }

        [ForeignKey("ClienteId")]
        public virtual Cliente Cliente { get; set; }

        public string Vendedor { get; set; }

        public string Observaciones { get; set; }

        [Required(ErrorMessage = "El Estado es obligatorio.")]
        public EstadoOrdenVenta Estado { get; set; }

        public virtual List<OrdenVentaDetalle> OrdenVentaDetalle { get; set; }

        public decimal Subtotal { get; set; }

        public decimal Iva10 { get; set; }

        public decimal Iva21 { get; set; }

        public decimal TotalOrden => Subtotal + Iva10 + Iva21;
    }
    public enum EstadoOrdenVenta
    {
        Presupuesto,
        Vencida,
        Procesada
    }
}
