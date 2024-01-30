using Microsoft.AspNetCore.Mvc.Rendering;

namespace Zetta.Models.ViewModels
{
    public class OrdenCompraVM
    {
        public OrdenCompra OrdenCompra { get; set; }
        public List<OrdenCompraDetalle> OrdenCompraDetalle { get; set; }
        public IEnumerable<SelectListItem> ProveedorLista { get; set; }
        public IEnumerable<SelectListItem> ProductoCodigoLista { get; set; } // Lista de códigos de productos
        public IEnumerable<SelectListItem> ProductoNombreLista { get; set; } // Lista de nombres de productos
        public DateTime FechaActual { get; set; }
        public int ProductoIdSel { get; set; }// Id del producto seleccionado
        public string ProductoCodigo { get; set; } // Código del producto seleccionado
        public string ProductoNombre { get; set; } // Nombre del producto seleccionado
        public int? ProductoCantidad { get; set; } // Cambiado a int? (nullable)

    }

    public class OrdenDetalle
    {
        public int Cantidad { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public int ProductoId { get; set; }

    }
}
