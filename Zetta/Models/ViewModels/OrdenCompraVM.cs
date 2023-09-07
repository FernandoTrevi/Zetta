﻿using Microsoft.AspNetCore.Mvc.Rendering;

namespace Zetta.Models.ViewModels
{
    public class OrdenCompraVM
    {
        public OrdenCompra OrdenCompra { get; set; }
        public List<OrdenCompraDetalle> OrdenCompraDetalle { get; set; }
        public IEnumerable<SelectListItem> ProveedorLista { get; set; }
        public IEnumerable<SelectListItem> ProductoCodigoLista { get; set; } // Lista de códigos de productos
        public IEnumerable<SelectListItem> ProductoNombreLista { get; set; } // Lista de nombres de productos
        public DateTime FechaActual { get; set; }
        public string ProductoCodigo { get; set; } // Código del producto seleccionado
        public string ProductoNombre { get; set; } // Nombre del producto seleccionado
        public int ProductoCantidad { get; set; }
    }
}
