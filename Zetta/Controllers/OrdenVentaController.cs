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

            };

            return View(ordenVentaVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(OrdenVentaVM ordenVentaVM)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    // 1. Crear una nueva orden de Venta
                    var ordenVenta = new OrdenVenta
                    {
                        NroOrdenVenta = ordenVentaVM.OrdenVenta.NroOrdenVenta,
                        Fecha = ordenVentaVM.FechaActual,
                        ClienteId = ordenVentaVM.OrdenVenta.ClienteId,
                        Vencimiento = ordenVentaVM.OrdenVenta.Vencimiento,
                        CondicionPago = ordenVentaVM.OrdenVenta.CondicionPago,
                        Observaciones = ordenVentaVM.OrdenVenta.Observaciones,
                        Vendedor = ordenVentaVM.OrdenVenta.Vendedor,
                        Subtotal = ordenVentaVM.OrdenVenta.Subtotal / 100,
                        Iva10 = ordenVentaVM.OrdenVenta.Iva10 / 100,
                        Iva21 = ordenVentaVM.OrdenVenta.Iva21 / 100,
                        Estado = ordenVentaVM.OrdenVenta.Estado,
                    };

                    // 2. Agregar la orden de Venta al contexto
                    _context.Add(ordenVenta);
                    await _context.SaveChangesAsync();

                    var IdDeOrdenVenta = ordenVenta.Id;

                    // 3. Agregar detalles de la orden de Venta
                    foreach (OrdenVentaDetalle detalle in ordenVentaVM.OrdenVentaDetalle)
                    {
                       
                            var ordenVentaDetalle = new OrdenVentaDetalle
                            {
                                OrdenVentaId = IdDeOrdenVenta,
                                ProductoId = detalle.ProductoId,
                                Codigo = detalle.Codigo,
                                Nombre = detalle.Nombre,
                                Cantidad = detalle.Cantidad,
                                Precio = detalle.Precio / 100,
                                Alicuota = detalle.Alicuota / 100,
                                Iva = detalle.Iva / 100,
                                Descuento = detalle.Descuento / 100,
                            };

                            _context.Add(ordenVentaDetalle);
                        
                      
                    }

                    // 5. Guardar los cambios en la base de datos
                    await _context.SaveChangesAsync();

                    // 6. Generar una fila en la tabla Stock
                    if (ordenVentaVM.OrdenVenta.Estado == EstadoOrdenVenta.Procesada)
                    {
                        GenerarFilaStock(ordenVentaVM);
                    }

                    TempData["success"] = "Orden de Venta creada exitosamente!";

                    return RedirectToAction("Ver", new { id = IdDeOrdenVenta });

                    //return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Manejar excepciones aquí, realizar rollback si es necesario
                    ModelState.AddModelError(string.Empty, $"Ocurrió un error al procesar la orden de Venta: {ex.Message}");
                }

            }

            // Si llegamos aquí, significa que hubo un error, volvemos a mostrar el formulario con los errores.
            ordenVentaVM.ClienteLista = _context.Cliente
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Nombre
                });
            ordenVentaVM.ProductoCodigoLista = _context.Producto
                .Select(p => new SelectListItem
                {
                    Value = p.Codigo,
                    Text = p.Codigo
                });
            ordenVentaVM.ProductoNombreLista = _context.Producto
                .Select(p => new SelectListItem
                {
                    Value = p.Nombre,
                    Text = p.Nombre
                });

            return View(ordenVentaVM);
        }


        //GET
        public async Task<ActionResult> Ver(int id)
        {
            var ordenVenta = await _context.OrdenVenta
                    .Include(o => o.Cliente)
                    .Include(o => o.OrdenVentaDetalle)
                    .ThenInclude(od => od.Producto)
                    .FirstOrDefaultAsync(o => o.Id == id);
            if (ordenVenta == null)
            {
                return NotFound();
            }

            // Crear un objeto OrdenVentaVM y asignar la orden recuperada
            OrdenVentaVM ordenVentaVM = new()
            {
                OrdenVenta = ordenVenta,
                OrdenVentaDetalle = ordenVenta.OrdenVentaDetalle,
              
            };

            return View(ordenVentaVM);
        }

        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordenVenta = await _context.OrdenVenta
                .Include(o => o.Cliente) 
                .FirstOrDefaultAsync(o => o.Id == id);

            if (ordenVenta == null)
            {
                return NotFound();
            }
           
            return View(ordenVenta);
        }


        // POST: OrdenVenta/Eliminar/{id}
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var ordenVenta = await _context.OrdenVenta.FindAsync(id);
            var ordenVentaId = ordenVenta.NroOrdenVenta;
            if (ordenVenta == null)
            {
                return NotFound();
            }

            // Elimina la orden de Venta
            _context.OrdenVenta.Remove(ordenVenta);

            // Elimina las filas de OrdenVentaDetalle que coincidan con el id
            var ordenVentaDetalles = await _context.OrdenVentaDetalle
                .Where(d => d.OrdenVentaId == ordenVentaId)
                .ToListAsync();

            _context.OrdenVentaDetalle.RemoveRange(ordenVentaDetalles);

            await _context.SaveChangesAsync();
             if (ordenVenta.Estado == EstadoOrdenVenta.Procesada)
            {
                return RedirectToAction(nameof(Index));

            }
            else
            {
                return RedirectToAction(nameof(Index), new RouteValueDictionary { { "presupuesto", true } });

            }



        }
        //////////////////////////////////////////////////////////////////////////////////

        //Generar cambios en stock cuando se hace una venta
        private void GenerarFilaStock(OrdenVentaVM viewModel)
        {
            foreach (var detalle in viewModel.OrdenVentaDetalle)
            {
                // Recuperar el producto correspondiente de la base de datos
                var producto = _context.Producto.FirstOrDefault(p => p.Id == detalle.ProductoId);

                if (producto != null)
                {
                    // Actualizar el stock del producto
                    producto.Stock -= detalle.Cantidad;

                    // Crear y guardar la entrada en la tabla Stock
                    var nuevaEntradaStock = new Stock
                    {
                        ProductoId = detalle.ProductoId,
                        Cantidad = detalle.Cantidad,
                        Fecha = viewModel.FechaActual,
                        Concepto = "Ventas",
                        NroComprobante = "Rto. Nro: " + viewModel.OrdenVenta.NroOrdenVenta.ToString()

                    };

                    _context.Stock.Add(nuevaEntradaStock);
                    _context.SaveChanges();
                }
                else
                {
                    TempData["error"] = "No se encontró el Producto con el Id indicado";
                    continue;

                }
            }
        }

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
        public IActionResult OrdenesCompraCliente(int clienteId)
        {
            var ordenesCompraCliente = _context.OrdenVenta
                                            .Where(o => o.ClienteId == clienteId && o.Estado == EstadoOrdenVenta.Presupuesto)
                                            .ToList();

            return PartialView("_OrdenesCompraCliente", ordenesCompraCliente);
        }
        [HttpPost]
        public IActionResult GetDetallesOrdenes([FromBody] List<int> ordenesIds)
        {
            if (ordenesIds == null || !ordenesIds.Any())
            {
                return BadRequest(new { message = "No se han recibido IDs de órdenes." });
            }

            var detalles = _context.OrdenVentaDetalle
                                   .Where(d => ordenesIds.Contains(d.OrdenVentaId))
                                   .Select(d => new
                                   {
                                       d.Codigo,
                                       d.Nombre,
                                       d.Cantidad,
                                       d.Precio,
                                       d.Descuento,
                                       d.Alicuota,
                                   })
                                   .ToList();

            return Json(detalles);
        }

    }
}
