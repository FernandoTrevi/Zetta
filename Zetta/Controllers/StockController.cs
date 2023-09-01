using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Zetta.Datos;
using Zetta.Models.ViewModels;
using Zetta.Models;
using Microsoft.AspNetCore.Authorization;
using Zetta;

namespace Zetta1.Controllers
{
    [Authorize (Roles = WC.AdminRole)]
    public class StockController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StockController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: Stock
        public IActionResult Index(string? ProductoNombre)
        {
            var stockVM = new StockVM();

            // Obtener la fecha actual
            stockVM.FechaActual = DateTime.Now;

            // Obtener la lista de productos
            stockVM.ProductoLista = GetProductoList();

            // Obtener la lista de conceptos
            stockVM.ConceptoLista = GetConceptoList();



            stockVM.ProductoNombre = ProductoNombre;


            return View(stockVM);
        }

        // POST: Stock
        [HttpPost]
        public IActionResult Index([Bind("FechaActual,ProductoId,Cantidad,Concepto")] StockVM stockVM)
        {
            if (ModelState.IsValid)
            {
                // Obtener el producto seleccionado
                var producto = GetProductoById(stockVM.ProductoId);

                if (producto != null)
                {
                    int newStock = producto.Stock;

                    // Actualizar el stock del producto según el concepto seleccionado
                    if (stockVM.Concepto == "Ingreso de producto" || stockVM.Concepto == "Compras" || stockVM.Concepto == "Devoluciones")
                    {
                        newStock += stockVM.Cantidad;
                    }
                    else
                    {
                        newStock -= stockVM.Cantidad;

                        if (newStock < 0)
                        {
                            TempData["ErrorMessage"] = "No se puede realizar esta operación. Stock insuficiente.";
                        }
                    }

                    if (newStock >= 0)
                    {
                        producto.Stock = newStock;

                        // Guardar los cambios en la base de datos
                        var stock = new Stock
                        {
                            ProductoId = stockVM.ProductoId,
                            Cantidad = stockVM.Cantidad,
                            Fecha = stockVM.FechaActual,
                            Concepto = stockVM.Concepto
                        };

                        _context.Add(stock);
                        _context.SaveChanges();

                        // Mostrar mensaje de confirmación
                        TempData["Mensaje"] = "El stock se ha actualizado correctamente.";
                    }
                }

                // Redireccionar al caso de uso "Gestionar Productos"
                return RedirectToAction("Index");
            }


            // El modelo no es válido, obtener los mensajes de error
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                                          .Select(e => e.ErrorMessage);

            // Hacer algo con los mensajes de error, como agregarlos a una lista para mostrar en la vista
            var errorMessages = errors.ToList();

            // Volver a cargar las listas necesarias en el ViewModel
            stockVM.ProductoLista = GetProductoList();
            stockVM.ConceptoLista = GetConceptoList();

            // Pasar el ViewModel con los mensajes de error a la vista
            stockVM.ErrorMessages = errorMessages;

            return View(stockVM);
        }

        // Método para obtener la lista de productos
        // Método para obtener la lista de productos
        private SelectList GetProductoList()
        {
            var productos = _context.Producto.ToList();
            return new SelectList(productos, "Id", "Nombre");
        }


        // Método para obtener un producto por su ID
        private Producto GetProductoById(int productoId)
        {
            return _context.Producto.FirstOrDefault(p => p.Id == productoId);

        }



        // Método para obtener la lista de conceptos
        private SelectList GetConceptoList()
        {
            var conceptos = new[]
            {
                new SelectListItem { Value = "Ingreso de producto", Text = "Ingreso de producto" },
                new SelectListItem { Value = "Compras", Text = "Compras" },
                new SelectListItem { Value = "Devoluciones", Text = "Devoluciones" },
                new SelectListItem { Value = "Salida de producto", Text = "Salida de producto" },
                new SelectListItem { Value = "Venta", Text = "Venta" },
                new SelectListItem { Value = "Roturas/Pérdidas", Text = "Roturas/Pérdidas" },
                new SelectListItem { Value = "Uso Interno", Text = "Uso Interno" }
            };

            return new SelectList(conceptos, "Value", "Text");
        }
        // GET: Stock/VerStock/{ProductoId}
        public IActionResult VerStock([Bind("StockActual,ProductoNombre")] int id)
        {
            var producto = GetProductoById(id);

            if (producto != null)
            {
                var stockVM = new StockVM
                {
                    ProductoNombre = producto.Nombre,
                    StockActual = producto.Stock,
                    Movimientos = GetMovimientosByProductoId(id)
                };

                return View(stockVM);
            }

            // Producto no encontrado, puedes redireccionar a una página de error o realizar otra acción según tus necesidades.
            return RedirectToAction("Error");
        }
        private List<Stock> GetMovimientosByProductoId(int productoId)
        {
            return _context.Stock.Where(s => s.ProductoId == productoId).ToList();
        }
    }
}