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
    public class LibrosAutorsController : Controller
    {
        private readonly LibrosbdContext _context;

        public LibrosAutorsController(LibrosbdContext context)
        {
            _context = context;
        }

        // GET: LibrosAutors
        public async Task<IActionResult> Index()
        {
            var librosbdContext = _context.LibrosAutors.Include(l => l.IdAutorNavigation).Include(l => l.IsbnNavigation);
            return View(await librosbdContext.ToListAsync());
        }

        // GET: LibrosAutors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var librosAutor = await _context.LibrosAutors
                .Include(l => l.IdAutorNavigation)
                .Include(l => l.IsbnNavigation)
                .FirstOrDefaultAsync(m => m.IdLibrosAutor == id);
            if (librosAutor == null)
            {
                return NotFound();
            }

            return View(librosAutor);
        }

        // GET: LibrosAutors/Create
        public IActionResult Create()
        {
            ViewData["IdAutor"] = new SelectList(_context.Autors, "IdAutor", "IdAutor");
            ViewData["Isbn"] = new SelectList(_context.Libros, "Isbn", "Isbn");
            return View();
        }

        // POST: LibrosAutors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Create([FromForm] LibrosAutor librosautor)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(librosautor);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Registro creado exitosamente" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Error al crear el registro: " + ex.Message });
                }
            }

            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return Json(new { success = false, message = "Datos inválidos", errors });
        }

        // GET: LibrosAutors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var librosAutor = await _context.LibrosAutors.FindAsync(id);
            if (librosAutor == null)
            {
                return NotFound();
            }
            ViewData["IdAutor"] = new SelectList(_context.Autors, "IdAutor", "IdAutor", librosAutor.IdAutor);
            ViewData["Isbn"] = new SelectList(_context.Libros, "Isbn", "Isbn", librosAutor.Isbn);
            return View(librosAutor);
        }

        // POST: LibrosAutors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Edit(int id, [FromForm] LibrosAutor librosAutor)
        {
            if (id != librosAutor.IdLibrosAutor) 
            {
                return Json(new { success = false, message = "ID no coincide" });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(librosAutor);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Registro actualizado exitosamente" });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LibrosAutorExists(librosAutor.IdLibrosAutor))
                    {
                        return Json(new { success = false, message = "Registro no encontrado" });
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
        //public async Task<IActionResult> Edit(int id, [Bind("IdLibrosAutor,IdAutor,Isbn")] LibrosAutor librosAutor)
        //{
        //    if (id != librosAutor.IdLibrosAutor)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            //Mensaje de éxito
        //            _context.Update(librosAutor);
        //            await _context.SaveChangesAsync();
        //            TempData["Mensaje"] = "¡El registro se ha actualizado exitosamente!";
        //            return RedirectToAction(nameof(Index));

        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!LibrosAutorExists(librosAutor.IdLibrosAutor))
        //            {
        //                //Retornar error
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //    }
        //    ViewData["IdAutor"] = new SelectList(_context.Autors, "IdAutor", "IdAutor", librosAutor.IdAutor);
        //    ViewData["Isbn"] = new SelectList(_context.Libros, "Isbn", "Isbn", librosAutor.Isbn);
        //    return View(librosAutor);
        //}

        // GET: LibrosAutors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var librosAutor = await _context.LibrosAutors
                .Include(l => l.IdAutorNavigation)
                .Include(l => l.IsbnNavigation)
                .FirstOrDefaultAsync(m => m.IdLibrosAutor == id);
            if (librosAutor == null)
            {
                return NotFound();
            }

            return View(librosAutor);
        }

        // POST: LibrosAutors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var librosAutor = await _context.LibrosAutors.FindAsync(id);
                if (librosAutor != null)
                {
                    //Mensaje de éxito
                    _context.LibrosAutors.Remove(librosAutor);
                    await _context.SaveChangesAsync();
                    TempData["Mensaje"] = "Registro eliminado con éxito";
                    return RedirectToAction(nameof(Index));
                }
                return NotFound();
            }
            catch (DbUpdateException)
            {
                //Mensaje de error
                TempData["ErrorMessage"] = "No se puede eliminar este registro porque está siendo utilizado en otras tablas.";
                return RedirectToAction("Delete", new { id = id });
            }


        }

        private bool LibrosAutorExists(int id)
        {
            return _context.LibrosAutors.Any(e => e.IdLibrosAutor == id);
        }
    }
}
