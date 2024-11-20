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
    public class AutorsController : Controller
    {
        private readonly LibrosbdContext _context;

        public AutorsController(LibrosbdContext context)
        {
            _context = context;
        }

        // GET: Autors
        public async Task<IActionResult> Index(string buscar, string filtro)
        {
            var autores = from Autors in _context.Autors select Autors;
            if (!String.IsNullOrEmpty(buscar))
            {
                autores = autores.Where(s => s.Nombre!.Contains(buscar));
            }

            ViewData["FiltroNombre"] = String.IsNullOrEmpty(filtro) ? "NombreDescendente" : "";


            switch (filtro)
            {
                case "NombreDescendente":
                    autores = autores.OrderByDescending(autor => autor.Nombre);
                    break;


                default:
                    autores = autores.OrderBy(autor => autor.Nombre);
                    break;

            }

            return View(await autores.ToListAsync());
        }

        // GET: Autors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var autor = await _context.Autors
                .FirstOrDefaultAsync(m => m.IdAutor == id);
            if (autor == null)
            {
                return NotFound();
            }

            return View(autor);
        }

        // GET: Autors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Autors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Create([FromForm] Autor autor)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(autor);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Autor creado exitosamente" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Error al crear el autor: " + ex.Message });
                }
            }

            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return Json(new { success = false, message = "Datos inválidos", errors });
        }

        // GET: Autors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var autor = await _context.Autors.FindAsync(id);
            if (autor == null)
            {
                return NotFound();
            }
            return View(autor);
        }

        // POST: Autors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Edit(int id, [FromForm] Autor autor)
        {
            if (id != autor.IdAutor)
            {
                return Json(new { success = false, message = "ID no coincide" });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(autor);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Autor actualizado exitosamente" });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AutorExists(autor.IdAutor))
                    {
                        return Json(new { success = false, message = "Autor no encontrado" });
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

        // GET: Autors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var autor = await _context.Autors
                .FirstOrDefaultAsync(m => m.IdAutor == id);
            if (autor == null)
            {
                return NotFound();
            }

            return View(autor);
        }

        // POST: Autors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var autor = await _context.Autors.FindAsync(id);
                if (autor != null)
                {
                    _context.Autors.Remove(autor);
                    await _context.SaveChangesAsync();

                    // Use TempData to pass success message
                    return Json(new { success = true, message = "El autor se eliminó con éxito." });
                }
                return NotFound();
            }
            catch (DbUpdateException)
            {
                // Use TempData to pass error message
                TempData["ErrorMessage"] = "No se puede eliminar este autor porque está siendo utilizado en otras tablas.";
                return RedirectToAction("Delete", new { id = id });
            }
        }

        private bool AutorExists(int id)
        {
            return _context.Autors.Any(e => e.IdAutor == id);
        }
    }
}
