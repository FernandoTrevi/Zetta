using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zetta.Models
{
    public class OrdenCompraDetalle
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrdenCompraId { get; set; }

        [ForeignKey("OrdenCompraId")]
        public OrdenCompra OrdenCompra { get; set; }

        [Required]
        public int ProductoId { get; set; }

        [ForeignKey("ProductoId")]
        public Producto Producto { get; set; }

        [Required]
        public int Cantidad { get; set; }

    }
}
