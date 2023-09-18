using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Zetta.Datos;
using Zetta.Models;
using Zetta.Models.ViewModels;

namespace Zetta.Controllers
{
    [Authorize(Roles = WC.AdminRole)]

    public class ProveedorController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProveedorController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: ProveedorController
        public async Task<IActionResult> Index(string buscar, string ordenActual, int? numpag, string filtroActual)
        {
            IQueryable<Proveedor> query = _db.Proveedor.Include(p => p.Provincia).Include(c => c.CondIva);


            if (buscar != null)
                numpag = 1;
            else
                buscar = filtroActual;

            ViewData["OrdenActual"] = ordenActual;
            ViewData["FiltroActual"] = buscar;

            if (!String.IsNullOrEmpty(buscar))
            {
                query = query.Where(p => p.Razonsocial.Contains(buscar));
            }

            ViewData["FiltroNombre"] = String.IsNullOrEmpty(ordenActual) ? "NombreDescendente" : "";

            switch (ordenActual)
            {
                case "NombreDescendente":
                    query = query.OrderByDescending(proveedor => proveedor.Razonsocial);
                    break;
                default:
                    query = query.OrderBy(proveedor => proveedor.Razonsocial);
                    break;
            }

            int cantidadregistros = 5;
            var paginacion = await Paginacion<Proveedor>.CrearPaginacion(query, numpag ?? 1, cantidadregistros);
            return View(paginacion);
        }

        // GET: ProveedorController/Crear
        public ActionResult Crear()
        {
            ProveedorVM proveedorVM = new ProveedorVM()
            {
                Proveedor = new Proveedor(),
                ProvinciaLista = _db.Provincia.Select(p => new SelectListItem
                {
                    Text = p.Nombre,
                    Value = p.Id.ToString()
                }),
                CondIvaLista = _db.CondIva.Select(c => new SelectListItem
                {
                    Text = c.Nombre,
                    Value = c.Id.ToString()
                })
            };
            return View(proveedorVM);
        }

        // POST: ProveedorController/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Crear(ProveedorVM proveedorVM)
        {
            if (ModelState.IsValid)
            {
                _db.Proveedor.Add(proveedorVM.Proveedor);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(proveedorVM.Proveedor);
        }

        // GET: ProveedorController/Editar
        public async Task<ActionResult> Editar(int id)
        {
            var proveedor = await _db.Proveedor.FindAsync(id);
            if (proveedor == null)
            {
                return NotFound();
            }

            // Crear un objeto ProveedorVM y asignar el proveedor recuperado
            ProveedorVM proveedorVM = new ProveedorVM()
            {
                Proveedor = proveedor,
                ProvinciaLista = _db.Provincia.Select(p => new SelectListItem
                {
                    Text = p.Nombre,
                    Value = p.Id.ToString()
                }),
                CondIvaLista = _db.CondIva.Select(c => new SelectListItem
                {
                    Text = c.Nombre,
                    Value = c.Id.ToString()
                })
            };

            return View(proveedorVM);
        }


        // POST: ProveedorController/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Editar(int id, ProveedorVM proveedorVM)
        {
            if (id != proveedorVM.Proveedor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Actualizar los valores del proveedor original con los valores del ViewModel
                    var proveedorOriginal = await _db.Proveedor.FindAsync(id);
                    if (proveedorOriginal == null)
                    {
                        return NotFound();
                    }

                    proveedorOriginal.Razonsocial = proveedorVM.Proveedor.Razonsocial;
                    proveedorOriginal.Direccion = proveedorVM.Proveedor.Direccion;
                    proveedorOriginal.Localidad = proveedorVM.Proveedor.Localidad;
                    proveedorOriginal.ProvinciaId = proveedorVM.Proveedor.ProvinciaId;
                    proveedorOriginal.Telefono = proveedorVM.Proveedor.Telefono;
                    proveedorOriginal.Email = proveedorVM.Proveedor.Email;
                    proveedorOriginal.Contacto = proveedorVM.Proveedor.Contacto;
                    proveedorOriginal.Website = proveedorVM.Proveedor.Website;
                    proveedorOriginal.CondIvaId = proveedorVM.Proveedor.CondIvaId;
                    proveedorOriginal.Cuit = proveedorVM.Proveedor.Cuit;

                    _db.Entry(proveedorOriginal).State = EntityState.Modified;
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Manejar excepciones de concurrencia si es necesario
                    throw;
                }
            }

            // Si el modelo no es válido, retornar a la vista con el ViewModel
            proveedorVM.ProvinciaLista = _db.Provincia.Select(p => new SelectListItem
            {
                Text = p.Nombre,
                Value = p.Id.ToString()
            });
            proveedorVM.CondIvaLista = _db.CondIva.Select(c => new SelectListItem
            {
                Text = c.Nombre,
                Value = c.Id.ToString()
            });

            return View(proveedorVM);
        }

       
        // GET: ProveedorController/Eliminar
        public async Task<ActionResult> Eliminar(int? id)
        {
            var proveedor = await _db.Proveedor.FindAsync(id);
            if (proveedor == null)
            {
                return NotFound();
            }
            return View(proveedor);
        }

        // POST: ProveedorController/ConfirmarEliminar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Eliminar(int id)
        {
            var proveedor = await _db.Proveedor.FindAsync(id);
            if (proveedor == null)
            {
                return NotFound();
            }
            _db.Proveedor.Remove(proveedor);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}