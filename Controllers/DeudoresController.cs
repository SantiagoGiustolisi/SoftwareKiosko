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
    public class DeudoresController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DeudoresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Deudores
        public async Task<IActionResult> Index()
        {
            return View(await _context.Deudor.ToListAsync());
        }

        // GET: Deudores/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deudores = await _context.Deudor
                .FirstOrDefaultAsync(m => m.Id == id);
            if (deudores == null)
            {
                return NotFound();
            }

            return View(deudores);
        }

        // GET: Deudores/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Deudores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,fechaVenta,Descripcion,dineroEntregado,dineroTotal,dineroResto")] Deudor deudores)
        {
            if (ModelState.IsValid)
            {
                deudores.dineroResto = deudores.dineroTotal - deudores.dineroEntregado;
                _context.Add(deudores);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(deudores);
        }

        // GET: Deudores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deudores = await _context.Deudor.FindAsync(id);
            if (deudores == null)
            {
                return NotFound();
            }
            return View(deudores);
        }

        // POST: Deudores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,fechaVenta,Descripcion,dineroEntregado,dineroTotal,dineroResto")] Deudor deudores)
        {
            if (id != deudores.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    deudores.dineroResto = deudores.dineroTotal - deudores.dineroEntregado;
                    _context.Update(deudores);
                    await _context.SaveChangesAsync();

                    // ✅ Si pagó todo, mostramos mensaje
                    if (deudores.dineroResto == 0)
                    {
                        TempData["DeudaPagadaMessage"] = $" El cliente {deudores.Nombre} ha realizado el pago total de su deuda.";
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeudoresExists(deudores.Id))
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

            return View(deudores);
        }


        // GET: Deudores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deudores = await _context.Deudor
                .FirstOrDefaultAsync(m => m.Id == id);
            if (deudores == null)
            {
                return NotFound();
            }

            return View(deudores);
        }

        // POST: Deudores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deudores = await _context.Deudor.FindAsync(id);
            if (deudores != null)
            {
                _context.Deudor.Remove(deudores);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DeudoresExists(int id)
        {
            return _context.Deudor.Any(e => e.Id == id);
        }
    }
}
