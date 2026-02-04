using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Veterinaria_AppWeb.ViewModels
{
    public class TurnoViewModel
    {
        public int Id_Turno { get; set; }

        [Required(ErrorMessage = "El nombre del cliente es obligatorio")]
        [StringLength(100)]
        [Display(Name = "Nombre")]
        public string NombreCliente { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [StringLength(20)]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime Dia { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "El horario es obligatorio")]
        [DataType(DataType.Time)]
        public TimeSpan Horario { get; set; } = TimeSpan.Zero;

        [Required(ErrorMessage = "Debe seleccionar un servicio")]
        [Display(Name = "Servicio")]
        public int Id_Servicio { get; set; }

        // Para llenar el dropdown
        public IEnumerable<SelectListItem> ServiciosDisponibles { get; set; } = new List<SelectListItem>();
    }
}
