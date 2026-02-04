using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Veterinaria_AppWeb.Models
{
    public class Producto
    {
        [Key]
        public int Id_Producto { get; set; }

        [Required]
        [StringLength(100)]
        public string? Nombre {  get; set; }

        [Required]
        [StringLength(50)]
        public string? Categoria { get; set; }

        [Required]
        public int Stock { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Precio { get; set; }

    }
}
