namespace Zetta.Models.ViewModels
{
    public class ProductoUsuarioVM
    {
        public ProductoUsuarioVM()
        {
                ProductoLista = new List<Producto>();
        }

        public UsuarioAplicacion UsuarioAplicacion { get; set; }

        public IEnumerable<Producto> ProductoLista { get; set; }
    }
}
