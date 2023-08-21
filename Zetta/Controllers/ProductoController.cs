using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Zetta.Datos;
using Zetta.Models;
using Zetta.Models.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml.Table;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Zetta.Controllers
{
    public class ProductoController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductoController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Producto
        // GET: Producto
        public async Task<IActionResult> Index(string buscar, string ordenActual, int? numpag, string filtroActual)
        {
            IQueryable<Producto> query = _db.Producto.Include(m => m.Marca).Include(c => c.Categoria);


            if (buscar != null)
                numpag = 1;
            else
                buscar = filtroActual;

            ViewData["OrdenActual"] = ordenActual;
            ViewData["FiltroActual"] = buscar;

            if (!String.IsNullOrEmpty(buscar))
            {
                query = query.Where(p => p.Nombre.Contains(buscar) || p.Codigo.Contains(buscar));
            }

            ViewData["FiltroNombre"] = String.IsNullOrEmpty(ordenActual) ? "NombreDescendente" : "";
            ViewData["FiltroCodigo"] = ordenActual == "CodigoAscendente" ? "CodigoDescendente" : "CodigoAscendente";

            switch (ordenActual)
            {
                case "NombreDescendente":
                    query = query.OrderByDescending(producto => producto.Nombre);
                    break;
                case "CodigoDescendente":
                    query = query.OrderByDescending(producto => producto.Codigo);
                    break;
                case "CodigoAscendente":
                    query = query.OrderBy(producto => producto.Codigo);
                    break;
                default:
                    query = query.OrderBy(producto => producto.Nombre);
                    break;
            }

            int cantidadregistros = 5;
            var paginacion = await Paginacion<Producto>.CrearPaginacion(query, numpag ?? 1, cantidadregistros);
            return View(paginacion);
        }

        public IActionResult Upsert(int? Id)
        {
            ProductoVM productoVM = new ProductoVM()
            {
                Producto = new Producto(),
                MarcaLista = _db.Marca.Select(m => new SelectListItem
                {
                    Text = m.Nombre,
                    Value = m.Id.ToString()
                }),
                CategoriaLista = _db.Categoria.Select(c => new SelectListItem
                {
                    Text = c.Nombre,
                    Value = c.Id.ToString()
                })
            };

            if (Id == null)
            {
                // Crear un Nuevo Producto
                return View(productoVM);
            }
            else
            {
                productoVM.Producto = _db.Producto.Find(Id);
                if (productoVM.Producto == null)
                {
                    return NotFound();
                }
                return View(productoVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductoVM productoVM)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;

                if (productoVM.Producto.Id == 0)
                {
                    // Crear
                    string upload = webRootPath + WC.ImagenRuta;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);

                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    productoVM.Producto.ImagenUrl = fileName + extension;
                    _db.Producto.Add(productoVM.Producto);
                }
                else
                {
                    // Actualizar
                    var objProducto = _db.Producto.AsNoTracking().FirstOrDefault(p => p.Id == productoVM.Producto.Id);

                    if (files.Count > 0)  // Se carga nueva Imagen
                    {
                        string upload = webRootPath + WC.ImagenRuta;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        // borrar la imagen anterior
                        var anteriorFile = Path.Combine(upload, objProducto.ImagenUrl);
                        if (System.IO.File.Exists(anteriorFile))
                        {
                            System.IO.File.Delete(anteriorFile);
                        }
                        // fin Borrar imagen anterior

                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }

                        productoVM.Producto.ImagenUrl = fileName + extension;
                    }  // Caso contrario si no se carga una nueva imagen
                    else
                    {
                        productoVM.Producto.ImagenUrl = objProducto.ImagenUrl;
                    }

                    // Copiar el valor existente de Stock al objeto productoVM.Producto
                    productoVM.Producto.Stock = objProducto.Stock;

                    // Realizar la actualización del producto en la base de datos
                    _db.Producto.Update(productoVM.Producto);

                }
                productoVM.Producto.Precio = Math.Round(productoVM.Producto.Precio, 2);
                _db.SaveChanges();
                return RedirectToAction("Index");
            } // If ModelIsValid
            // Se llenan nuevamente las listas si algo falla.
            productoVM.MarcaLista = _db.Marca.Select(m => new SelectListItem
            {
                Text = m.Nombre,
                Value = m.Id.ToString()
            });
            productoVM.CategoriaLista = _db.Categoria.Select(c => new SelectListItem
            {
                Text = c.Nombre,
                Value = c.Id.ToString()
            });
            return View(productoVM);

        }
        //Get
        public IActionResult Eliminar(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }

            Producto producto = _db.Producto.Include(m => m.Marca)
                                            .Include(c => c.Categoria)
                                            .FirstOrDefault(p => p.Id == Id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);

        }

        // Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(Producto producto)
        {
            if (producto == null)
            {
                return NotFound();
            }
            // Eliminar la imagen
            string upload = _webHostEnvironment.WebRootPath + WC.ImagenRuta;

            // borrar la imagen anterior
            var anteriorFile = Path.Combine(upload, producto.ImagenUrl);
            if (System.IO.File.Exists(anteriorFile))
            {
                System.IO.File.Delete(anteriorFile);
            }
            // fin Borrar imagen anterior

            _db.Producto.Remove(producto);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult ExportarExcel()
        {
            var productos = _db.Producto.Include(m => m.Marca)
                                        .Include(c => c.Categoria)
                                        .ToList();

            byte[] fileContents;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Productos");

                // Definir encabezados de columna
                worksheet.Cells[1, 1].Value = "Código";
                worksheet.Cells[1, 2].Value = "Nombre";
                worksheet.Cells[1, 3].Value = "Marca";
                worksheet.Cells[1, 4].Value = "Categoría";
                worksheet.Cells[1, 5].Value = "Precio";

                // Aplicar formato a las columnas
                worksheet.Column(1).Style.Numberformat.Format = "@";  // Formato cadena (string) para la columna "Código"
                worksheet.Column(5).Style.Numberformat.Format = "#,##0.00";  // Formato de moneda para la columna "Precio"

                // Llenar datos de productos
                int row = 2;
                foreach (var producto in productos)
                {
                    worksheet.Cells[row, 1].Value = producto.Codigo;
                    worksheet.Cells[row, 2].Value = producto.Nombre;
                    worksheet.Cells[row, 3].Value = producto.Marca.Nombre;
                    worksheet.Cells[row, 4].Value = producto.Categoria.Nombre;
                    worksheet.Cells[row, 5].Value = producto.Precio;

                    row++;
                }

                // Autoajustar ancho de columnas
                worksheet.Cells.AutoFitColumns();

                // Agregar estilo de tabla a toda la tabla
                var tableRange = worksheet.Cells[1, 1, row - 1, 5];
                var table = worksheet.Tables.Add(tableRange, "TablaProductos");
                table.TableStyle = TableStyles.Medium2;

                // Convertir el paquete de Excel a un arreglo de bytes
                fileContents = package.GetAsByteArray();
            }

            // Devolver el archivo de Excel como una descarga
            return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "productos.xlsx");
        }
    }
}
