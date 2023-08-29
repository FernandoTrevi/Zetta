using System.ComponentModel.DataAnnotations;

namespace Zetta.Models
{
    public class Categoria
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nombre de Categoria es Obligatorio.")]
        public string Nombre { get; set; }
    }
}
