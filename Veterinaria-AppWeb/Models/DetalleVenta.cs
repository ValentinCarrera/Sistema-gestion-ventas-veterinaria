using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Veterinaria_AppWeb.Models
{
    public class DetalleVenta
    {
        [Key]
        public int Id_detalle { get; set; }

        // Relación con Venta
        [Required]
        public int Id_venta { get; set; }

        [ForeignKey("Id_venta")]
        public Venta Venta { get; set; }

        // Puede ser un producto o un servicio
        public int? Id_producto { get; set; }
        [ForeignKey("Id_producto")]
        public Producto Producto { get; set; }

        public int? Id_servicio { get; set; }
        [ForeignKey("Id_servicio")]
        public Servicio Servicio { get; set; }

        [Required]
        public int Cantidad { get; set; } = 1;

        [Required]
        [DataType(DataType.Currency)]
        public decimal PrecioUnitario { get; set; } // ingresado manual

       
        public decimal Subtotal { get; set; } // = Cantidad * PrecioUnitario
    }
}
