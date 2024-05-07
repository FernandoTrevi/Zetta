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

    public class ClienteController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ClienteController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: ClienteController
        public async Task<IActionResult> Index(string buscar, string ordenActual, int? numpag, string filtroActual)
        {
            IQueryable<Cliente> query = _db.Cliente.Include(p => p.Provincia).Include(c => c.CondIva);


            if (buscar != null)
                numpag = 1;
            else
                buscar = filtroActual;

            ViewData["OrdenActual"] = ordenActual;
            ViewData["FiltroActual"] = buscar;

            if (!String.IsNullOrEmpty(buscar))
            {
                query = query.Where(p => p.Nombre.Contains(buscar));
            }

            ViewData["FiltroNombre"] = String.IsNullOrEmpty(ordenActual) ? "NombreDescendente" : "";

            switch (ordenActual)
            {
                case "NombreDescendente":
                    query = query.OrderByDescending(cliente => cliente.Nombre);
                    break;
                default:
                    query = query.OrderBy(cliente => cliente.Nombre);
                    break;
            }

            int cantidadregistros = 5;
            var paginacion = await Paginacion<Cliente>.CrearPaginacion(query, numpag ?? 1, cantidadregistros);
            return View(paginacion);
        }

        // GET: ClienteController/Crear
        public ActionResult Crear()
        {
            ClienteVM clienteVM = new ClienteVM()
            {
                Cliente = new Cliente(),
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
            return View(clienteVM);
        }

        // POST: ClienteController/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Crear(ClienteVM clienteVM)
        {
            if (ModelState.IsValid)
            {
                _db.Cliente.Add(clienteVM.Cliente);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(clienteVM.Cliente);
        }

        // GET: ClienteController/Editar
        public async Task<ActionResult> Editar(int id)
        {
            var cliente = await _db.Cliente.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }

            // Crear un objeto ClienteVM y asignar el cliente recuperado
            ClienteVM clienteVM = new ClienteVM()
            {
                Cliente = cliente,
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

            return View(clienteVM);
        }


        // POST: ClienteController/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Editar(int id, ClienteVM clienteVM)
        {
            if (id != clienteVM.Cliente.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Actualizar los valores del Cliente original con los valores del ViewModel
                    var clienteOriginal = await _db.Cliente.FindAsync(id);
                    if (clienteOriginal == null)
                    {
                        return NotFound();
                    }

                    clienteOriginal.Nombre = clienteVM.Cliente.Nombre;
                    clienteOriginal.Direccion = clienteVM.Cliente.Direccion;
                    clienteOriginal.Localidad = clienteVM.Cliente.Localidad;
                    clienteOriginal.ProvinciaId = clienteVM.Cliente.ProvinciaId;
                    clienteOriginal.Telefono = clienteVM.Cliente.Telefono;
                    clienteOriginal.Email = clienteVM.Cliente.Email;
                    clienteOriginal.CondIvaId = clienteVM.Cliente.CondIvaId;
                    clienteOriginal.Dni = clienteVM.Cliente.Dni;

                    _db.Entry(clienteOriginal).State = EntityState.Modified;
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
            clienteVM.ProvinciaLista = _db.Provincia.Select(p => new SelectListItem
            {
                Text = p.Nombre,
                Value = p.Id.ToString()
            });
            clienteVM.CondIvaLista = _db.CondIva.Select(c => new SelectListItem
            {
                Text = c.Nombre,
                Value = c.Id.ToString()
            });

            return View(clienteVM);
        }


        // GET: ClienteController/Eliminar
        public async Task<ActionResult> Eliminar(int id)
        {
            var cliente = await _db.Cliente.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        // POST: ClienteController/ConfirmarEliminar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmarEliminar(int id)
        {
            var cliente = await _db.Cliente.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            _db.Cliente.Remove(cliente);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
