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
    public class ProveedoresController : Controller
    {
		private readonly ApplicationDbContext _context;
		private readonly IWebHostEnvironment env;
		public ProveedoresController(ApplicationDbContext context, IWebHostEnvironment env)
		{
			_context = context;
			this.env = env;
		}

		// GET: Proveedores
		public async Task<IActionResult> Index()
        {
            return View(await _context.Proveedor.ToListAsync());
        }

        // GET: Proveedores/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proveedor = await _context.Proveedor
                .FirstOrDefaultAsync(m => m.Id == id);
            if (proveedor == null)
            {
                return NotFound();
            }

            return View(proveedor);
        }

        // GET: Proveedores/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Proveedores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NombreProveedor,Telefono")] Proveedor proveedor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(proveedor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(proveedor);
        }

        // GET: Proveedores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proveedor = await _context.Proveedor.FindAsync(id);
            if (proveedor == null)
            {
                return NotFound();
            }
            return View(proveedor);
        }

        // POST: Proveedores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NombreProveedor,Telefono")] Proveedor proveedor)
        {
            if (id != proveedor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(proveedor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProveedorExists(proveedor.Id))
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
            return View(proveedor);
        }

		public async Task<IActionResult> Importar()
		{
			var archivos = HttpContext.Request.Form.Files;
			if (archivos != null && archivos.Count > 0)
			{
				var archivo = archivos[0];
				if (archivo.Length > 0)
				{
					var pathDestino = Path.Combine(env.WebRootPath, "Importaciones");
					var archivoDestino = Guid.NewGuid().ToString();
					archivoDestino = archivoDestino.Replace("-", "");
					archivoDestino += Path.GetExtension(archivo.FileName);
					var rutaDestino = Path.Combine(pathDestino, archivoDestino);
					using (var filestream = new FileStream(rutaDestino, FileMode.Create))
					{
						archivo.CopyTo(filestream);
					}
					using (var file = new FileStream(rutaDestino, FileMode.Open))
					{
						List<string> renglones = new List<string>();
						List<Proveedor> DeporteArch = new List<Proveedor>();
						StreamReader fileContent = new StreamReader(file, System.Text.Encoding.Default);
						do
						{
							renglones.Add(fileContent.ReadLine());
						} while (!fileContent.EndOfStream);

						if (renglones.Count > 0)
						{
							foreach (var renglon in renglones)
							{
								string[] data = renglon.Split(";");
								if (data.Length == 2)
								{
									Proveedor proveedor = new Proveedor();
									proveedor.NombreProveedor = data[0].Trim();
									proveedor.Telefono = data[1].Trim();
									DeporteArch.Add(proveedor);
								}
							}
							if (DeporteArch.Count > 0)
							{
								_context.AddRange(DeporteArch);
								await _context.SaveChangesAsync();
							}
						}
					}
				}
			}
			var deportes = await _context.Proveedor.ToListAsync();

			return View("Index", deportes);  // Cambié aquí para enviar la lista directamente
		}

           public IActionResult SoporteTecnico(string msj, string numero)
    {
        // Creamos la URL de WhatsApp
        string urlWhatsApp = $"https://api.whatsapp.com/send?phone={numero}&text={Uri.EscapeDataString(msj)}";
    
        // En vez de hacer un redirect, solo pasamos la URL a la vista
        TempData["WhatsAppUrl"] = urlWhatsApp;

        // Luego redirigimos a la vista donde lo vamos a manejar
        return RedirectToAction("Index");  // O la vista que desees
    }




        // GET: Proveedores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proveedor = await _context.Proveedor
                .FirstOrDefaultAsync(m => m.Id == id);
            if (proveedor == null)
            {
                return NotFound();
            }

            return View(proveedor);
        }

        // POST: Proveedores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var proveedor = await _context.Proveedor.FindAsync(id);
            if (proveedor != null)
            {
                _context.Proveedor.Remove(proveedor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProveedorExists(int id)
        {
            return _context.Proveedor.Any(e => e.Id == id);
        }
    }
}
