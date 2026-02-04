using Microsoft.AspNetCore.Mvc;
using Veterinaria_AppWeb.Data;
using Veterinaria_AppWeb.Models;
using Veterinaria_AppWeb.ViewModels;

namespace Veterinaria_AppWeb.Controllers
{
    public class ServiciosController : Controller
    {
        private readonly VeterinariaContext _context;

        public ServiciosController(VeterinariaContext context)
        {
            _context = context;
        }

        // GET: Servicios
        public IActionResult Index()
        {
            var servicios = _context.Servicio
                .Select(s => new ServicioViewModel
                {
                    Id_Servicio = s.Id_Servicio,
                    Nombre = s.Nombre,
                    Descripcion = s.Descripcion
                }).ToList();

            return View(servicios);
        }

        // GET: Servicios/Details
        public IActionResult Details(int id)
        {
            var servicio = _context.Servicio
                .Where(s => s.Id_Servicio == id)
                .Select(s => new ServicioViewModel
                {
                    Id_Servicio = s.Id_Servicio,
                    Nombre = s.Nombre,
                    Descripcion = s.Descripcion
                }).FirstOrDefault();

            if (servicio == null) return NotFound();

            return View(servicio);
        }

        // GET: Servicios/Create
        public IActionResult Create()
        {
            return View(new ServicioViewModel());
        }

        // POST: Servicios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ServicioViewModel model)
        {
            if (ModelState.IsValid)
            {
                var servicio = new Servicio
                {
                    Nombre = model.Nombre,
                    Descripcion = model.Descripcion
                };

                _context.Servicio.Add(servicio);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Servicios/Edit
        public IActionResult Edit(int id)
        {
            var servicio = _context.Servicio.Find(id);
            if (servicio == null) return NotFound();

            var model = new ServicioViewModel
            {
                Id_Servicio = servicio.Id_Servicio,
                Nombre = servicio.Nombre,
                Descripcion = servicio.Descripcion
            };

            return View(model);
        }

        // POST: Servicios/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ServicioViewModel model)
        {
            if (id != model.Id_Servicio) return NotFound();

            if (ModelState.IsValid)
            {
                var servicio = _context.Servicio.Find(id);
                if (servicio == null) return NotFound();

                servicio.Nombre = model.Nombre;
                servicio.Descripcion = model.Descripcion;

                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Servicios/Delete
        public IActionResult Delete(int id)
        {
            var servicio = _context.Servicio
                .Where(s => s.Id_Servicio == id)
                .Select(s => new ServicioViewModel
                {
                    Id_Servicio = s.Id_Servicio,
                    Nombre = s.Nombre,
                    Descripcion = s.Descripcion
                }).FirstOrDefault();

            if (servicio == null) return NotFound();

            return View(servicio);
        }

        // POST: Servicios/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var servicio = _context.Servicio.Find(id);
            if (servicio == null) return NotFound();

            // Verificar si el servicio está siendo usado en alguna venta
            bool tieneVentas = _context.DetallesVenta.Any(d => d.Id_servicio == id);
            if (tieneVentas)
            {
                TempData["ErrorMessage"] = "No se puede eliminar este servicio porque ya fue utilizado en una o más ventas.";
                return RedirectToAction(nameof(Index));
            }

            _context.Servicio.Remove(servicio);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Servicio eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
