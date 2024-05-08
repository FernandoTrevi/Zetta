using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zetta.Datos;
using Zetta.Models;

namespace Zetta.Controllers
{
    public class OrdenVentaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdenVentaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: OrdenVenta/Index
        public async Task<IActionResult> Index(string buscar, string ordenActual, int? numpag, string filtroActual)
        {
            IQueryable<OrdenVenta> query = _context.OrdenVenta.Include(o => o.Cliente);

            if (buscar != null)
                numpag = 1;
            else
                buscar = filtroActual;

            ViewData["OrdenActual"] = ordenActual;
            ViewData["FiltroActual"] = buscar;

            if (!string.IsNullOrEmpty(buscar))
            {
                query = query.Where(o => o.NroOrdenVenta.ToString().Contains(buscar) || o.Cliente.Nombre.Contains(buscar));
            }

            if (ordenActual == "nroorden")
            {
                query = query.OrderByDescending(o => o.NroOrdenVenta);
            }
            else
            {
                query = query.OrderByDescending(o => o.Id);
            }


            int cantidadregistros = 5; // Cambia esta cantidad según tus preferencias
            var paginacion = await Paginacion<OrdenVenta>.CrearPaginacion(query, numpag ?? 1, cantidadregistros);

            return View(paginacion);
        }
    }
}
