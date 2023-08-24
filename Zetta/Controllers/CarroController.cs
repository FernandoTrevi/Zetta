using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Zetta.Datos;
using Zetta.Models;
using Zetta.Utilidades;

namespace Zetta.Controllers
{
    public class CarroController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CarroController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            List<CarroCompra> carroComprasList = new List<CarroCompra>();
            if(HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.SessionCarroCompras) != null &&
               HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.SessionCarroCompras).Count() > 0) 
            {
                carroComprasList = HttpContext.Session.Get<List<CarroCompra>>(WC.SessionCarroCompras);
            }
            List <int> prodEnCarro = carroComprasList.Select(i=> i.ProductoId).ToList();
            IEnumerable<Producto> productoList = _db.Producto.Where(p=>prodEnCarro.Contains(p.Id)).ToList();
            return View(productoList);
        }

        public IActionResult Remover(int Id)
        {
            List<CarroCompra> carroComprasList = new List<CarroCompra>();
            if (HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.SessionCarroCompras) != null &&
               HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.SessionCarroCompras).Count() > 0)
            {
                carroComprasList = HttpContext.Session.Get<List<CarroCompra>>(WC.SessionCarroCompras);
            }

            carroComprasList.Remove(carroComprasList.FirstOrDefault(p => p.ProductoId == Id));

            HttpContext.Session.Set<List<CarroCompra>>(WC.SessionCarroCompras, carroComprasList);
           
            return RedirectToAction(nameof(Index));
        }
    }
}
