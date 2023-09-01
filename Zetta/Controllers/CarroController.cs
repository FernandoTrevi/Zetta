using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Zetta.Datos;
using Zetta.Models;
using Zetta.Models.ViewModels;
using Zetta.Utilidades;

namespace Zetta.Controllers
{
    public class CarroController : Controller
    {
        [BindProperty]
        public ProductoUsuarioVM productoUsuarioVM { get; set; }
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;
        public CarroController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment, IEmailSender emailSender)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
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

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {
            return RedirectToAction(nameof(Resumen));
        }

        public IActionResult Resumen() 
        {
            //Trae al usuario conectado
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            //

            //Obtiene la lista de productos de la sesión
            List<CarroCompra> carroComprasList = new List<CarroCompra>();
            if (HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.SessionCarroCompras) != null &&
               HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.SessionCarroCompras).Count() > 0)
            {
                carroComprasList = HttpContext.Session.Get<List<CarroCompra>>(WC.SessionCarroCompras);
            }
            List<int> prodEnCarro = carroComprasList.Select(i => i.ProductoId).ToList();
            IEnumerable<Producto> productoList = _db.Producto.Where(p => prodEnCarro.Contains(p.Id)).ToList();
            //

            //Llena el viewModels
            productoUsuarioVM = new ProductoUsuarioVM()
            {
                UsuarioAplicacion = _db.UsuarioAplicacion.FirstOrDefault(u => u.Id == claims.Value),
                ProductoLista = productoList.ToList(),

            };
            //

            //Retorna el ViewModels a la vista Resúmen
            return View(productoUsuarioVM); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Resumen")]
        public async Task<IActionResult> ResumenPost(ProductoUsuarioVM productoUsuarioVM)
        {
            var templatePath = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
                                                               + "templates"
                                                               + Path.DirectorySeparatorChar.ToString()
                                                               + "PlantillaOrden.html";
            var subjet = "Nueva orden";
            var htmlBody = "";

            using (StreamReader sr = System.IO.File.OpenText(templatePath))
            {
                htmlBody = sr.ReadToEnd();
            }

            StringBuilder productoListaSb = new StringBuilder();

            foreach(var prod in productoUsuarioVM.ProductoLista)
            {
                productoListaSb.Append($" - Nombre: {prod.Nombre} <span style=´font-size:14px;´>(Id: {prod.Id})<span/><br/>");
            }

            string msgBody = string.Format(htmlBody,
                                           productoUsuarioVM.UsuarioAplicacion.NombreCompleto,
                                           productoUsuarioVM.UsuarioAplicacion.Email,
                                           productoUsuarioVM.UsuarioAplicacion.PhoneNumber,
                                           productoListaSb.ToString());

            await _emailSender.SendEmailAsync(WC.EmailAdmin,subjet,msgBody);
                


            return RedirectToAction(nameof(Confirmacion));
        }

        public IActionResult Confirmacion()
        {
            HttpContext.Session.Clear();
            return View();
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
