using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Zetta.Datos;
using Zetta.Models;
using Zetta.Models.ViewModels;
using Zetta.Utilidades;

namespace Zetta.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db; 
        } 

        public IActionResult Index()
        {
            HomeVM homeVM = new()
            {
                Productos = _db.Producto.Include(m => m.Marca).Include(c => c.Categoria),
                Categorias = _db.Categoria                
            };
            return View(homeVM);
        }

        public IActionResult Detalle(int Id) 
        {
            List<CarroCompra> carroComprasList = new();
            if (HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.SessionCarroCompras) != null &&
                HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.SessionCarroCompras).Count() > 0)
            {
                carroComprasList = HttpContext.Session.Get<List<CarroCompra>>(WC.SessionCarroCompras);
            }
            DetalleVM detalleVM = new()
            {
                Producto = _db.Producto.Include(m => m.Marca).Include(c => c.Categoria)
                .Where(p=>p.Id == Id).FirstOrDefault(),
                ExisteEnCarro = false
            };
            foreach (var item in carroComprasList)
            {
                if(item.ProductoId == Id)
                {
                    detalleVM.ExisteEnCarro= true;
                }
            }
            return View(detalleVM);
        }

        [HttpPost, ActionName("Detalle")]
        public IActionResult DetallePost(int Id)
        {
            List<CarroCompra>carroComprasList = new();
            if(HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.SessionCarroCompras) != null &&
                HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.SessionCarroCompras).Count() > 0)
            {
                carroComprasList = HttpContext.Session.Get<List<CarroCompra>>(WC.SessionCarroCompras);
            }
            carroComprasList.Add(new CarroCompra { ProductoId = Id});
            HttpContext.Session.Set(WC.SessionCarroCompras, carroComprasList);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoverDeCarro(int Id)
        {
            List<CarroCompra> carroComprasList = new();
            if (HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.SessionCarroCompras) != null &&
                HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.SessionCarroCompras).Count() > 0)
            {
                carroComprasList = HttpContext.Session.Get<List<CarroCompra>>(WC.SessionCarroCompras);
            }
            var prodARemover = carroComprasList.SingleOrDefault(x => x.ProductoId == Id);
            if (prodARemover != null)
            {
                carroComprasList.Remove(prodARemover);
            }
            HttpContext.Session.Set(WC.SessionCarroCompras, carroComprasList);
            return RedirectToAction(nameof(Index));

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}