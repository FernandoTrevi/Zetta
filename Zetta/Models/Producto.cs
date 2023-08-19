using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Zetta.Models
{
    public class Producto
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El Código de Producto es Obligatorio.")]
        public string Codigo { get; set; }

        [Required(ErrorMessage = "El Nombre de Producto es Obligatorio.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La Marca de Producto es Obligatorio.")]
        public int MarcaId { get; set; }

        [ForeignKey("MarcaId")]
        public virtual Marca? Marca { get; set; }

        [Required(ErrorMessage = "La Categoría de Producto es Obligatorio.")]
        public int CategoriaId { get; set; }

        [ForeignKey("CategoriaId")]
        public virtual Categoria? Categoria { get; set; }

        [Required(ErrorMessage = "La Descripción de Producto es Obligatorio.")]
        public string Descripcion { get; set; }

        public string? ImagenUrl { get; set; }

        [Required(ErrorMessage = "El Costo de Producto es Obligatorio.")]
        //[Range(1, double.MaxValue, ErrorMessage = "El Costo debe ser Mayor a Cero")]
        public double Costo { get; set; }

        public double Margen { get; set; }

        [DataType(DataType.Currency)]
        public double Precio { get; set; }

        public int Stock { get; set; }

        public int StockMinimo { get; set; }

        public bool Publicado { get; set; }

    }
}
