using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Zetta.Models.ViewModels
{
    public class CompraVM
    {
        public DateTime FechaFactura { get; set; }

        public int ProveedorId { get; set; } // Agregamos la propiedad ProveedorId

        public string NombreProveedor { get; set; } // Agregamos la propiedad NombreProveedor

        public IEnumerable<SelectListItem> ProveedorLista { get; set; }

        public List<Stock> MovimientoStock { get; set; }

        public List<OrdenCompra> OrdenCompra { get; set; }

        [Required]
        public string NumeroFactura { get; set; }

    }

}
