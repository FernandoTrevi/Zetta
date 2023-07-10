using System.ComponentModel.DataAnnotations;

namespace Zetta.Models
{
    public class Marca
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nombre de Marca es Obligatorio.")]
        public string? Nombre { get; set; }
    }
}
