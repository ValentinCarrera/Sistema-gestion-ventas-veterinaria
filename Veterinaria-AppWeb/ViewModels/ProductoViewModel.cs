using System.ComponentModel.DataAnnotations;

namespace Veterinaria_AppWeb.ViewModels
{
    public class ProductoViewModel
    {
        public int Id_Producto { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string? Nombre { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria")]
        [StringLength(50)]
        public string? Categoria { get; set; }

        [Required(ErrorMessage = "El stock es obligatorio")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock debe ser 0 o mayor")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(typeof(decimal), "0", "9999999999", ErrorMessage = "El precio debe ser 0 o mayor")]
        [DataType(DataType.Currency)]
        public decimal Precio { get; set; }
    }
}
