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
    public class LibroesController : Controller
    {
        private readonly LibrosbdContext _context;

        public LibroesController(LibrosbdContext context)
        {
            _context = context;
        }

        // GET: Libroes
        public async Task<IActionResult> Index()
        {
            var librosbdContext = _context.Libros.Include(l => l.CodigoCategoriaNavigation).Include(l => l.NitEditorialNavigation);
            return View(await librosbdContext.ToListAsync());
        }

        // GET: Libroes/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libro = await _context.Libros
                .Include(l => l.CodigoCategoriaNavigation)
                .Include(l => l.NitEditorialNavigation)
                .FirstOrDefaultAsync(m => m.Isbn == id);
            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }

        // GET: Libroes/Create
        public IActionResult Create()
        {
            ViewData["CodigoCategoria"] = new SelectList(_context.Categorias, "CodigoCategoria", "CodigoCategoria");
            ViewData["NitEditorial"] = new SelectList(_context.Editoriales, "Nit", "Nit");
            return View();
        }

        // POST: Libroes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Isbn,Titulo,Descripcion,NombreAutor,Publicacion,FechaRegistro,CodigoCategoria,NitEditorial")] Libro libro)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(libro);
                    await _context.SaveChangesAsync();

                    // Mensaje de éxito
                    TempData["Mensaje"] = "¡El libro se ha creado exitosamente!";
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
                        ModelState.AddModelError("Isbn",
                            "Ya existe un libro con este ISBN. Por favor, ingrese un ISBN diferente.");
                        TempData["Mensaje"] = "Error: El ISBN ingresado ya está registrado en el sistema.";
                        TempData["TipoMensaje"] = "error";
                    }
                    else
                    {
                        //Mensaje de error
                        ModelState.AddModelError(string.Empty,
                            "Ha ocurrido un error al crear el libro. Por favor, intente nuevamente.");
                        TempData["Mensaje"] = "Error al crear el libro.";
                        TempData["TipoMensaje"] = "error";
                    }
                }
                catch (Exception ex)
                {
                    //Mensaje de error
                    ModelState.AddModelError(string.Empty,
                        "Ha ocurrido un error inesperado. Por favor, intente nuevamente.");
                    TempData["Mensaje"] = "Error inesperado al crear el libro.";
                    TempData["TipoMensaje"] = "error";
                }
            }

            // Si llegamos aquí, algo falló - volver a mostrar el formulario
            ViewData["CodigoCategoria"] = new SelectList(_context.Categorias, "CodigoCategoria", "CodigoCategoria", libro.CodigoCategoria);
            ViewData["NitEditorial"] = new SelectList(_context.Editoriales, "Nit", "Nit", libro.NitEditorial);
            return View(libro);
        }

        // GET: Libroes/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libro = await _context.Libros.FindAsync(id);
            if (libro == null)
            {
                return NotFound();
            }
            ViewData["CodigoCategoria"] = new SelectList(_context.Categorias, "CodigoCategoria", "CodigoCategoria", libro.CodigoCategoria);
            ViewData["NitEditorial"] = new SelectList(_context.Editoriales, "Nit", "Nit", libro.NitEditorial);
            return View(libro);
        }

        // POST: Libroes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Isbn,Titulo,Descripcion,NombreAutor,Publicacion,FechaRegistro,CodigoCategoria,NitEditorial")] Libro libro)
        {
            if (id != libro.Isbn)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //Mensaje de éxito
                    _context.Update(libro);
                    await _context.SaveChangesAsync();
                    TempData["Mensaje"] = "¡El libro se ha actualizado exitosamente!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LibroExists(libro.Isbn))
                    {
                        //Retornar error
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CodigoCategoria"] = new SelectList(_context.Categorias, "CodigoCategoria", "CodigoCategoria", libro.CodigoCategoria);
            ViewData["NitEditorial"] = new SelectList(_context.Editoriales, "Nit", "Nit", libro.NitEditorial);
            return View(libro);
        }

        // GET: Libroes/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libro = await _context.Libros
                .Include(l => l.CodigoCategoriaNavigation)
                .Include(l => l.NitEditorialNavigation)
                .FirstOrDefaultAsync(m => m.Isbn == id);
            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }

        // POST: Libroes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                var libro = await _context.Libros.FindAsync(id);
                if (libro != null)
                {
                    //Mensaje de éxito
                    _context.Libros.Remove(libro);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return NotFound();
            }
            catch (DbUpdateException)
            {
                //Mensaje de error
                TempData["ErrorMessage"] = "No se puede eliminar este libro porque está siendo utilizado en otras tablas.";
                return RedirectToAction("Delete", new { id = id });
            }
        }

        private bool LibroExists(string id)
        {
            return _context.Libros.Any(e => e.Isbn == id);
        }
    }
}
