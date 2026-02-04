using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Veterinaria_AppWeb.ViewModels
{
    public class VentaViewModel
    {
        public int Id_venta { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Debe elegir una forma de pago")]
        [Display(Name = "Forma de Pago")]
        public string FormaDePago { get; set; }

        // No se permite ingresar MontoTotal manualmente: se calcula en el controlador
        public decimal MontoTotal { get; set; }

        // Lista de detalles (ítems de venta)
        public List<DetalleVentaViewModel> Detalles { get; set; } = new();

        // --- Para combos en la cabecera ---
        public IEnumerable<SelectListItem> MetodosPago { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Productos { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Servicios { get; set; } = new List<SelectListItem>();
    }
}
