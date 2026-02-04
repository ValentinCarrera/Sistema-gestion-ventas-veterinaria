using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Veterinaria_AppWeb.Data;
using Veterinaria_AppWeb.Models;
using Veterinaria_AppWeb.ViewModels;
using System.Collections.Generic;

namespace Veterinaria_AppWeb.Controllers
{
    public class TurnosController : Controller
    {
        private readonly VeterinariaContext _context;
        
        private const int BUFFER_MINUTOS = 60;

        public TurnosController(VeterinariaContext context)
        {
            _context = context;
        }

        // INDEX: lista mensual con filtro por año y mes
        public async Task<IActionResult> Index(int? year, int? month)
        {
            // Mes y año actual si no se pasan parámetros
            var now = DateTime.Now;
            int selYear = year ?? now.Year;
            int selMonth = month ?? now.Month;

            // Rango: primer y último día del mes seleccionado
            var inicio = new DateTime(selYear, selMonth, 1);
            var fin = inicio.AddMonths(1).AddDays(-1);

            var turnos = await _context.Turnos
                .Include(t => t.Servicio)
                .Where(t => t.Dia >= inicio && t.Dia <= fin)
                .OrderBy(t => t.Dia)
                .ThenBy(t => t.Horario)
                .ToListAsync();

            ViewBag.SelYear = selYear;
            ViewBag.SelMonth = selMonth;

            return View(turnos);
        }

        // GET: Create
        public IActionResult Create()
        {
            var vm = new TurnoViewModel
            {
                Dia = DateTime.Today,
                Horario = TimeSpan.FromHours(9), // horario sugerido
                ServiciosDisponibles = GetServiciosSelectList()
            };
            return View(vm);
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TurnoViewModel vm)
        {
            vm.ServiciosDisponibles = GetServiciosSelectList();

            if (!ModelState.IsValid) return View(vm);

            // Validación de conflictos (traigo los turnos del día y chequeo en memoria) 
            var turnosDia = await _context.Turnos
                .Where(t => t.Dia == vm.Dia)
                .ToListAsync();

            var buffer = TimeSpan.FromMinutes(BUFFER_MINUTOS);
            foreach (var t in turnosDia)
            {
                var diff = (t.Horario - vm.Horario).Duration();
                if (diff < buffer)
                {
                    ModelState.AddModelError("", $"Conflicto con turno existente a las {t.Horario:hh\\:mm} (Servicio: {t.Servicio?.Nombre ?? "—"}) — debe haber al menos {BUFFER_MINUTOS} minutos de separación.");
                    return View(vm);
                }
            }

            // Mapear y guardar
            var turno = new Turno
            {
                NombreCliente = vm.NombreCliente,
                Telefono = vm.Telefono,
                Dia = vm.Dia.Date,
                Horario = vm.Horario,
                Id_Servicio = vm.Id_Servicio
            };

            _context.Turnos.Add(turno);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Details
        public async Task<IActionResult> Details(int id)
        {
            var turno = await _context.Turnos
                .Include(t => t.Servicio)
                .FirstOrDefaultAsync(t => t.Id_Turno == id);

            if (turno == null) return NotFound();
            return View(turno);
        }

        // GET: Edit
        public async Task<IActionResult> Edit(int id)
        {
            var turno = await _context.Turnos.FindAsync(id);
            if (turno == null) return NotFound();

            var vm = new TurnoViewModel
            {
                Id_Turno = turno.Id_Turno,
                NombreCliente = turno.NombreCliente,
                Telefono = turno.Telefono,
                Dia = turno.Dia,
                Horario = turno.Horario,
                Id_Servicio = turno.Id_Servicio,
                ServiciosDisponibles = GetServiciosSelectList()
            };
            return View(vm);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TurnoViewModel vm)
        {
            vm.ServiciosDisponibles = GetServiciosSelectList();

            if (id != vm.Id_Turno) return NotFound();
            if (!ModelState.IsValid) return View(vm);

            // Conflicto: considerar otros turnos del día excluyendo el actual
            var turnosDia = await _context.Turnos
                .Where(t => t.Dia == vm.Dia && t.Id_Turno != vm.Id_Turno)
                .ToListAsync();

            var buffer = TimeSpan.FromMinutes(BUFFER_MINUTOS);
            foreach (var t in turnosDia)
            {
                var diff = (t.Horario - vm.Horario).Duration();
                if (diff < buffer)
                {
                    ModelState.AddModelError("", $"Conflicto con turno existente a las {t.Horario:hh\\:mm} (Servicio: {t.Servicio?.Nombre ?? "—"}) — debe haber al menos {BUFFER_MINUTOS} minutos de separación.");
                    return View(vm);
                }
            }

            var turno = await _context.Turnos.FindAsync(id);
            if (turno == null) return NotFound();

            turno.NombreCliente = vm.NombreCliente;
            turno.Telefono = vm.Telefono;
            turno.Dia = vm.Dia.Date;
            turno.Horario = vm.Horario;
            turno.Id_Servicio = vm.Id_Servicio;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Delete
        public async Task<IActionResult> Delete(int id)
        {
            var turno = await _context.Turnos
                .Include(t => t.Servicio)
                .FirstOrDefaultAsync(t => t.Id_Turno == id);

            if (turno == null) return NotFound();
            return View(turno);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var turno = await _context.Turnos.FindAsync(id);
            if (turno == null) return NotFound();

            _context.Turnos.Remove(turno);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Helper: lista de servicios para dropdown
        private List<SelectListItem> GetServiciosSelectList()
        {
            return _context.Servicio
                .OrderBy(s => s.Nombre)
                .Select(s => new SelectListItem
                {
                    Value = s.Id_Servicio.ToString(),
                    Text = $"{s.Nombre} ( {s.Descripcion})"
                }).ToList();
        }
    }
}
