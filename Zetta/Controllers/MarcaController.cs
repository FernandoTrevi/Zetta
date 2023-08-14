using Microsoft.AspNetCore.Mvc;
using Zetta.Datos;
using Zetta.Models;

namespace Zetta.Controllers
{
    public class MarcaController : Controller
    {
        private readonly ApplicationDbContext _db;
        public MarcaController(ApplicationDbContext db)
        {
            _db = db;
        }

        //GET: Marca
        public async Task<IActionResult> Index(string buscar, int? numpag, string filtroActual)
        {
            ViewData["FiltroActual"] = filtroActual;

            if (buscar != null)
                numpag = 1;
            else
                buscar = filtroActual;

            ViewData["Buscar"] = buscar;

            IQueryable<Marca> query = _db.Marca;

            if (!String.IsNullOrEmpty(buscar))
            {
                query = query.Where(m => m.Nombre.ToLower().Contains(buscar.ToLower()));
            }

            query = query.OrderBy(m => m.Nombre); // Ordenar alfabéticamente por nombre

            int cantidadregistros = 5;

            var paginacion = await Paginacion<Marca>.CrearPaginacion(query, numpag ?? 1, cantidadregistros);

            return View(paginacion);
        }

        //GET: Marca
        // public IActionResult Index(string buscar)
        //public IActionResult Index(string buscar, int? numpag, string filtroActual)
        //{
        //    IEnumerable<Marca> lista = _db.Marca;

        //    if (buscar != null)
        //        numpag = 1;
        //    else
        //        buscar = filtroActual;


        //    if (!String.IsNullOrEmpty(buscar))
        //    {
        // Se convierte tanto el nombre de la categoría como el término de búsqueda a minúsculas utilizando el método ToLower(). De esta manera,
        // la búsqueda se realiza sin tener en cuenta las mayúsculas o minúsculas. Ademásla consulta se realizará utilizando el método Contains
        // para verificar si el nombre de la categoría contiene la parte ingresada por el usuario.
        //        lista = lista.Where(m => m.Nombre.ToLower().Contains(buscar.ToLower()));
        //    }

        //    ViewData["Buscar"] = buscar;
        //    ViewData["FiltroActual"] = filtroActual;


        //    lista = lista.OrderBy(m => m.Nombre);
        //    int cantidadregistros = 5;
        //    return View(await Paginacion<Marca>.CrearPaginacion(Nombre.AsNoTracking(), numpag ?? 1, cantidadregistros));
        //    //return View(lista);
        //}


        // GET: Marca/Crear
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Marca marca)
        {
            if (ModelState.IsValid)
            {
                // Comprobar si la marca ya existe en la base de datos
                if (_db.Marca.Any(m => m.Nombre == marca.Nombre))
                {
                    ModelState.AddModelError("Nombre", "La Marca ya existe!");
                    return View(marca);
                }

                _db.Marca.Add(marca);
                _db.SaveChanges();
                TempData["success"] = "Marca creada exitosamente!";
            }
            return RedirectToAction(nameof(Index));
        }
        //public IActionResult Crear(Marca marca)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _db.Marca.Add(marca);
        //        _db.SaveChanges();
        //        TempData["success"] = "Marca creada exitosamente!";
        //    }
        //    return RedirectToAction(nameof(Index));
        //}

        // GET: Marca/Editar
        public IActionResult Editar(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            var obj = _db.Marca.Find(Id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Marca marca)
        {
            if (ModelState.IsValid)
            {
                _db.Marca.Update(marca);
                _db.SaveChanges();
                TempData["success"] = "Marca Actualizada exitosamente!";
                return RedirectToAction(nameof(Index));
            }
            return View(marca);
        }

        // GET: Marca/Eliminar
        public IActionResult Eliminar(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            var obj = _db.Marca.Find(Id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(Marca marca)
        {
            if (marca == null)
            {
                return NotFound();
            }
            _db.Marca.Remove(marca);
            _db.SaveChanges();
            TempData["success"] = "Marca Eliminada exitosamente!";
            return RedirectToAction(nameof(Index));
        }
    }
}
