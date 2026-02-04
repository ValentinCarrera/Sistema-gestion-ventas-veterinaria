using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Veterinaria_AppWeb.Data;           // <-- tu namespace de DbContext
using Veterinaria_AppWeb.Models;
using Veterinaria_AppWeb.ViewModels;
using System.Collections.Generic;

namespace Veterinaria_AppWeb.Controllers
{
    public class VentasController : Controller
    {
        private readonly VeterinariaContext _context;

        public VentasController(VeterinariaContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(DateTime? fecha)
        {
            // Fecha a filtrar (si no viene, hoy)
            var fechaFiltro = fecha?.Date ?? DateTime.Today;
            var start = fechaFiltro;
            var end = fechaFiltro.AddDays(1);

            // Traer ventas del día 
            var ventas = await _context.Ventas
                .AsNoTracking()
                .Include(v => v.Detalles).ThenInclude(d => d.Producto)
                .Include(v => v.Detalles).ThenInclude(d => d.Servicio)
                .Where(v => v.Fecha >= start && v.Fecha < end)
                .OrderByDescending(v => v.Fecha)
                .ToListAsync();

            // Calcular totales
            var totalDia = ventas.Sum(v => v.MontoTotal);
            var totalEfectivo = ventas.Where(v => v.FormaDePago == "Efectivo").Sum(v => v.MontoTotal);
            var totalDebito = ventas.Where(v => v.FormaDePago == "Débito").Sum(v => v.MontoTotal);
            var totalCredito = ventas.Where(v => v.FormaDePago == "Crédito").Sum(v => v.MontoTotal);
            var totalTransferencia = ventas.Where(v => v.FormaDePago == "Transferencia").Sum(v => v.MontoTotal);

            ViewBag.FechaFiltro = fechaFiltro.ToString("yyyy-MM-dd");
            ViewBag.TotalDia = totalDia;
            ViewBag.TotalEfectivo = totalEfectivo;
            ViewBag.TotalDebito = totalDebito;
            ViewBag.TotalCredito = totalCredito;
            ViewBag.TotalTransferencia = totalTransferencia;

            return View(ventas);

        }
      
        // CREATE  (GET)        
        public IActionResult Create()
        {
            var vm = new VentaViewModel
            {
                Fecha = DateTime.Now,
                MetodosPago = GetMetodosPago(),
                Productos = GetProductosSelectList(),
                Servicios = GetServiciosSelectList(),
                Detalles = new List<DetalleVentaViewModel>
                {
                    new DetalleVentaViewModel
                    {
                        ProductosDisponibles = GetProductosSelectList(),
                        ServiciosDisponibles = GetServiciosSelectList()
                    }
                }
            };
            return View(vm);
        }

        // CREATE  (POST)

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VentaViewModel vm)
        {
            // 1) Verificar stock ANTES de cualquier ModelState.IsValid
            //    (por si el modelo es válido pero el stock no)
            var detallesProductos = vm.Detalles
                .Where(d => d.Id_producto.HasValue && d.Cantidad > 0)
                .ToList();

            foreach (var d in detallesProductos)
            {
                var producto = await _context.Productos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id_Producto == d.Id_producto.Value);

                if (producto == null)
                {
                    ModelState.AddModelError("",
                        $"El producto seleccionado no existe.");
                    continue;
                }

                if (d.Cantidad > producto.Stock)
                {
                    ModelState.AddModelError("", $"Stock insuficiente para {producto.Nombre}. Disponible: {producto.Stock}");
                }
            }

            // 2) Si ya hubo errores de stock, recargar combos y volver
            if (!ModelState.IsValid)
            {
                return RecargarYVolver(vm);
            }

            //  Validar modelo completo
            if (!ModelState.IsValid)
            {
                return RecargarYVolver(vm);
            }

            // 4) Filtrar ítems válidos (producto o servicio)
            // exige que al menos un producto o servicio esté elegido y que cantidad y precio sean correctos
            var detallesValidos = vm.Detalles
                .Where(d =>
                    (d.Id_producto.HasValue || d.Id_servicio.HasValue) && d.Cantidad > 0 && d.PrecioUnitario >= 0)
                .ToList();

            if (!detallesValidos.Any())
            {
                ModelState.AddModelError("", "Debe agregar al menos un ítem válido.");
                return RecargarYVolver(vm);
            }

            // 5) Crear la venta
            var venta = new Venta
            {
                Fecha = DateTime.Now,
                FormaDePago = vm.FormaDePago,
                MontoTotal = detallesValidos.Sum(d => d.Cantidad * d.PrecioUnitario)
            };

            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync();

            // 6) Crear detalles y si es producto descontar stock
            foreach (var d in detallesValidos)
            {
                var detalle = new DetalleVenta
                {
                    Id_venta = venta.Id_venta,
                    Id_producto = d.Id_producto,
                    Id_servicio = d.Id_servicio,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Subtotal = d.Cantidad * d.PrecioUnitario
                };
                _context.DetallesVenta.Add(detalle);

                if (d.Id_producto.HasValue)
                {
                    var producto = await _context.Productos
                        .FirstOrDefaultAsync(p => p.Id_Producto == d.Id_producto.Value);

                    if (producto != null)
                    {
                        // Evita stock negativo
                        producto.Stock = Math.Max(0, producto.Stock - d.Cantidad);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Helper para recargar combos y volver a la vista
        private IActionResult RecargarYVolver(VentaViewModel vm)
        {
            vm.MetodosPago = GetMetodosPago();
            vm.Productos = GetProductosSelectList();
            vm.Servicios = GetServiciosSelectList();

            foreach (var d in vm.Detalles)
            {
                d.ProductosDisponibles = GetProductosSelectList();
                d.ServiciosDisponibles = GetServiciosSelectList();
            }
            return View("Create", vm);
        }

        //  Details
        public async Task<IActionResult> Details(int id)
        {
            var venta = await _context.Ventas
                .Include(v => v.Detalles)
                    .ThenInclude(d => d.Producto)
                .Include(v => v.Detalles)
                    .ThenInclude(d => d.Servicio)
                .FirstOrDefaultAsync(v => v.Id_venta == id);

            if (venta == null) return NotFound();
            return View(venta);
        }

        // Delete
        public async Task<IActionResult> Delete(int id)
        {
            var venta = await _context.Ventas
                .Include(v => v.Detalles)
                    .ThenInclude(d => d.Producto)
                .Include(v => v.Detalles)
                    .ThenInclude(d => d.Servicio)
                .FirstOrDefaultAsync(v => v.Id_venta == id);

            if (venta == null) return NotFound();
            return View(venta);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venta = await _context.Ventas
                .Include(v => v.Detalles)
                .FirstOrDefaultAsync(v => v.Id_venta == id);

            if (venta == null) return NotFound();

            // Reponer stock al eliminar
            foreach (var d in venta.Detalles)
            {
                if (d.Id_producto.HasValue)
                {
                    var producto = await _context.Productos
                        .FirstOrDefaultAsync(p => p.Id_Producto == d.Id_producto.Value);
                    if (producto != null)
                    {
                        producto.Stock += d.Cantidad;
                    }
                }
            }

            _context.DetallesVenta.RemoveRange(venta.Detalles);
            _context.Ventas.Remove(venta);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Helpers 
        private List<SelectListItem> GetMetodosPago() =>
            new() {
                new SelectListItem { Value = "Efectivo", Text = "Efectivo" },
                new SelectListItem { Value = "Débito", Text = "Débito" },
                new SelectListItem { Value = "Crédito", Text = "Crédito" },
                new SelectListItem { Value = "Transferencia", Text = "Transferencia" }
            };

        private List<SelectListItem> GetProductosSelectList() =>
            _context.Productos
                .Select(p => new SelectListItem
                {
                    Value = p.Id_Producto.ToString(),
                    Text = $"{p.Nombre} (Stock: {p.Stock} Precio: ${p.Precio} )"
                })
                .ToList();

        private List<SelectListItem> GetServiciosSelectList() =>
            _context.Servicio
                .Select(s => new SelectListItem
                {
                    Value = s.Id_Servicio.ToString(),
                    Text = $"{s.Nombre} ({s.Descripcion})"
                })
                .ToList();
    }
}