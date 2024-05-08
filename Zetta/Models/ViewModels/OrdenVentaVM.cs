using Microsoft.AspNetCore.Mvc.Rendering;

namespace Zetta.Models.ViewModels
{
    public class OrdenVentaVM
    {
        public OrdenVenta Presupuesto { get; set; }
        public UsuarioAplicacion UsuarioAplicacion { get; set; }
        public List<OrdenVentaDetalle> OrdenVentaDetalle { get; set; }
        public IEnumerable<SelectListItem> ClienteLista { get; set; }
        public IEnumerable<SelectListItem> ProductoCodigoLista { get; set; } // Lista de códigos de productos
        public IEnumerable<SelectListItem> ProductoNombreLista { get; set; } // Lista de nombres de productos
        public DateTime FechaActual { get; set; }
        public int ProductoIdSel { get; set; }// Id del producto seleccionado
        public string ProductoCodigo { get; set; } // Código del producto seleccionado
        public string ProductoNombre { get; set; } // Nombre del producto seleccionado
        public int? ProductoCantidad { get; set; } // Cambiado a int? (nullable)
        public decimal? ProductoPrecio { get; set; }
    }
}
