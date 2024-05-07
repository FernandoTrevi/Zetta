using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Zetta.Models
{
    public class Cliente
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El Nombre y apellido es Obligatorio.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La Dirección es Obligatoria.")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "La Localidad es Obligatoria.")]
        public string Localidad { get; set; }

        [Required(ErrorMessage = "El Provincia es Obligatorio.")]
        public int ProvinciaId { get; set; }

        [ForeignKey("ProvinciaId")]
        public virtual Provincia Provincia { get; set; }

        [Required(ErrorMessage = "El Teléfono es Obligatorio.")]
        public string Telefono { get; set; }

        public string Email { get; set; }

        [Required(ErrorMessage = "La elección de una condición es Obligatorio.")]
        public int CondIvaId { get; set; }

        [ForeignKey("CondIvaId")]
        public virtual CondIva CondIva { get; set; }

        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Solo se permiten números sin espacios en blanco.")]
        public string Dni { get; set; }
    }
}
