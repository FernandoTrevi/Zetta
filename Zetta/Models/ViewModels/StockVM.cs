using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace Zetta.Models.ViewModels
{
    public class StockVM
    {
        public DateTime FechaActual { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un producto.")]
        public int ProductoId { get; set; }

        public SelectList ProductoLista { get; set; }

        [Required(ErrorMessage = "Debe ingresar la cantidad.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero.")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un concepto.")]
        public string Concepto { get; set; }

        public SelectList ConceptoLista { get; set; }

        public List<string> ErrorMessages { get; set; }

        public string ProductoNombre { get; set; }

        public int StockActual { get; set; }

        public List<Stock> Movimientos { get; set; }

    }
}