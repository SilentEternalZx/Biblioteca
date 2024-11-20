using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CRUDJulianCarmonaEF.Models;

namespace CRUDJulianCarmonaEF.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly LibrosbdContext _context;

        public CategoriasController(LibrosbdContext context)
        {
            _context = context;
        }

        // GET: Categorias
        public async Task<IActionResult> Index(string buscar, string filtro)
        {
            var categorias = from Categorias in _context.Categorias select Categorias;
            if (!String.IsNullOrEmpty(buscar))
            {
                categorias = categorias.Where(s => s.Nombre!.Contains(buscar));
            }

            ViewData["FiltroNombre"] = String.IsNullOrEmpty(filtro) ? "NombreDescendente" : "";


            switch (filtro)
            {
                case "NombreDescendente":
                    categorias = categorias.OrderByDescending(categoria => categoria.Nombre);
                    break;


                default:
                    categorias = categorias.OrderBy(categoria => categoria.Nombre);
                    break;

            }

            return View(await categorias.ToListAsync());
        }

        // GET: Categorias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(m => m.CodigoCategoria == id);
            if (categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
        }

        // GET: Categorias/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categorias/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Create([FromForm] Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        //verificar si ya existe el código
                        if (CategoriaExists(categoria.CodigoCategoria))
                        {
                            return Json(new { success = false, message = "El código de categoría ya existe" });
                        }
                        //verificar si el nombre ya existe
                        if (_context.Categorias.Any(c => c.Nombre == categoria.Nombre))
                        {
                            return Json(new { success = false, message = "Ya existe una categoría con ese nombre" });
                        }

                        _context.Add(categoria);
                        await _context.SaveChangesAsync();
                        return Json(new { success = true, message = "Categoría creada exitosamente" });
                    }



                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Error al crear la categoría: " + ex.Message });
                }
            }
            //error de validación
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return Json(new { success = false, message = "Datos inválidos", errors });
        }

        // GET: Categorias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
            {
                return NotFound();
            }
            return View(categoria);
        }

        // POST: Categorias/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Edit(int id, [FromForm] Categoria categoria)
        {
            if (id != categoria.CodigoCategoria)
            {
                return Json(new { success = false, message = "ID no coincide" });
            }

            if (ModelState.IsValid)
            {
                try
                {

                    if (ModelState.IsValid)
                    {
                        if (_context.Categorias.Any(c => c.Nombre == categoria.Nombre))
                        {
                            return Json(new { success = false, message = "Ya existe una categoría con ese nombre" });
                        }
                        _context.Update(categoria);
                        await _context.SaveChangesAsync();
                        return Json(new { success = true, message = "Categoría actualizada exitosamente" });

                    }


                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoriaExists(categoria.CodigoCategoria))
                    {
                        return Json(new { success = false, message = "Categoría no encontrada" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Error de concurrencia" });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Error al actualizar: " + ex.Message });
                }
            }

            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return Json(new { success = false, message = "Datos inválidos", errors });
        }

        // GET: Categorias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(m => m.CodigoCategoria == id);
            if (categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
        }

        // POST: Categorias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            try
            {
                var categoria = await _context.Categorias.FindAsync(id);
                if (categoria != null)
                {
                    //Mensaje de éxito
                    _context.Categorias.Remove(categoria);
                    await _context.SaveChangesAsync();
                    TempData["Mensaje"] = "Categoría eliminada con éxito.";
                    return RedirectToAction(nameof(Index));
                }
                return NotFound();
            }
            catch (DbUpdateException)
            {
                //Mensaje de error
                TempData["ErrorMessage"] = "No se puede eliminar esta categoría porque está siendo utilizada en otras tablas.";
                return RedirectToAction("Delete", new { id = id });
            }

        }

        private bool CategoriaExists(int id)
        {
            return _context.Categorias.Any(e => e.CodigoCategoria == id);
        }
    }
}
