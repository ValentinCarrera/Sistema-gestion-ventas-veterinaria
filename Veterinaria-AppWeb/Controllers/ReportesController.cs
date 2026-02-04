using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Veterinaria_AppWeb.Data;
using Veterinaria_AppWeb.ViewModels;

namespace Veterinaria_AppWeb.Controllers
{
    public class ReportesController : Controller
    {
        private readonly VeterinariaContext _context;
        public ReportesController(VeterinariaContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? año, int? mes)
        {
            // Mes y año actuales por defecto
            var fechaHoy = DateTime.Today;
            int anioSel = año ?? fechaHoy.Year;
            int mesSel = mes ?? fechaHoy.Month;

            // Rango de fechas del mes
            DateTime inicio = new DateTime(anioSel, mesSel, 1);
            DateTime fin = inicio.AddMonths(1);

            var ventasMes = await _context.Ventas
                .Where(v => v.Fecha >= inicio && v.Fecha < fin)
                .ToListAsync();

            // Agrupar por día
            var detallePorDia = ventasMes
                .GroupBy(v => v.Fecha.Date)
                .OrderBy(g => g.Key)
                .Select(g => new ReporteDia
                {
                    Dia = g.Key,
                    Total = g.Sum(x => x.MontoTotal),
                    Efectivo = g.Where(x => x.FormaDePago == "Efectivo").Sum(x => x.MontoTotal),
                    Debito = g.Where(x => x.FormaDePago == "Débito").Sum(x => x.MontoTotal),
                    Credito = g.Where(x => x.FormaDePago == "Crédito").Sum(x => x.MontoTotal),
                    Transferencia = g.Where(x => x.FormaDePago == "Transferencia").Sum(x => x.MontoTotal)
                })
                .ToList();

            var vm = new ReporteMensualViewModel
            {
                Año = anioSel,
                Mes = mesSel,
                DetallePorDia = detallePorDia,
                TotalMes = detallePorDia.Sum(d => d.Total),
                TotalEfectivo = detallePorDia.Sum(d => d.Efectivo),
                TotalDebito = detallePorDia.Sum(d => d.Debito),
                TotalCredito = detallePorDia.Sum(d => d.Credito),
                TotalTransferencia = detallePorDia.Sum(d => d.Transferencia)
            };

            ViewBag.Meses = Enumerable.Range(1, 12).Select(m => new SelectListItem
                {
                    Text = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m),
                    Value = m.ToString(),
                    Selected = m == mesSel
                }).ToList();

            ViewBag.Años = Enumerable.Range(fechaHoy.Year - 5, 6).Select(a => new SelectListItem
                {
                    Text = a.ToString(),
                    Value = a.ToString(),
                    Selected = a == anioSel
                }).ToList();

            return View(vm);
        }

    }
}
