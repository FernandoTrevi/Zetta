using Microsoft.AspNetCore.Mvc.Rendering;

namespace Zetta.Models.ViewModels
{
    public class ProveedorVM
    {
        public Proveedor Proveedor { get; set; }
        public IEnumerable<SelectListItem> ProvinciaLista { get; set; }
        public IEnumerable<SelectListItem> CondIvaLista { get; set; }
    }
}
