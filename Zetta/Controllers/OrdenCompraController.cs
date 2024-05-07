using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Zetta.Datos;
using Zetta.Models;
using Zetta.Models.ViewModels;
using Zetta.Utilidades;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Zetta.Controllers
{
    public class OrdenCompraController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConverter _converter;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;

        public OrdenCompraController(ApplicationDbContext context, IConverter converter, IWebHostEnvironment webHostEnvironment, IEmailSender emailSender)
        {
            _context = context;
            _converter = converter;
            _emailSender = emailSender;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: OrdenCompra/Index
        public async Task<IActionResult> Index(string buscar, string ordenActual, int? numpag, string filtroActual)
        {
            IQueryable<OrdenCompra> query = _context.OrdenCompra.Include(o => o.Proveedor);

            if (buscar != null)
                numpag = 1;
            else
                buscar = filtroActual;

            ViewData["OrdenActual"] = ordenActual;
            ViewData["FiltroActual"] = buscar;

            if (!string.IsNullOrEmpty(buscar))
            {
                query = query.Where(o => o.NroOrden.ToString().Contains(buscar) || o.Proveedor.Razonsocial.Contains(buscar));
            }

            if (ordenActual == "nroorden")
            {
                query = query.OrderByDescending(o => o.NroOrden);
            }
            else
            {
                query = query.OrderByDescending(o => o.Id); 
            }


            int cantidadregistros = 5; // Cambia esta cantidad según tus preferencias
            var paginacion = await Paginacion<OrdenCompra>.CrearPaginacion(query, numpag ?? 1, cantidadregistros);

            return View(paginacion);
        }

        // GET: OrdenCompra/Crear
        public IActionResult Crear()
        {
            // Llama a la función para obtener el último número de orden
            var ultimoNumeroOrden = ObtenerUltimoNumeroOrden();

            var ordenCompraVM = new OrdenCompraVM
            {

                ProveedorLista = _context.Proveedor
                    .Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.Razonsocial
                    }),

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
                OrdenCompra = new OrdenCompra { NroOrden = ultimoNumeroOrden },
                // Inicializa la lista de detalles como una lista vacía
                OrdenCompraDetalle = new List<OrdenCompraDetalle>()
            };

            return View(ordenCompraVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(OrdenCompraVM ordenCompraVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // 1. Crear una nueva orden de compra
                    var ordenCompra = new OrdenCompra
                    {
                        NroOrden = ordenCompraVM.OrdenCompra.NroOrden,
                        Fecha = ordenCompraVM.FechaActual,
                        ProveedorId = ordenCompraVM.OrdenCompra.ProveedorId,
                        Observaciones = ordenCompraVM.OrdenCompra.Observaciones,
                        Estado = ordenCompraVM.OrdenCompra.Estado
                    };

                    // 2. Agregar la orden de compra al contexto
                    _context.Add(ordenCompra);
                    await _context.SaveChangesAsync();

                    var IdDeOrdenCompra = ordenCompra.Id;

                    // 3. Agregar detalles de la orden de compra
                    foreach (OrdenCompraDetalle detalle in ordenCompraVM.OrdenCompraDetalle)
                    {
                        string codigoSeleccionado = detalle.Codigo;
                        var productoIdSeleccionado = ObtenerProductoIdPorCodigo(codigoSeleccionado);

                        if (productoIdSeleccionado.HasValue)
                        {
                            var ordenCompraDetalle = new OrdenCompraDetalle
                            {
                                OrdenCompraId = IdDeOrdenCompra,
                                ProductoId = productoIdSeleccionado.Value,
                                Codigo = detalle.Codigo,
                                Nombre = detalle.Nombre,
                                Cantidad = detalle.Cantidad,
                                Precio = detalle.Precio
                            };

                            _context.Add(ordenCompraDetalle);
                        }
                        else
                        {
                            // Manejar el caso en el que no se encontró el ProductoId
                            // Puedes mostrar un mensaje de error o tomar una acción adecuada
                        }
                    }

                    // 4. Actualizar el último número de orden en la base de datos
                    ActualizarUltimoNumeroOrden(ordenCompraVM.OrdenCompra.NroOrden);

                    // 5. Guardar los cambios en la base de datos
                    await _context.SaveChangesAsync();
                    TempData["success"] = "Orden de compra creada exitosamente!";

                    return RedirectToAction("Ver", new { id = IdDeOrdenCompra });

                    //return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Manejar excepciones aquí, realizar rollback si es necesario
                    ModelState.AddModelError(string.Empty, $"Ocurrió un error al procesar la orden de compra: {ex.Message}");
                }

            }

            // Si llegamos aquí, significa que hubo un error, volvemos a mostrar el formulario con los errores.
            ordenCompraVM.ProveedorLista = _context.Proveedor
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Razonsocial
                });
            ordenCompraVM.ProductoCodigoLista = _context.Producto
                .Select(p => new SelectListItem
                {
                    Value = p.Codigo,
                    Text = p.Codigo
                });
            ordenCompraVM.ProductoNombreLista = _context.Producto
                .Select(p => new SelectListItem
                {
                    Value = p.Nombre,
                    Text = p.Nombre
                });

            return View(ordenCompraVM);
        }

        public async Task<ActionResult> Editar(int id)
        {
            var ordenCompra = await _context.OrdenCompra
                    .Include(o => o.Proveedor)
                    .Include(o => o.OrdenCompraDetalle)
                        .ThenInclude(od => od.Producto)
                    .FirstOrDefaultAsync(o => o.Id == id);
            if (ordenCompra == null)
            {
                return NotFound();
            }
            // Crear un objeto OrdenCompraVM y asignar la orden recuperada
            OrdenCompraVM ordenCompraVM = new()
            {
                OrdenCompra = ordenCompra,
                OrdenCompraDetalle = ordenCompra.OrdenCompraDetalle,
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
                ProductoIdSel = id
            };
            return View(ordenCompraVM);
        }

        [HttpPost]
        public async Task<IActionResult> Editar([FromBody] OrdenCompraVM ordenCompraVM)
        {
            try
            {
                // Obtén la orden de compra existente con detalles desde la base de datos
                OrdenCompra ordenCompraExistente = await _context.OrdenCompra
                    .Include(o => o.OrdenCompraDetalle)
                    .FirstOrDefaultAsync(o => o.Id == ordenCompraVM.OrdenCompra.Id);

                if (ordenCompraExistente != null)
                {
                    // Actualiza las propiedades de la orden de compra
                    _context.Entry(ordenCompraExistente).CurrentValues.SetValues(ordenCompraVM.OrdenCompra);


                    // Actualiza los detalles de la orden
                    ActualizarDetallesOrdenCompra(ordenCompraExistente, ordenCompraVM.OrdenCompraDetalle);

                    // Guarda los cambios en la base de datos
                    await _context.SaveChangesAsync();

                    return Json(new { respuesta = true });
                }

                return Json(new { respuesta = false, mensaje = "Orden de compra no encontrada" });
            }
            catch (Exception ex)
            {
                return Json(new { respuesta = false, mensaje = ex.Message });
            }
        }

        //GET
        public async Task<ActionResult> Ver(int id)
        {
            var ordenCompra = await _context.OrdenCompra
                    .Include(o => o.Proveedor)
                    .Include(o => o.OrdenCompraDetalle)
                    .ThenInclude(od => od.Producto)
                    .FirstOrDefaultAsync(o => o.Id == id);
            if (ordenCompra == null)
            {
                return NotFound();
            }
           
            // Crear un objeto OrdenCompraVM y asignar la orden recuperada
            OrdenCompraVM ordenCompraVM = new()
            {
                OrdenCompra = ordenCompra,
                OrdenCompraDetalle = ordenCompra.OrdenCompraDetalle,
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
                ProductoIdSel = id
            };

            return View(ordenCompraVM);
        }

        private void ActualizarDetallesOrdenCompra(OrdenCompra ordenCompra, List<OrdenCompraDetalle> nuevosDetalles)
        {
            // Elimina los detalles existentes que tienen el mismo Id que los nuevos detalles
            var detallesEliminar = ordenCompra.OrdenCompraDetalle
                .Where(detalle => detalle.OrdenCompraId == ordenCompra.Id)
                .ToList();

            foreach (var detalleEliminar in detallesEliminar)
            {
                //// Desvincula la entidad antes de eliminarla
                //_context.Entry(detalleEliminar).State = EntityState.Detached;
                ordenCompra.OrdenCompraDetalle.Remove(detalleEliminar);
            }

            var detallesAgregar = nuevosDetalles
                .Select(nuevoDetalle =>
                {
                    var nuevoDetalleInstancia = new OrdenCompraDetalle
                    {
                        ProductoId = ObtenerProductoIdPorCodigo(nuevoDetalle.Codigo).Value,
                        Cantidad = nuevoDetalle.Cantidad,
                        Precio = nuevoDetalle.Precio,
                        Codigo = nuevoDetalle.Codigo,
                        Nombre = nuevoDetalle.Nombre,
                        OrdenCompra = ordenCompra
                    };
                    return nuevoDetalleInstancia;
                })
                .ToList();

            foreach (var nuevoDetalle in detallesAgregar)
            {
                ordenCompra.OrdenCompraDetalle.Add(nuevoDetalle);
            }

        }

        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordenCompra = await _context.OrdenCompra
                .Include(o => o.Proveedor) // Incluye la información del proveedor
                .FirstOrDefaultAsync(o => o.Id == id);

            if (ordenCompra == null)
            {
                return NotFound();
            }

            // Puedes mostrar una vista de confirmación de eliminación aquí
            // y permitir que el usuario confirme antes de eliminar realmente la orden.

            return View(ordenCompra);
        }

        // POST: OrdenCompra/Eliminar/{id}
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var ordenCompra = await _context.OrdenCompra.FindAsync(id);
            var ordenCompraId = ordenCompra.NroOrden;
            if (ordenCompra == null)
            {
                return NotFound();
            }

            // Elimina la orden de compra
            _context.OrdenCompra.Remove(ordenCompra);

            // Elimina las filas de OrdenCompraDetalle que coincidan con el id
            var ordenCompraDetalles = await _context.OrdenCompraDetalle
                .Where(d => d.OrdenCompraId == ordenCompraId)
                .ToListAsync();

            _context.OrdenCompraDetalle.RemoveRange(ordenCompraDetalles);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult CargarCompra()
        {
            // Obtener la lista de proveedores desde la base de datos
            var proveedores = _context.Proveedor.Select(proveedor =>
                new SelectListItem
                {
                    Value = proveedor.Id.ToString(),
                    Text = proveedor.Razonsocial
                }).ToList();

            // Obtener todas las órdenes de compra desde la base de datos
            var ordenesCompra = _context.OrdenCompra
                        .Include(orden => orden.OrdenCompraDetalle) // Cargar los detalles de la orden de compra
                        .ToList();

            // Crear el ViewModel con la lista de proveedores y todas las órdenes de compra
            var viewModel = new CompraVM
            {
                FechaFactura = DateTime.Today,
                ProveedorLista = proveedores,
                OrdenCompra = ordenesCompra
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult CargarCompra(CompraVM viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {                   

                    // 2. Cambiar puntualmente el estado de la Orden de Compra asociada
                    CambiarEstadoOrdenCompra(viewModel);

                    // 3. Cambiar el precio del producto en la tabla Productos
                    ActualizarPrecioProductos(viewModel);

                    // 1. Generar una fila en la tabla Stock
                    GenerarFilaStock(viewModel);

                    TempData["success"] = "Factura cargada exitosamente!";

                    return RedirectToAction("Index"); // Redirigir a alguna página de éxito
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error al cargar la factura de compra: {ex.Message}");
                    // Manejar el error de alguna manera apropiada, como mostrar un mensaje de error en la vista
                }
            }

            // Si llegamos aquí, significa que ocurrió un error, así que volvemos a cargar la vista
            return View(viewModel);
        }

        // Métodos auxiliares para realizar las operaciones específicas

        private void GenerarFilaStock(CompraVM viewModel)
        {
            foreach (var detalle in viewModel.DetallesFactura)
            {
                // Recuperar el producto correspondiente de la base de datos
                var producto = _context.Producto.FirstOrDefault(p => p.Id == detalle.ProductoId);

                if (producto != null)
                {
                    // Actualizar el stock del producto
                    producto.Stock += detalle.Cantidad;

                    // Crear y guardar la entrada en la tabla Stock
                    var nuevaEntradaStock = new Stock
                    {
                        ProductoId = detalle.ProductoId,
                        Cantidad = detalle.Cantidad,
                        Fecha = viewModel.FechaFactura,
                        Concepto = "Compras",
                        NroComprobante = viewModel.NumeroFactura
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


        private void CambiarEstadoOrdenCompra(CompraVM viewModel)
        {
            // Obtener los IDs únicos de las órdenes de compra asociadas a los detalles de la factura
            var ordenesCompraIds = viewModel.DetallesFactura
                .Select(detalle => detalle.OrdenCompraId)
                .Distinct()
                .ToList();

            // Obtener las órdenes de compra asociadas a los IDs obtenidos
            var ordenesCompra = _context.OrdenCompra
                .Where(orden => ordenesCompraIds.Contains(orden.Id))
                .ToList();

            // Cambiar el estado de cada orden de compra a "Procesada"
            foreach (var orden in ordenesCompra)
            {
                orden.Estado = EstadoOrden.Procesada;
            }

            // Guardar los cambios en la base de datos
            _context.SaveChanges();
        }

        private void ActualizarPrecioProductos(CompraVM viewModel)
        {
            // Iterar por cada detalle de la factura y actualizar el precio del producto en la tabla Productos
            foreach (var detalle in viewModel.DetallesFactura)
            {
                var producto = _context.Producto.Find(detalle.ProductoId);
                if (producto != null)
                {
                    producto.Costo = (double)detalle.Precio;
                    producto.Precio = (double)detalle.Precio * (1 + (producto.Margen / 100));
                }
            }

            // Guardar los cambios en la base de datos
            _context.SaveChanges();
        }

        // Otras acciones del controlador...

        [HttpGet]
        public IActionResult ObtenerDetallesOrdenCompra(int[] ordenIds)
        {
            try
            {
                // Obtener los detalles de las órdenes de compra según los IDs recibidos
                var detalles = _context.OrdenCompraDetalle
                    .Where(detalle => ordenIds.Contains(detalle.OrdenCompraId))
                    .ToList();

                // Devolver los detalles de las órdenes de compra en formato JSON
                return Json(detalles);
            }
            catch (Exception ex)
            {
                // Manejar cualquier error y devolver un código de estado apropiado
                return StatusCode(500, $"Error al obtener los detalles de las órdenes de compra: {ex.Message}");
            }
        }



        [HttpGet]
        public IActionResult ObtenerOrdenesCompra(int proveedorId)
        {
            try
            {
                // Obtener las órdenes de compra del proveedor especificado
                var ordenesCompra = _context.OrdenCompra
                    .Where(o => o.ProveedorId == proveedorId && o.Estado == EstadoOrden.Enviada)
                    .ToList();

                // Devolver las órdenes de compra en formato JSON
                return Json(ordenesCompra);
            }
            catch (Exception ex)
            {
                // Manejar cualquier error y devolver un código de estado apropiado
                return StatusCode(500, $"Error al obtener las órdenes de compra: {ex.Message}");
            }
        }






        public async Task<IActionResult> EnviarPDFPorCorreo(int id, string nombreProveedor, string emailProveedor)
        {
            // URL de la página web desde la que quieres leer el contenido
            string url = $"https://localhost:7259/OrdenCompra/Ver/{id}";

            // Crear cliente HttpClient
            using ( HttpClient client = new HttpClient())
            {
                try
                {
                    // Realizar la solicitud GET a la URL
                    HttpResponseMessage response = await client.GetAsync(url);

                    // Verificar si la solicitud fue exitosa
                    if (response.IsSuccessStatusCode)
                    {
                        // Leer el contenido de la página web
                        string webContent = await response.Content.ReadAsStringAsync();

                        // Actualizar el estado de la orden de compra a "Enviada"
                        var ordenCompra = await _context.OrdenCompra.FirstOrDefaultAsync(o => o.Id == id);
                        if (ordenCompra != null)
                        {
                            ordenCompra.Estado = EstadoOrden.Enviada;
                            await _context.SaveChangesAsync();
                        }
                        // Crear el mensaje de correo electrónico con el contenido de la página web
                        await _emailSender.SendEmailAsync(emailProveedor, "Nueva orden", webContent);
                        TempData["success"] = "Orden de compra enviada exitosamente!";

                    }
                    else
                    {
                        // Manejar el caso donde la solicitud no fue exitosa
                        // Esto podría ser una redirección, un error del servidor, etc.
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (Exception ex)
                {
                    // Manejar cualquier excepción que ocurra durante el proceso
                    // Esto podría ser un error de red, una excepción de lectura/escritura, etc.
                    Console.WriteLine("Error: " + ex.Message);
                    return RedirectToAction(nameof(Index));
                }
            }

            // Redireccionar a la página de índice u otra acción apropiada
            return RedirectToAction(nameof(Index));
        }


        public int? ObtenerProductoIdPorCodigo(string codigo)
        {

            var listaDeProductos = _context.Producto;

            // Buscar el ProductoId por código usando LINQ
            var producto = listaDeProductos.FirstOrDefault(p => p.Codigo == codigo);

            if (producto != null)
            {
                return producto.Id;
            }

            return null; // Manejar el caso en el que no se encontró el ProductoId
        }

        // Función para obtener el último número de orden
        private int ObtenerUltimoNumeroOrden()
        {
            var ultimoOrden = _context.OrdenCompra
                .OrderByDescending(o => o.NroOrden)
                .FirstOrDefault();

            if (ultimoOrden != null)
            {
                return ultimoOrden.NroOrden + 1;
            }

            // Si no hay órdenes en la base de datos, inicia en 1
            return 1;
        }

        // Función para actualizar el último número de orden en la base de datos
        private static void ActualizarUltimoNumeroOrden(int nuevoNumeroOrden)
        {
            // Puedes guardar el último número de orden en algún lugar de la base de datos
            // para usarlo como referencia en el futuro.
        }

        //HASTA ACA ESTA BIEN!!********************************************************************************************************
      
        public IActionResult MostrarPDFenPagina(int id)
        {
            string urlBase = $"{Request.Scheme}://{Request.Host}";
            var url_pagina = $"{urlBase}/OrdenCompra/Ver/{id}";

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = new GlobalSettings()
                {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait
                },
                Objects = {
                    new ObjectSettings(){
                        Page = url_pagina
                    }
                }

            };

            var archivoPDF = _converter.Convert(pdf);

            return File(archivoPDF, "application/pdf");
        }

        public IActionResult DescargarPDF(int id)
        {
            string urlBase = $"{Request.Scheme}://{Request.Host}";
            string url_pagina = $"{urlBase}/OrdenCompra/Ver/{id}";

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = new GlobalSettings()
                {
                    PaperSize = PaperKind.A4,   //A4 (210 x 297 mm); A4Plus (210 mm x 330 mm).
                    Orientation = Orientation.Portrait
                },
                Objects = {
                    new ObjectSettings(){
                        Page = url_pagina
                    }
                }

            };
            // Obtener el número de orden de compra
            var ordenCompra = _context.OrdenCompra
                   .Include(o => o.Proveedor)
                   .FirstOrDefault(o => o.Id == id); if (ordenCompra == null)
            {
                // Manejar el caso en el que no se encuentra la orden de compra
                return NotFound();
            }

            // Crear un nombre de archivo con el número de orden y el nombre del proveedor
            string nombreProveedor = ordenCompra.Proveedor != null ? ordenCompra.Proveedor.Razonsocial : "ProveedorDesconocido";
            string nombrePDF = $"orden_compra_N°{ordenCompra.NroOrden}_{nombreProveedor}.pdf";

            var archivoPDF = _converter.Convert(pdf);

            return File(archivoPDF, "application/pdf", nombrePDF);
        }

    }
}
