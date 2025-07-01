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
    public class IngresosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment env;
        public IngresosController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            this.env = env;
        }
        // GET: Ingresos
        public async Task<IActionResult> Index()
        {
            var ingresos = await _context.Ingreso.Include(i => i.stockProductos).ToListAsync();

            return View(await _context.Ingreso.ToListAsync());
        }

        // GET: Ingresos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingreso = await _context.Ingreso
                .Include(v => v.stockProductos)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ingreso == null)
            {
                return NotFound();
            }

            return View(ingreso);
        }

        // GET: Ingresos/Create
        public IActionResult Create()
        {
            var productos = _context.StockProducto.ToList();

            ViewBag.Productos = new SelectList(productos, "Id", "Nombre");

            return View();
        }

        // POST: Ingresos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,productoId,Cantidad,fechaIngreso")] Ingreso ingresos)
        {
            if (ModelState.IsValid)
            {
                // Verificar que el reloj existe
                var producto = await _context.StockProducto.FindAsync(ingresos.productoId);
                if (producto == null)
                {
                    ModelState.AddModelError("productoId", "El producto seleccionado no es válido.");
                }
                else
                {
                    ingresos.stockProductos = producto;

                    producto.Cantidad += ingresos.Cantidad;
                    _context.Update(producto);

                    _context.Add(ingresos);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"¡Ingreso registrado con éxito! \nSe realizó el ingreso de {ingresos.Cantidad} unidad(es) de {producto.Nombre}";
                    return RedirectToAction(nameof(Index));
                }
            }

            var allProductos = await _context.StockProducto.ToListAsync();
            ViewBag.Productos = new SelectList(allProductos, "Id", "Nombre");
            return View(ingresos);
        }

        // GET: Ingresos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingresos = await _context.Ingreso.FindAsync(id);
            if (ingresos == null)
            {
                return NotFound();
            }

            var allProductos = await _context.StockProducto.ToListAsync();
            ViewBag.Productos = new SelectList(allProductos, "Id", "Nombre", ingresos.productoId);

            return View(ingresos);
        }

        // POST: Ingresos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, [Bind("Id,productoId,Cantidad,fechaIngreso")] Ingreso ingresos)
{
    if (id != ingresos.Id)
    {
        return NotFound();
    }

    if (ModelState.IsValid)
    {
        try
        {
            var existingIngreso = await _context.Ingreso.FirstOrDefaultAsync(i => i.Id == id);
            if (existingIngreso == null)
            {
                return NotFound();
            }

            // Verificamos si se cambió el producto
            if (existingIngreso.productoId != ingresos.productoId)
            {
                // Devolver la cantidad al producto anterior
                var oldProducto = await _context.StockProducto.FindAsync(existingIngreso.productoId);
                if (oldProducto != null)
                {
                    oldProducto.Cantidad -= existingIngreso.Cantidad;
                    _context.Update(oldProducto);
                }

                // Agregar la cantidad al nuevo producto
                var newProducto = await _context.StockProducto.FindAsync(ingresos.productoId);
                if (newProducto != null)
                {
                    newProducto.Cantidad += ingresos.Cantidad;
                    _context.Update(newProducto);
                }
            }
            else
            {
                // Mismo producto, ajustar stock según diferencia de cantidades
                var producto = await _context.StockProducto.FindAsync(ingresos.productoId);
                if (producto != null)
                {
                    producto.Cantidad += ingresos.Cantidad - existingIngreso.Cantidad;
                    _context.Update(producto);
                }
            }

            // Actualizar ingreso
            existingIngreso.productoId = ingresos.productoId;
            existingIngreso.Cantidad = ingresos.Cantidad;
            existingIngreso.fechaIngreso = ingresos.fechaIngreso;

            _context.Update(existingIngreso);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!IngresosExists(ingresos.Id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
        return RedirectToAction(nameof(Index));
    }

    var allProductos = await _context.StockProducto.ToListAsync();
    ViewBag.Productos = new SelectList(allProductos, "Id", "Nombre", ingresos.productoId);
    return View(ingresos);
}
    

        // GET: Ingresos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingresos = await _context.Ingreso
                .Include(v => v.stockProductos)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ingresos == null)
            {
                return NotFound();
            }

            return View(ingresos);
        }

        // POST: Ingresos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ingresos = await _context.Ingreso.FindAsync(id);
            if (ingresos != null)
            {
                _context.Ingreso.Remove(ingresos);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IngresosExists(int id)
        {
            return _context.Ingreso.Any(e => e.Id == id);
        }
    }
}

