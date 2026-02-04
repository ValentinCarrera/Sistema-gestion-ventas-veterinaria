using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veterinaria_AppWeb.Data;
using Veterinaria_AppWeb.Models;
using Veterinaria_AppWeb.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace Veterinaria_AppWeb.Controllers
{
    public class ProductosController : Controller
    {
        private readonly VeterinariaContext _context;

        public ProductosController(VeterinariaContext context)
        {
            _context = context;
        }

        // GET: Producto
        public async Task<IActionResult> Index(string search)
        {
            var query = _context.Productos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p =>
                    p.Nombre.Contains(search) ||
                    p.Categoria.Contains(search));
            }

            var productos = await query
                .Select(p => new ProductoViewModel
                {
                    Id_Producto = p.Id_Producto,
                    Nombre = p.Nombre,
                    Categoria = p.Categoria,
                    Stock = p.Stock,
                    Precio = p.Precio
                })
                .ToListAsync();

            // Para mantener el texto en la caja de búsqueda
            ViewBag.Search = search;

            return View(productos);
        }

        // GET: Producto/Create
        public IActionResult Create()
        {
            return View(new ProductoViewModel());
        }

        // POST: Producto/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductoViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);

            var nombreNormalizado = viewModel.Nombre?.Trim().ToUpper();
            var existe = await _context.Productos
                                       .AnyAsync(p => p.Nombre.ToUpper() == nombreNormalizado);

            if (existe)
            {
                ModelState.AddModelError("Nombre", "Ya existe un producto con ese nombre.");
                return View(viewModel);
            }

            try
            {
                var producto = new Producto
                {
                    Nombre = viewModel.Nombre,
                    Categoria = viewModel.Categoria,
                    Stock = viewModel.Stock,
                    Precio = viewModel.Precio
                };

                _context.Productos.Add(producto);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("", $"Error al crear el producto: {ex.Message}");
                return View(viewModel);
            }
        }

        // GET: Producto/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            var viewModel = new ProductoViewModel
            {
                Id_Producto = producto.Id_Producto,
                Nombre = producto.Nombre,
                Categoria = producto.Categoria,
                Stock = producto.Stock,
                Precio = producto.Precio
            };

            return View(viewModel);
        }

        // POST: Producto/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductoViewModel viewModel)
        {
            if (id != viewModel.Id_Producto)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
                return View(viewModel);

            var nombreNormalizado = viewModel.Nombre?.Trim().ToUpper();
            var existe = await _context.Productos
                                       .AnyAsync(p => p.Id_Producto != id &&
                                                      p.Nombre.ToUpper() == nombreNormalizado);

            if (existe)
            {
                ModelState.AddModelError("Nombre", "Ya existe otro producto con ese nombre.");
                return View(viewModel);
            }

            try
            {
                var producto = await _context.Productos.FindAsync(id);
                if (producto == null)
                {
                    return NotFound();
                }

                producto.Nombre = viewModel.Nombre;
                producto.Categoria = viewModel.Categoria;
                producto.Stock = viewModel.Stock;
                producto.Precio = viewModel.Precio;

                _context.Productos.Update(producto);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // GET: Productos/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var producto = await _context.Productos.FirstOrDefaultAsync(p => p.Id_Producto == id);

            if (producto == null)
                return NotFound();

            var viewModel = new ProductoViewModel
            {
                Id_Producto = producto.Id_Producto,
                Nombre = producto.Nombre,
                Categoria = producto.Categoria,
                Stock = producto.Stock,
                Precio = producto.Precio
            };

            return View(viewModel);
        }

        // GET: Productos/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var producto = await _context.Productos.FirstOrDefaultAsync(m => m.Id_Producto == id);
            if (producto == null)
                return NotFound();

            var viewModel = new ProductoViewModel
            {
                Id_Producto = producto.Id_Producto,
                Nombre = producto.Nombre,
                Categoria = producto.Categoria,
                Stock = producto.Stock,
                Precio = producto.Precio
            };

            return View(viewModel);
        }

        // POST: Producto/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
                return NotFound();

            // 🔒 Verificar si el producto está asociado a alguna venta
            bool tieneVentas = _context.DetallesVenta.Any(d => d.Id_servicio == id);
            if (tieneVentas)
            {
                TempData["ErrorMessage"] = "⚠️ No se puede eliminar este producto porque ya fue utilizado en una o más ventas.";
                return RedirectToAction(nameof(Index));
            }

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "✅ Producto eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.Id_Producto == id);
        }


        // GET: Productos/Reponer
        public async Task<IActionResult> Reponer(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();

            var vm = new ReposicionStockViewModel
            {
                Id_Producto = producto.Id_Producto,
                Nombre = producto.Nombre,
                StockActual = producto.Stock
            };
            return View(vm);
        }

        // POST: Productos/Reponer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reponer(ReposicionStockViewModel vm)
        {
            // Repoblar campos que la vista necesita si volvemos a mostrarla
            if (!ModelState.IsValid)
            {
                // Traer nombre/stock actual por si cambiaron en BD 
                var productoTmp = await _context.Productos.FindAsync(vm.Id_Producto);
                if (productoTmp != null)
                {
                    vm.Nombre = productoTmp.Nombre;
                    vm.StockActual = productoTmp.Stock;
                }
                return View(vm);
            }

            var producto = await _context.Productos.FindAsync(vm.Id_Producto);
            if (producto == null) return NotFound();

            if (vm.Sumar)
            {
                producto.Stock += vm.Cantidad;
            }
            else
            {
                // Reemplaza el stock por la cantidad indicada
                producto.Stock = vm.Cantidad;
            }

            await _context.SaveChangesAsync();

            TempData["Msg"] = $"Se actualizó el stock de {producto.Nombre}. Stock actual: {producto.Stock}";
            return RedirectToAction(nameof(Index));
        }

    }
}