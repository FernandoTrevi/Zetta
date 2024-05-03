using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zetta.Models
{
    public class Stock
    {
        [Key]
        public int IdMovimiento { get; set; }

        [ForeignKey("ProductoId")]
        public int ProductoId { get; set; }

        public virtual Producto Producto { get; set; }

        public int Cantidad { get; set; }

        public DateTime Fecha { get; set; }

        public string Concepto { get; set; }

        public string? NroComprobante { get; set; }
    }
}
