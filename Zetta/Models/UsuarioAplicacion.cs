using Microsoft.AspNetCore.Identity;


namespace Zetta.Models
{
    public class UsuarioAplicacion : IdentityUser
    {
        public string NombreCompleto { get; set; }
    }
}
