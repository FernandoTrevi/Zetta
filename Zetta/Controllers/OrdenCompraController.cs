using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using Zetta.Datos;
using Zetta.Models;
using Zetta.Models.ViewModels;
using static OrdenCompraController;

public class OrdenCompraController : Controller
{
    private readonly ApplicationDbContext _context;

    public OrdenCompraController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: OrdenCompra/Index
    public async Task<IActionResult> Index()
    {
        var ordenCompra = await _context.OrdenCompra.Include(o => o.Proveedor).ToListAsync();
        return View(ordenCompra);
    }

    [HttpPost]
    public IActionResult Index([FromBody] OrdenCompraVM ordenCompraVM)
    {
        // Crear una nueva orden de compra
        OrdenCompra ordenCompra = ordenCompraVM.OrdenCompra;

        // Asignar los detalles de la orden de compra
        ordenCompra.OrdenCompraDetalle = ordenCompraVM.OrdenCompraDetalle;

        // Agregar la orden de compra a la base de datos
        _context.OrdenCompra.Add(ordenCompra);
        _context.SaveChanges();

        return Json(new { respuesta = true });
    }


    //public class CompraVM
    //{
    //    public Compra oCompra { get; set; }
    //    public List<DetalleCompra> oDetalleCompra { get; set; }



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

    // POST: OrdenCompra/Crear
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(OrdenCompraVM ordenCompraVM)
    {
        if (ModelState.IsValid)
        {


            // 1.Crear una nueva orden de compra
            var ordenCompra = new OrdenCompra
            {
                NroOrden = ordenCompraVM.OrdenCompra.NroOrden,
                Fecha = ordenCompraVM.FechaActual,
                ProveedorId = ordenCompraVM.OrdenCompra.ProveedorId,
                Observaciones = ordenCompraVM.OrdenCompra.Observaciones,
                Estado = ordenCompraVM.OrdenCompra.Estado
            };

            // 2.Agregar la orden de compra al contexto
            _context.Add(ordenCompra);

            // Vuelve a inicializar la lista Detalles antes de agregar elementos
            ordenCompraVM.OrdenCompraDetalle = new List<OrdenCompraDetalle>();

            // 3.Agregar detalles de la orden de compra
            foreach (OrdenCompraDetalle detalle in ordenCompraVM.OrdenCompraDetalle)
            {
                var ordenCompraDetalle = new OrdenCompraDetalle
                {
                    OrdenCompraId = ordenCompra.Id,
                    ProductoId = detalle.ProductoId,
                    Cantidad = detalle.Cantidad
                };

                _context.Add(ordenCompraDetalle);
            }

            // 4.Actualiza el último número de orden en la base de datos
            ActualizarUltimoNumeroOrden(ordenCompraVM.OrdenCompra.NroOrden);

            // 5.Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
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

    // Otras acciones del controlador...

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
    private void ActualizarUltimoNumeroOrden(int nuevoNumeroOrden)
    {
        // Puedes guardar el último número de orden en algún lugar de la base de datos
        // para usarlo como referencia en el futuro.
    }
}








// GET: OrdenCompra/Editar
//public async Task<IActionResult> Editar(int? id)
//    {
//        if (id == null)
//        {
//            return NotFound();
//        }

//        var ordenCompra = await _context.OrdenCompra.FindAsync(id);

//        if (ordenCompra == null)
//        {
//            return NotFound();
//        }

//        return View(ordenCompra);
//    }

// POST: OrdenCompra/Editar
//[HttpPost]
//[ValidateAntiForgeryToken]
//public async Task<IActionResult> Editar(int id, OrdenCompra ordenCompra)
//{
//    if (id != ordenCompra.Id)
//    {
//        return NotFound();
//    }

//    if (ModelState.IsValid)
//    {
//        try
//        {
// Agregar lógica aquí para editar la orden de compra y sus detalles
// Puedes acceder a los detalles a través de ordenCompra.OrdenCompraDetalles
// y actualizarlos en la base de datos

//            _context.Update(ordenCompra);
//            await _context.SaveChangesAsync();
//        }
//        catch (DbUpdateConcurrencyException)
//        {
//            if (!OrdenCompraExists(ordenCompra.Id))
//            {
//                return NotFound();
//            }
//            else
//            {
//                throw;
//            }
//        }
//        return RedirectToAction(nameof(Index));
//    }
//    return View(ordenCompra);
//}

// GET: OrdenCompra/Eliminar
//public async Task<IActionResult> Eliminar(int? id)
//{
//    if (id == null)
//    {
//        return NotFound();
//    }

//    var ordenCompra = await _context.OrdenCompra
//        .FirstOrDefaultAsync(m => m.Id == id);

//    if (ordenCompra == null)
//    {
//        return NotFound();
//    }

//    return View(ordenCompra);
//}

// POST: OrdenCompra/Eliminar
//[HttpPost, ActionName("Eliminar")]
//[ValidateAntiForgeryToken]
//public async Task<IActionResult> EliminarConfirma(int id)
//{
//    var ordenCompra = await _context.OrdenCompra.FindAsync(id);

//    // Agregar lógica aquí para eliminar la orden de compra y sus detalles de la base de datos
//    // Asegúrate de manejar adecuadamente las relaciones con OrdenCompraDetalle

//    _context.OrdenCompra.Remove(ordenCompra);
//    await _context.SaveChangesAsync();
//    return RedirectToAction(nameof(Index));
//}

//private bool OrdenCompraExists(int id)
//{
//    return _context.OrdenCompra.Any(e => e.Id == id);
//}
//}








//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Linq;
//using System.Threading.Tasks;
//using Zetta.Datos;
//using Zetta.Models;
//using Zetta.Models.ViewModels;

//namespace Zetta.Controllers
//{
//    public class OrdenCompraController : Controller
//    {
//        private readonly ApplicationDbContext _db;
//        private readonly IWebHostEnvironment _webHostEnvironment;

//        public OrdenCompraController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
//        {
//            _db = db;
//            _webHostEnvironment = webHostEnvironment;
//        }

//        public IActionResult Index()
//        {
//            var ordenCompra = _db.OrdenCompra.ToList();
//            return View(ordenCompra);
//        }

//        // GET: OrdenCompraController/Crear
//        public async Task<IActionResult> Crear()
//        {
//            var ordenCompraVM = new OrdenCompraVM()
//            {
//                OrdenCompra = new OrdenCompra(),
//                ProveedorLista = await _db.Proveedor.Select(p => new SelectListItem
//                {
//                    Text = p.Razonsocial,
//                    Value = p.Id.ToString()
//                }).ToListAsync(),

//                ProductoCodigoLista = await _db.Producto.Select(pcl => new SelectListItem
//                {
//                    Text = pcl.Codigo,
//                    Value = pcl.Id.ToString()
//                }).ToListAsync(),

//                ProductoNombreLista = await _db.Producto.Select(pnl => new SelectListItem
//                {
//                    Text = pnl.Nombre,
//                    Value = pnl.Id.ToString()
//                }).ToListAsync(),

//                FechaActual = DateTime.Now
//            };
//            return View(ordenCompraVM);
//        }

//        // POST: ProveedorController/Crear
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> Crear(OrdenCompraVM ordenCompraVM)
//        {
//            if (ModelState.IsValid)
//            {
//                // Agregar la orden de compra a la base de datos
//                _db.OrdenCompra.Add(ordenCompraVM.OrdenCompra);
//                await _db.SaveChangesAsync(); 

//                // Procesar los detalles de la orden de compra
//                foreach (var detalle in ordenCompraVM.OrdenCompraDetalle)
//                {
//                    detalle.OrdenCompraId = ordenCompraVM.OrdenCompra.Id;
//                    _db.OrdenCompraDetalle.Add(detalle);
//                }

//                await _db.SaveChangesAsync();
//                return RedirectToAction(nameof(Index));
//            }

//            return View(ordenCompraVM);
//        }
//    }
//}
