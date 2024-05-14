using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Zetta.Datos;
using Zetta.Models;
using Zetta.Models.ViewModels;

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
        public async Task<IActionResult> Index(string buscar, string ordenActual, int? numpag, string filtroActual, bool presupuesto)
        {
            IQueryable<OrdenVenta> query = _context.OrdenVenta.Include(o => o.Cliente);

            if (buscar != null)
                numpag = 1;
            else
                buscar = filtroActual;

            ViewData["OrdenActual"] = ordenActual;
            ViewData["FiltroActual"] = buscar;
            ViewData["Presupuesto"] = presupuesto;

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

            if (presupuesto)
            {
                query = query.Where(o => o.Estado == EstadoOrdenVenta.Presupuesto || o.Estado == EstadoOrdenVenta.Vencida);
            }
            else
            {
                query = query.Where(o => o.Estado == EstadoOrdenVenta.Procesada);
            }

            int cantidadregistros = 5; // Cambia esta cantidad según tus preferencias
            var paginacion = await Paginacion<OrdenVenta>.CrearPaginacion(query, numpag ?? 1, cantidadregistros);

            return View(paginacion);
        }

        public IActionResult Crear(bool presupuesto)
        {
            ViewData["Presupuesto"] = presupuesto;

            // Llama a la función para obtener el último número de orden
            var ultimoNumeroOrden = ObtenerUltimoNumeroOrden();

            var ordenVentaVM = new OrdenVentaVM
            {

                ClienteLista = _context.Cliente
                    .Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.Nombre
                    }),
                // Seleccionar todos los productos disponibles
                Productos = _context.Producto.ToList(),

                ProductoCodigoLista = _context.Producto
                    .Select(p => new SelectListItem
                    {
                        Value = p.Codigo,
                        Text = p.Codigo
                    }),
                ProductoNombreLista = _context.Producto
                    .Select(p => new SelectListItem
                    {
                        Value = p.Nombre,
                        Text = p.Nombre
                    }),
               
                FechaActual = DateTime.Today,
                // Asigna el último número de orden obtenido
                OrdenVenta = new OrdenVenta { NroOrdenVenta = ultimoNumeroOrden },
                // Inicializa la lista de detalles como una lista vacía
                OrdenVentaDetalle = new List<OrdenVentaDetalle>(),

                UsuarioAplicacion = new UsuarioAplicacion { }
            };

            return View(ordenVentaVM);
        }


        //////////////////////////////////////////////////////////////////////////////////

        // Función para obtener el último número de orden
        private int ObtenerUltimoNumeroOrden()
        {
            var ultimoOrden = _context.OrdenVenta
                .OrderByDescending(o => o.NroOrdenVenta)
                .FirstOrDefault();

            if (ultimoOrden != null)
            {
                return ultimoOrden.NroOrdenVenta + 1;
            }

            // Si no hay órdenes en la base de datos, inicia en 1
            return 1;
        }
    }
}
