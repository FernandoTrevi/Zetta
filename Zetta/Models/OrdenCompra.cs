using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zetta.Models
{
    public class OrdenCompra
    {
        [Key]
        public int Id { get; set; }

        public int NroOrden { get; set; } // Se autoincrementará en la base de datos
        public DateTime Fecha { get; set; }

        // Relación con el proveedor
        [Required(ErrorMessage = "El Proveedor es Obligatorio.")]
        public int ProveedorId { get; set; }

        [ForeignKey("ProveedorId")]
        public virtual Proveedor Proveedor { get; set; }
       
        public string Observaciones { get; set; }

        [Required(ErrorMessage = "El Estado es obligatorio.")]
        public EstadoOrden Estado { get; set; }

        public virtual List<OrdenCompraDetalle> OrdenCompraDetalle { get; set; }

    }

    public enum EstadoOrden
    {
        Pendiente,
        Procesada
    }
}

// Validación de Estados: Con el uso de enumeradores, puedes aprovechar mejor las capacidades de validación.
// Por ejemplo, si deseas    asegurarte de que solo se puedan establecer los valores "Pendiente" o "Procesada"
// en el campo "Estado", puedes usar una anotación de validación personalizada. Esto evitará que se introduzcan valores no válidos.

//public class EstadoValidoAttribute : ValidationAttribute
//{
//    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
//    {
//        if (value is EstadoOrden estado && (estado == EstadoOrden.Pendiente || estado == EstadoOrden.Procesada))
//        {
//            return ValidationResult.Success;
//        }

//        return new ValidationResult("El estado proporcionado no es válido.");
//    }
//}
