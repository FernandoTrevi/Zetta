using Microsoft.AspNetCore.Mvc;
using Zetta.Datos;
using Zetta.Models;

namespace Zetta.Controllers
{
    public class CategoriaController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategoriaController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: Categoria
        public async Task<IActionResult> Index(string buscar, int? numpag, string filtroActual)
        {
            ViewData["FiltroActual"] = filtroActual;

            if (buscar != null)
                numpag = 1;
            else
                buscar = filtroActual;

            ViewData["Buscar"] = buscar;

            IQueryable<Categoria> query = _db.Categoria;

            if (!String.IsNullOrEmpty(buscar))
            {
                query = query.Where(c => c.Nombre.ToLower().Contains(buscar.ToLower()));
            }

            int cantidadregistros = 5;
            var paginacion = await Paginacion<Categoria>.CrearPaginacion(query, numpag ?? 1, cantidadregistros);

            return View(paginacion);
        }

        //public IActionResult Index(string buscar)
        //{
        //    IEnumerable<Categoria> lista = _db.Categoria;

        //    if (!String.IsNullOrEmpty(buscar))
        //    {
        // Se convierte tanto el nombre de la categoría como el término de búsqueda a minúsculas utilizando el método ToLower(). De esta manera,
        // la búsqueda se realiza sin tener en cuenta las mayúsculas o minúsculas. Ademásla consulta se realizará utilizando el método Contains
        // para verificar si el nombre de la categoría contiene la parte ingresada por el usuario.
        //        lista = lista.Where(c => c.Nombre.ToLower().Contains(buscar.ToLower()));
        //    }

        //    lista = lista.OrderBy(c => c.Nombre);

        //    return View(lista);
        //}

        //public IActionResult Crear()
        //{
        //    return View();
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                // Comprobar si la categoria ya existe en la base de datos
                if (_db.Categoria.Any(c => c.Nombre == categoria.Nombre))
                {
                    ModelState.AddModelError("Nombre", "La Categoría ya existe!");
                    return View(categoria);
                }

                _db.Categoria.Add(categoria);
                _db.SaveChanges();
                TempData["success"] = "Categoría creada exitosamente!";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Categoria/Editar
        public IActionResult Editar(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            var obj = _db.Categoria.Find(Id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                _db.Categoria.Update(categoria);
                _db.SaveChanges();
                TempData["success"] = "Categoría Actualizada exitosamente";
                return RedirectToAction(nameof(Index));
            }
            return View(categoria);
        }

        // GET: Categoria/Eliminar
        public IActionResult Eliminar(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            var obj = _db.Categoria.Find(Id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(Categoria categoria)
        {
            if (categoria == null)
            {
                return NotFound();
            }
            _db.Categoria.Remove(categoria);
            _db.SaveChanges();
            TempData["success"] = "Categoría Eliminada exitosamente";
            return RedirectToAction(nameof(Index));
        }
    }
}

