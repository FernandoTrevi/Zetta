using Microsoft.AspNetCore.Mvc.Rendering;

namespace Zetta.Models.ViewModels
{
    public class ProductoVM
    {
        public Producto Producto { get; set; }
        public IEnumerable<SelectListItem>? MarcaLista { get; set; }
        public IEnumerable<SelectListItem>? CategoriaLista { get; set; }
       
    }
}
