using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Zetta.Models.ViewModels
{
    public class CompraVM
    {
        public DateTime FechaFactura { get; set; }

        public int ProveedorId { get; set; } 

        public string NombreProveedor { get; set; } 

        public IEnumerable<SelectListItem> ProveedorLista { get; set; }

        public List<Stock> MovimientoStock { get; set; }

        public List<OrdenCompra> OrdenCompra { get; set; }

        [Required]
        public string NumeroFactura { get; set; }

        public List<DetalleFactura> DetallesFactura { get; set; } // Nueva propiedad para los detalles de la factura
    }

    public class DetalleFactura
    {
        public int ProductoId { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal Total { get; set; }
        public int OrdenCompraId { get; set; } 

    }

}
