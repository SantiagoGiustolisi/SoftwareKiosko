using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal_Santiago_Giustolisi.Data;
using ProyectoFinal_Santiago_Giustolisi.Models;

namespace ProyectoFinal_Santiago_Giustolisi.Controllers
{
    public class VentasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment env;

        public VentasController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            this.env = env;
        }

        // GET: Ventas
        public async Task<IActionResult> Index()
        {
            var ventas = await _context.Venta.Include(i => i.stockProductos).ToListAsync();
            return View(ventas);
        }

        // GET: Ventas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var venta = await _context.Venta
                .Include(v => v.stockProductos)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (venta == null)
                return NotFound();

            return View(venta);
        }

        // GET: Ventas/Create
        // GET: Ventas/Create
        public IActionResult Create(int? productoId)
        {
            var productos = _context.StockProducto.ToList();

            if (productos.Count == 0)
            {
                TempData["Error"] = "Debés cargar productos antes de realizar una venta.";
                return RedirectToAction("Index", "StockProducto");
            }

            var venta = new Venta();

            if (productoId.HasValue)
            {
                var producto = productos.FirstOrDefault(p => p.Id == productoId.Value);
                if (producto == null)
                    return NotFound();

                venta.productoId = producto.Id;
            }

            // Aquí creamos SelectListItems deshabilitando productos sin stock
            ViewBag.Producto = productos.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Cantidad > 0 ? p.Nombre : $"❌ {p.Nombre} (Sin stock)",
                Disabled = p.Cantidad <= 0,
                Selected = p.Id == venta.productoId
            }).ToList();

            return View(venta);
        }


        // POST: Ventas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,productoId,Cantidad,precioIngreso,precioEgreso,Margen,Cliente,fechaVenta")] Venta venta)
        {
            if (ModelState.IsValid)
            {
                var producto = await _context.StockProducto.FindAsync(venta.productoId);
                if (producto == null)
                {
                    ModelState.AddModelError("productoId", "El producto seleccionado no es válido.");
                    return View(venta);
                }

                if (producto.Cantidad < venta.Cantidad)
                {
                    ModelState.AddModelError("cantidad", "No hay suficiente stock disponible.");
                    ViewBag.Producto = new SelectList(await _context.StockProducto.ToListAsync(), "Id", "Nombre");
                    return View(venta);
                }

                if (venta.Cantidad == 0)
                {
                    ModelState.AddModelError("cantidad", "Debe ingresar un número mayor a 0.");
                    ViewBag.Producto = new SelectList(await _context.StockProducto.ToListAsync(), "Id", "Nombre");
                    return View(venta);
                }

                venta.stockProductos = producto;
                producto.Cantidad -= venta.Cantidad;
                venta.Margen = venta.precioEgreso - venta.precioIngreso;

                _context.Add(venta);
                await _context.SaveChangesAsync();

                // Mensaje con cantidad y nombre de producto
                TempData["Success"] = $"¡Venta registrada con éxito! \nSe realizó la venta de {venta.Cantidad} unidad(es) de {producto.Nombre}'.";

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Producto = new SelectList(await _context.StockProducto.ToListAsync(), "Id", "Nombre", venta.productoId);
            return View(venta);
        }


        // GET: Ventas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var ventas = await _context.Venta.FindAsync(id);
            if (ventas == null)
                return NotFound();

            var allProductos = await _context.StockProducto.ToListAsync();
            ViewBag.Producto = new SelectList(allProductos, "Id", "Nombre", ventas.productoId);
            return View(ventas);
        }

        // POST: Ventas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,productoId,Cantidad,precioIngreso,precioEgreso,Margen,Cliente,fechaVenta")] Venta venta)
        {
            if (id != venta.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var ventaOriginal = await _context.Venta.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);
                    if (ventaOriginal == null)
                        return NotFound();

                    var stockProducto = await _context.StockProducto.FindAsync(venta.productoId);
                    if (stockProducto == null)
                    {
                        ModelState.AddModelError("productoId", "El producto seleccionado no es válido.");
                        return View(venta);
                    }

                    stockProducto.Cantidad += ventaOriginal.Cantidad; // Reponer stock anterior
                    stockProducto.Cantidad -= venta.Cantidad;         // Restar nuevo stock

                    if (stockProducto.Cantidad < 0)
                    {
                        ModelState.AddModelError("Cantidad", "No hay suficiente stock disponible.");
                        ViewBag.Producto = new SelectList(await _context.StockProducto.ToListAsync(), "Id", "Nombre", venta.productoId);
                        return View(venta);
                    }

                    _context.Update(stockProducto);
                    venta.Margen = venta.precioEgreso - venta.precioIngreso;
                    _context.Update(venta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VentaExists(venta.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Producto = new SelectList(await _context.StockProducto.ToListAsync(), "Id", "Nombre", venta.productoId);
            return View(venta);
        }

        public IActionResult GenerarTicket(int id)
        {
            var venta = _context.Venta
                        .Include(v => v.stockProductos)
                        .FirstOrDefault(v => v.Id == id);

            if (venta == null)
                return NotFound();

            return View(venta);
        }


        // GET: Ventas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var venta = await _context.Venta
                .Include(v => v.stockProductos)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (venta == null)
                return NotFound();

            return View(venta);
        }

        // POST: Ventas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venta = await _context.Venta
                .Include(v => v.stockProductos)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venta != null)
            {
                var producto = venta.stockProductos;
                if (producto != null)
                {
                    producto.Cantidad += venta.Cantidad;
                    _context.Update(producto);
                }

                _context.Venta.Remove(venta);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        private bool VentaExists(int id)
        {
            return _context.Venta.Any(e => e.Id == id);
        }
    }
}
