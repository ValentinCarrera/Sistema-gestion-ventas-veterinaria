using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Veterinaria_AppWeb.Models
{
    public class Turno
    {
        [Key]
        public int Id_Turno { get; set; }

        [Required]
        [StringLength(100)]
        public string NombreCliente { get; set; }

        [Required]
        [StringLength(20)]
        public string Telefono { get; set; }

        [Required]        
        [Column(TypeName = "date")]        
        public DateTime Dia { get; set; }

        [Required]        
        [Column(TypeName = "time")]        
        public TimeSpan Horario { get; set; }

        // Relación con Servicio
        [Required]
        public int Id_Servicio { get; set; }

        [ForeignKey("Id_Servicio")]
        public Servicio Servicio { get; set; }
    }
}
