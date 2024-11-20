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
    public class EditorialesController : Controller
    {
        private readonly LibrosbdContext _context;

        public EditorialesController(LibrosbdContext context)
        {
            _context = context;
        }

        // GET: Editoriales
        public async Task<IActionResult> Index(string buscar, string filtro)
        {
            var editoriales = from Editoriales in _context.Editoriales select Editoriales;
            if (!String.IsNullOrEmpty(buscar))
            {
                editoriales = editoriales.Where(s => s.Nombres!.Contains(buscar));
            }

            ViewData["FiltroNombre"] = String.IsNullOrEmpty(filtro) ? "NombreDescendente" : "";


            switch (filtro)
            {
                case "NombreDescendente":
                    editoriales = editoriales.OrderByDescending(editorial => editorial.Nombres);
                    break;


                default:
                    editoriales = editoriales.OrderBy(editorial => editorial.Nombres);
                    break;

            }

            return View(await editoriales.ToListAsync());
        }

        // GET: Editoriales/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var editoriale = await _context.Editoriales
                .FirstOrDefaultAsync(m => m.Nit == id);
            if (editoriale == null)
            {
                return NotFound();
            }

            return View(editoriale);
        }

        // GET: Editoriales/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Editoriales/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Create([FromForm] Editoriale editorial)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (ModelState.IsValid)
                    {
                        //verificar si ya existe el código
                        if (EditorialeExists(editorial.Nit))
                        {
                            return Json(new { success = false, message = "El NIT ya existe" });
                        }
                        //verificar si el nombre ya existe
                        if (_context.Editoriales.Any(c => c.Nombres == editorial.Nombres))
                        {
                            return Json(new { success = false, message = "Ya existe una categoría con ese nombre" });

                        }
                        if (_context.Editoriales.Any(c => c.Email == editorial.Email))
                        {
                            return Json(new { success = false, message = "Ese email ya está registrado" });
                        }
                        if (_context.Editoriales.Any(c => c.Direccion == editorial.Direccion))
                        {
                            return Json(new { success = false, message = "Esa dirección ya está registrada" });
                        }
                        if (_context.Editoriales.Any(c => c.Direccion == editorial.Direccion))
                        {
                            return Json(new { success = false, message = "Ese teléfono ya está registrado" });
                        }

                        _context.Add(editorial);
                        await _context.SaveChangesAsync();
                        return Json(new { success = true, message = "Editorial creada exitosamente" });
                    }
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al crear la editorial: " + ex.Message });
            }

            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return Json(new { success = false, message = "Datos inválidos", errors });
        }

        // GET: Editoriales/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var editoriale = await _context.Editoriales.FindAsync(id);
            if (editoriale == null)
            {
                return NotFound();
            }
            return View(editoriale);
        }

        // POST: Editoriales/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Edit(int id, [FromForm] Editoriale editorial)
        {
            if (id != editorial.Nit)
            {
                return Json(new { success = false, message = "ID no coincide" });
            }

            if (ModelState.IsValid)
            {
                try
                {

                    if (ModelState.IsValid)
                    {
                        if (_context.Editoriales.Any(c => c.Nombres == editorial.Nombres))
                        {
                            return Json(new { success = false, message = "Ya existe una categoría con ese nombre" });
                        }
                        _context.Update(editorial);
                        await _context.SaveChangesAsync();
                        return Json(new { success = true, message = "Categoría actualizada exitosamente" });

                    }


                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EditorialeExists(editorial.Nit))
                    {
                        return Json(new { success = false, message = "Editorial no encontrada" });
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

        // GET: Editoriales/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var editoriale = await _context.Editoriales
                .FirstOrDefaultAsync(m => m.Nit == id);
            if (editoriale == null)
            {
                return NotFound();
            }

            return View(editoriale);
        }

        // POST: Editoriales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {

                var editoriale = await _context.Editoriales.FindAsync(id);
                if (editoriale != null)
                {
                    //Mensaje de éxito
                    _context.Editoriales.Remove(editoriale);
                    await _context.SaveChangesAsync();
                    TempData["Mensaje"] = "Editorial eliminada con éxito.";
                    return RedirectToAction(nameof(Index));
                }
                return NotFound();
            }

            catch (DbUpdateException)
            {
                //Mensaje de error
                TempData["ErrorMessage"] = "No se puede eliminar esta editorial porque está siendo utilizada en otras tablas.";
                return RedirectToAction("Delete", new { id = id });
            }
        }

        private bool EditorialeExists(int id)
        {
            return _context.Editoriales.Any(e => e.Nit == id);
        }
    }
}
