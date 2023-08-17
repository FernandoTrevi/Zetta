using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Zetta.Controllers
{
    public class ProveedorController : Controller
    {
        // GET: ProveedorController
        public ActionResult Index()
        {
            return View();
        }

        // GET: ProveedorController/Crear
        public ActionResult Crear()
        {
            return View();
        }

        // POST: ProveedorController/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ProveedorController/Editar
        public ActionResult Editar(int id)
        {
            return View();
        }

        // POST: ProveedorController/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ProveedorController/Eliminar
        public ActionResult Eliminar(int id)
        {
            return View();
        }

        // POST: ProveedorController/Eliminar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Eliminar(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
