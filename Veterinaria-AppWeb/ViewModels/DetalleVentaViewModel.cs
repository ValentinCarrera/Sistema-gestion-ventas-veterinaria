using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Veterinaria_AppWeb.ViewModels
{
    public class DetalleVentaViewModel
    {
        public int Id_detalle { get; set; }  // se usa en Edición

        [Display(Name = "Producto")]
        public int? Id_producto { get; set; }   // null si se elige servicio

        [Display(Name = "Servicio")]
        public int? Id_servicio { get; set; }   // null si se elige producto

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Cantidad mínima 1")]
        public int Cantidad { get; set; } = 1;

        [Required]
        [Range(typeof(decimal), "0.01", "9999999999", ErrorMessage = "Ingrese un precio válido")]
        [Display(Name = "Precio Unitario")]
        public decimal PrecioUnitario { get; set; }

        // Solo para la vista (dropdowns) 
        public IEnumerable<SelectListItem> ProductosDisponibles { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> ServiciosDisponibles { get; set; } = new List<SelectListItem>();

        // Propiedad calculada para mostrar en la tabla resumen
        public decimal Subtotal => Cantidad * PrecioUnitario;
    }
}