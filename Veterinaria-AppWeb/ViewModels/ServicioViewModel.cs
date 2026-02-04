using System.ComponentModel.DataAnnotations;

namespace Veterinaria_AppWeb.ViewModels
{
    public class ServicioViewModel
    {
        public int Id_Servicio { get; set; }

        [Required(ErrorMessage = "El nombre del servicio es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
        public string? Nombre { get; set; }

        [StringLength(200, ErrorMessage = "La descripción no puede superar los 200 caracteres")]
        public string? Descripcion { get; set; }
    }
}
