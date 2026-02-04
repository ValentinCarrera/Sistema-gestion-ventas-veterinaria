using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veterinaria_AppWeb.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using Veterinaria_AppWeb.ViewModels;
using Veterinaria_AppWeb.Models;
using System.Diagnostics;

namespace Veterinaria_AppWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;       

        private readonly VeterinariaContext _context;
        public HomeController(ILogger<HomeController> logger, VeterinariaContext context)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            // Productos con menor stock
            var productos = await _context.Productos
                .OrderBy(p => p.Stock)
                .Take(10)
                .ToListAsync();

            // Semana actual 
            var hoy = DateTime.Today;
            var inicioSemana = hoy.AddDays(-(int)hoy.DayOfWeek + (int)DayOfWeek.Monday);
            var finSemana = inicioSemana.AddDays(7).AddSeconds(-1);

            var turnos = await _context.Turnos
                .Include(t => t.Servicio)   // Mostramos el servicio
                .Where(t => t.Dia >= inicioSemana && t.Dia <= finSemana)
                .OrderBy(t => t.Dia)
                .ThenBy(t => t.Horario)
                .ToListAsync();

            var vm = new HomeViewModel
            {
                ProductosBajoStock = productos,
                TurnosSemana = turnos
            };

            return View(vm);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
