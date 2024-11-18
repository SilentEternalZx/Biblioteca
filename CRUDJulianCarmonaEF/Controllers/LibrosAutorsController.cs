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
        public async Task<IActionResult> Create([Bind("IdLibrosAutor,IdAutor,Isbn")] LibrosAutor librosAutor)
        {
            if (ModelState.IsValid)
            {

                try
                {

                    _context.Add(librosAutor);
                    await _context.SaveChangesAsync();
                    TempData["Mensaje"] = "¡El registro se ha creado exitosamente!";
                    TempData["TipoMensaje"] = "success";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    // Verificar si es un error de llave primaria duplicada
                    if (ex.InnerException != null &&
                        (ex.InnerException.Message.Contains("PRIMARY KEY") ||
                         ex.InnerException.Message.Contains("UNIQUE KEY") ||
                         ex.InnerException.Message.Contains("duplicate key")))
                    {
                        //Mensaje de alerta en caso de ingresar un id ya existente
                        ModelState.AddModelError("IdLibrosAutor",
                            "Ya existe un registro con este ID. Por favor, ingrese un ID diferente.");
                        TempData["Mensaje"] = "Error: El ID ingresado ya está registrado en el sistema.";
                        TempData["TipoMensaje"] = "error";
                    }
                    else
                    {
                        //Mensaje de error
                        ModelState.AddModelError(string.Empty,
                            "Ha ocurrido un error al crear el registro. Por favor, intente nuevamente.");
                        TempData["Mensaje"] = "Error al crear el registro.";
                        TempData["TipoMensaje"] = "error";
                    }
                }
                catch (Exception ex)
                {
                    //Mensaje de error
                    ModelState.AddModelError(string.Empty,
                        "Ha ocurrido un error inesperado. Por favor, intente nuevamente.");
                    TempData["Mensaje"] = "Error inesperado al crear el registro.";
                    TempData["TipoMensaje"] = "error";
                }
            }
            ViewData["IdAutor"] = new SelectList(_context.Autors, "IdAutor", "IdAutor", librosAutor.IdAutor);
            ViewData["Isbn"] = new SelectList(_context.Libros, "Isbn", "Isbn", librosAutor.Isbn);
            return View(librosAutor);
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
        public async Task<IActionResult> Edit(int id, [Bind("IdLibrosAutor,IdAutor,Isbn")] LibrosAutor librosAutor)
        {
            if (id != librosAutor.IdLibrosAutor)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //Mensaje de éxito
                    _context.Update(librosAutor);
                    await _context.SaveChangesAsync();
                    TempData["Mensaje"] = "¡El registro se ha actualizado exitosamente!";
                    return RedirectToAction(nameof(Index));

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LibrosAutorExists(librosAutor.IdLibrosAutor))
                    {
                        //Retornar error
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewData["IdAutor"] = new SelectList(_context.Autors, "IdAutor", "IdAutor", librosAutor.IdAutor);
            ViewData["Isbn"] = new SelectList(_context.Libros, "Isbn", "Isbn", librosAutor.Isbn);
            return View(librosAutor);
        }

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
