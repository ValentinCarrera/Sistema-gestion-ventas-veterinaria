using System;
using System.Collections.Generic;
using Veterinaria_AppWeb.Models;

namespace Veterinaria_AppWeb.ViewModels
{
    public class HomeViewModel
    {
        public List<Producto> ProductosBajoStock { get; set; } = new();
        public List<Turno> TurnosSemana { get; set; } = new();
    }
}
