using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Zetta.Models;
using Zetta.Models.ViewModels;


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
        public DbSet<OrdenCompra> OrdenCompra { get; set; }
        public DbSet<OrdenCompraDetalle> OrdenCompraDetalle { get; set; }
        public DbSet<Cliente> Cliente { get; set; }
        public DbSet<OrdenVenta> OrdenVenta { get; set; }
        public DbSet<OrdenVentaDetalle> OrdenVentaDetalle { get; set; }


    }
}
