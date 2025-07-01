using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ZXing;
using ZXing.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal_Santiago_Giustolisi.Data;
using ProyectoFinal_Santiago_Giustolisi.Models;
using ZXing.QrCode;
using ZXing.Windows.Compatibility;


namespace ProyectoFinal_Santiago_Giustolisi.Controllers
{
    public class StockProductosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment env;

        public StockProductosController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            this.env = env;
        }

        // GET: StockProductos
        [Authorize]
        public async Task<IActionResult> Index(string? busquedaNombre, int paginaActual = 1)
        {
            int tamañoPagina = 3;

            var appDBcontexto = _context.StockProducto
                                        .Include(s => s.categoria);

            var query = appDBcontexto.AsQueryable();

            if (!string.IsNullOrEmpty(busquedaNombre))
            {
                query = query.Where(s => s.Nombre.Contains(busquedaNombre));
            }

            var totalElementos = await query.CountAsync();

            var totalPaginas = (int)Math.Ceiling(totalElementos / (double)tamañoPagina);

            var productosPagina = await query
                .Skip((paginaActual - 1) * tamañoPagina)
                .Take(tamañoPagina)
                .ToListAsync();

            var viewModel = new StockProductoViewModels
            {
                productos = productosPagina,
                PaginaActual = paginaActual,
                TotalPaginas = totalPaginas,
                BusquedaNombre = busquedaNombre
            };

            return View(viewModel);
        }

        public IActionResult CodigoBarras(int id)
        {
            var producto = _context.StockProducto.Find(id);
            if (producto == null)
                return NotFound();

            string textoCodigo = $"{producto.Nombre} - ${producto.Precio}";

            var writer = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions
                {
                    Height = 100,
                    Width = 300,
                    Margin = 10
                }
            };

            var pixelData = writer.Write(textoCodigo);

            using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppRgb))
            {
                var bitmapData = bitmap.LockBits(new Rectangle(0, 0, pixelData.Width, pixelData.Height),
                    ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);

                try
                {
                    System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                }
                finally
                {
                    bitmap.UnlockBits(bitmapData);
                }

                using (var ms = new MemoryStream())
                {
                    bitmap.Save(ms, ImageFormat.Png);
                    return File(ms.ToArray(), "image/png");
                }
            }
        }


        // GET: StockProductos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stockProducto = await _context.StockProducto
                .Include(s => s.categoria)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (stockProducto == null)
            {
                return NotFound();
            }

            return View(stockProducto);
        }

        // GET: StockProductos/Create
        public IActionResult Create()
        {
            ViewData["categoriaId"] = new SelectList(_context.Categoria, "Id", "NombreCategoria");
            return View();
        }

        // POST: StockProductos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,categoriaId,Nombre,Cantidad,Precio,Foto")] StockProducto modelo)
        {
            if (modelo.Foto == null)
            {
                modelo.Foto = "Sin foto";
            }
            if (ModelState.IsValid)
            {
                var archivos = HttpContext.Request.Form.Files;
                if (archivos != null && archivos.Count > 0)
                {
                    var archivofoto = archivos[0];
                    if (archivofoto.Length > 0)
                    {
                        var pathDestino = Path.Combine(env.WebRootPath, "images\\productos");
                        var archivoDestino = Guid.NewGuid().ToString();
                        archivoDestino = archivoDestino.Replace("-", "");
                        archivoDestino += Path.GetExtension(archivofoto.FileName);
                        var rutaDestino = Path.Combine(pathDestino, archivoDestino);
                        using (var filestream = new FileStream(rutaDestino, FileMode.Create))
                        {
                            archivofoto.CopyTo(filestream);
                            modelo.Foto = archivoDestino;
                        }
                    }
                }

                _context.Add(modelo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["categoriaId"] = new SelectList(_context.Categoria, "Id", "NombreCategoria", modelo.categoriaId);
            return View(modelo);
        }

        // GET: StockProductos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stockProducto = await _context.StockProducto.FindAsync(id);
            if (stockProducto == null)
            {
                return NotFound();
            }
            ViewData["categoriaId"] = new SelectList(_context.Categoria, "Id", "NombreCategoria", stockProducto.categoriaId);
            return View(stockProducto);
        }

        // POST: StockProductos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,categoriaId,Nombre,Cantidad,Precio,Foto")] StockProducto modelo)
        {
            if (id != modelo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var archivos = HttpContext.Request.Form.Files;
                if (archivos != null && archivos.Count > 0)
                {
                    var archivofoto = archivos[0];
                    if (archivofoto.Length > 0)
                    {
                        var pathDestino = Path.Combine(env.WebRootPath, "images\\productos");
                        var archivoDestino = Guid.NewGuid().ToString();
                        archivoDestino = archivoDestino.Replace("-", "");
                        archivoDestino += Path.GetExtension(archivofoto.FileName);
                        var rutaDestino = Path.Combine(pathDestino, archivoDestino);
                        if (!string.IsNullOrEmpty(modelo.Foto))
                        {
                            string fotoAnterior = Path.Combine(pathDestino, modelo.Foto);
                            if (System.IO.File.Exists(fotoAnterior))
                                System.IO.File.Delete(fotoAnterior);
                        }

                        using (var filestream = new FileStream(rutaDestino, FileMode.Create))
                        {
                            archivofoto.CopyTo(filestream);
                            modelo.Foto = archivoDestino;
                        }
                    }
                }
                try
                {
                    _context.Update(modelo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StockProductoExists(modelo.Id))
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
            ViewData["categoriaId"] = new SelectList(_context.Categoria, "Id", "NombreCategoria", modelo.categoriaId);
            return View(modelo);
        }

        // GET: StockProductos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stockProducto = await _context.StockProducto
                .Include(s => s.categoria)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (stockProducto == null)
            {
                return NotFound();
            }

            return View(stockProducto);
        }

        // POST: StockProductos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.StockProducto.FindAsync(id);

            if (producto != null)
            {
                // Eliminar ingresos relacionados
                var ingresos = _context.Ingreso.Where(i => i.productoId == producto.Id).ToList();
                _context.Ingreso.RemoveRange(ingresos);

                // Eliminar ventas relacionadas
                var ventas = _context.Venta.Where(v => v.productoId == producto.Id).ToList();
                _context.Venta.RemoveRange(ventas);

                // Eliminar el producto
                _context.StockProducto.Remove(producto);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool StockProductoExists(int id)
        {
            return _context.StockProducto.Any(e => e.Id == id);
        }

        // NUEVO: GET para mostrar formulario de carga de imagen para leer código de barras
        [HttpGet]
        public IActionResult LeerCodigoBarras()
        {
            return View();
        }

        // NUEVO: POST para procesar la imagen y leer el código de barras
        [HttpPost]
        public async Task<IActionResult> LeerCodigoBarras(IFormFile imagen)
        {
            if (imagen == null || imagen.Length == 0)
            {
                ModelState.AddModelError("", "No se subió ninguna imagen.");
                return View();
            }

            using (var stream = imagen.OpenReadStream())
            {
                using (var bitmap = new Bitmap(stream))
                {
                    var reader = new BarcodeReader();   

                    var result = reader.Decode(bitmap);

                    if (result != null)
                    {
                        ViewBag.CodigoLeido = result.Text;
                        return View();
                    }
                    else
                    {
                        ModelState.AddModelError("", "No se pudo leer ningún código de barras.");
                        return View();
                    }
                }
            }
        }

    }
}
