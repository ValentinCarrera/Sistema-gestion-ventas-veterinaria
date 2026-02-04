using System.ComponentModel.DataAnnotations;

namespace Veterinaria_AppWeb.ViewModels
{
    public class ReposicionStockViewModel
    {
        public int Id_Producto { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public int StockActual { get; set; }

        [Required(ErrorMessage = "Ingrese la cantidad")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }

        // True = sumar al stock existente (comportamiento por defecto).
        // False = reemplazar el stock por la cantidad ingresada.
        [Display(Name = "Sumar a stock existente")]
        public bool Sumar { get; set; } = true;
    }
}
