using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Zetta.Models;


namespace Zetta.Datos
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Categoria> Categoria { get; set; }
        public DbSet<Marca> Marca { get; set; }
        public DbSet<Producto> Producto { get; set; }
        public DbSet<Stock> Stock { get; set; }
        public DbSet<UsuarioAplicacion> UsuarioAplicacion { get; set; }
        public DbSet<CondIva> CondIva { get; set; }
        public DbSet<Provincia> Provincia { get; set; }
        public DbSet<Proveedor> Proveedor { get; set; }
    }
}
