using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zetta.Models
{
    public class Proveedor
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "La Razón Social es Obligatoria.")]
        public string? Razonsocial { get; set; }

        public string? Contacto { get; set; }

        [Required(ErrorMessage = "La Dirección es Obligatoria.")]
        public string? Direccion { get; set; }

        [Required(ErrorMessage = "La Localidad es Obligatoria.")]
        public string? Localidad { get; set; }

        [Required(ErrorMessage = "El Provincia es Obligatorio.")]
        public int ProvinciaId { get; set; }

        [ForeignKey("ProvinciaId")] 
        public virtual Provincia? Provincia { get; set; }

        [Required(ErrorMessage = "El Teléfono es Obligatorio.")]
        public string? Telefono { get; set; }

        [Required(ErrorMessage = "El Email es Obligatorio.")]
        public string? Email { get; set; }

        public string? Website { get; set; }

        [Required(ErrorMessage = "La elección de una condición es Obligatorio.")]
        public int CondIvaId { get; set; }

        [ForeignKey("CondIvaId")]
        public virtual CondIva? CondIva { get; set; }

        [Required(ErrorMessage = "El Clave Tributaria es Obligatoria.")]
        public string? Cuit { get; }

    }
}
