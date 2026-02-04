using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Veterinaria_AppWeb.Models
{
    public class Venta
    {
        [Key]
        public int Id_venta { get; set; }

        [Required]
        public DateTime Fecha { get; set; } = DateTime.Now; // se carga automático

        [Required]
        [DataType(DataType.Currency)]
        public decimal MontoTotal { get; set; }

        
        [StringLength(20)]
        [Required(ErrorMessage = "La forma de pago es obligatoria")]
        public string FormaDePago { get; set; } // Efectivo, Débito, Crédito, Transferencia
        

        // Relación con DetalleVenta
        public ICollection<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
    }
}
