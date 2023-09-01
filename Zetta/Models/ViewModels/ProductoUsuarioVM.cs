namespace Zetta.Models.ViewModels
{
    public class ProductoUsuarioVM
    {
        public ProductoUsuarioVM()
        {
                ProductoLista = new List<Producto>();
        }

        public UsuarioAplicacion UsuarioAplicacion { get; set; }

        public IList<Producto> ProductoLista { get; set; }
    }
}
