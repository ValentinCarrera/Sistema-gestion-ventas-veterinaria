using System.ComponentModel.DataAnnotations;

namespace Veterinaria_AppWeb.Models
{
    public class Servicio
    {
        [Key]
        public int Id_Servicio { get; set; }

        [Required]
        [StringLength(100)]
        public string? Nombre { get; set; }

        [StringLength(200)]
        public string? Descripcion { get; set; }
    }
}
