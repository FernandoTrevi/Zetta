using System.ComponentModel.DataAnnotations;

namespace Zetta.Models
{
    public class CondIva
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }    
    }
}
