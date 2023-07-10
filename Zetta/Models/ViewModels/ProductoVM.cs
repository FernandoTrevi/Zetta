using Microsoft.AspNetCore.Mvc.Rendering;

namespace Zetta.Models.ViewModels
{
    public class ProductoVM
    {
        public Producto Producto { get; set; }
        public IEnumerable<SelectListItem>? MarcaLista { get; set; }
        public IEnumerable<SelectListItem>? CategoriaLista { get; set; }
        public bool Publicado { get; set; }
        public bool AlertaStock { get; set; }
        public int StockMinimo { get; set; }
    }
}
