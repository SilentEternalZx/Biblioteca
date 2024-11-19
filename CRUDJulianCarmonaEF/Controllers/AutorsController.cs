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
        public async Task<IActionResult> Create([Bind("IdAutor,Nombre,Apellido,Nacionalidad")] Autor autor)
        {

            

            if (ModelState.IsValid)
            {

                try
                {

                    //Mensaje de éxito
                    _context.Add(autor);
                    await _context.SaveChangesAsync();
                    TempData["Mensaje"] = "¡El autor se ha creado exitosamente!";
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
                        ModelState.AddModelError("IdAutor",
                            "Ya existe un autor con este ID. Por favor, ingrese un ID diferente.");
                        TempData["Mensaje"] = "Error: El ID ingresado ya está registrado en el sistema.";
                        TempData["TipoMensaje"] = "error";
                    }
                    else
                    {

                        //Mensaje de error
                        ModelState.AddModelError(string.Empty,
                            "Ha ocurrido un error al crear el autor. Por favor, intente nuevamente.");
                        TempData["Mensaje"] = "Error al crear el autor.";
                        TempData["TipoMensaje"] = "error";
                    }
                }
                catch (Exception ex)
                {

                    //Mensaje de error
                    ModelState.AddModelError(string.Empty,
                        "Ha ocurrido un error inesperado. Por favor, intente nuevamente.");
                    TempData["Mensaje"] = "Error inesperado al crear el autor.";
                    TempData["TipoMensaje"] = "error";
                }


            }
            
            return View(autor);
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
        public async Task<IActionResult> Edit(int id, [Bind("IdAutor,Nombre,Apellido,Nacionalidad")] Autor autor)
        {
            if (id != autor.IdAutor)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    //Mensaje de éxito
                    _context.Update(autor);
                    await _context.SaveChangesAsync();
                    TempData["Mensaje"] = "¡El autor se ha actualizado exitosamente!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AutorExists(autor.IdAutor))
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
            return View(autor);
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

                    //Mensaje de éxito
                    _context.Autors.Remove(autor);
                    await _context.SaveChangesAsync();
                    TempData["Mensaje"] = "Autor eliminado con éxito.";
                    return RedirectToAction(nameof(Index));
                }
                return NotFound();

            }
            catch (DbUpdateException)
            {

                //Mensaje de error
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
