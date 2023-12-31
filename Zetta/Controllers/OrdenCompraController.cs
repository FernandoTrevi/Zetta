﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
    public async Task<IActionResult> Index(string buscar, string ordenActual, int? numpag, string filtroActual)
    {
        IQueryable<OrdenCompra> query = _context.OrdenCompra.Include(o => o.Proveedor);

        if (buscar != null)
            numpag = 1;
        else
            buscar = filtroActual;

        ViewData["OrdenActual"] = ordenActual;
        ViewData["FiltroActual"] = buscar;

        if (!string.IsNullOrEmpty(buscar))
        {
            query = query.Where(o => o.NroOrden.ToString().Contains(buscar) || o.Proveedor.Razonsocial.Contains(buscar));
        }

        if (ordenActual == "nroorden")
        {
            query = query.OrderBy(o => o.NroOrden);
        }
        else
        {
            query = query.OrderBy(o => o.Id); // Orden predeterminado
        }

        int cantidadregistros = 5; // Cambia esta cantidad según tus preferencias
        var paginacion = await Paginacion<OrdenCompra>.CrearPaginacion(query, numpag ?? 1, cantidadregistros);

        return View(paginacion);
    }

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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(OrdenCompraVM ordenCompraVM)
    {
        if (ModelState.IsValid)
        {
            try
            {
                // 1. Crear una nueva orden de compra
                var ordenCompra = new OrdenCompra
                {
                    NroOrden = ordenCompraVM.OrdenCompra.NroOrden,
                    Fecha = ordenCompraVM.FechaActual,
                    ProveedorId = ordenCompraVM.OrdenCompra.ProveedorId,
                    Observaciones = ordenCompraVM.OrdenCompra.Observaciones,
                    Estado = ordenCompraVM.OrdenCompra.Estado
                };

                // 2. Agregar la orden de compra al contexto
                _context.Add(ordenCompra);
                await _context.SaveChangesAsync();

                var IdDeOrdenCompra = ordenCompra.Id;

                // 3. Agregar detalles de la orden de compra
                foreach (OrdenCompraDetalle detalle in ordenCompraVM.OrdenCompraDetalle)
                {
                    string codigoSeleccionado = detalle.Codigo;
                    var productoIdSeleccionado = ObtenerProductoIdPorCodigo(codigoSeleccionado);

                    if (productoIdSeleccionado.HasValue)
                    {
                        var ordenCompraDetalle = new OrdenCompraDetalle
                        {
                            OrdenCompraId = IdDeOrdenCompra,
                            ProductoId = productoIdSeleccionado.Value,
                            Codigo = detalle.Codigo,
                            Nombre = detalle.Nombre,
                            Cantidad = detalle.Cantidad
                        };

                        _context.Add(ordenCompraDetalle);
                    }
                    else
                    {
                        // Manejar el caso en el que no se encontró el ProductoId
                        // Puedes mostrar un mensaje de error o tomar una acción adecuada
                    }
                }

                // 4. Actualizar el último número de orden en la base de datos
                ActualizarUltimoNumeroOrden(ordenCompraVM.OrdenCompra.NroOrden);

                // 5. Guardar los cambios en la base de datos
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Manejar excepciones aquí, realizar rollback si es necesario
                ModelState.AddModelError(string.Empty, "Ocurrió un error al procesar la orden de compra.");
            }
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


    public async Task<IActionResult> Eliminar(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var ordenCompra = await _context.OrdenCompra
            .Include(o => o.Proveedor) // Incluye la información del proveedor
            .FirstOrDefaultAsync(o => o.Id == id);

        if (ordenCompra == null)
        {
            return NotFound();
        }

        // Puedes mostrar una vista de confirmación de eliminación aquí
        // y permitir que el usuario confirme antes de eliminar realmente la orden.

        return View(ordenCompra);
    }



    // POST: OrdenCompra/Eliminar/{id}
    [HttpPost, ActionName("Eliminar")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(int id)
    {
        var ordenCompra = await _context.OrdenCompra.FindAsync(id);
        var ordenCompraId = ordenCompra.NroOrden;
        if (ordenCompra == null)
        {
            return NotFound();
        }

        // Elimina la orden de compra
        _context.OrdenCompra.Remove(ordenCompra);

        // Elimina las filas de OrdenCompraDetalle que coincidan con el id
        var ordenCompraDetalles = await _context.OrdenCompraDetalle
            .Where(d => d.OrdenCompraId == ordenCompraId)
            .ToListAsync();

        _context.OrdenCompraDetalle.RemoveRange(ordenCompraDetalles);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }


    // Otras acciones del controlador...

    public int? ObtenerProductoIdPorCodigo(string codigo)
    {

        var listaDeProductos = _context.Producto;

        // Buscar el ProductoId por código usando LINQ
        var producto = listaDeProductos.FirstOrDefault(p => p.Codigo == codigo);

        if (producto != null)
        {
            return producto.Id;
        }

        return null; // Manejar el caso en el que no se encontró el ProductoId
    }

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
