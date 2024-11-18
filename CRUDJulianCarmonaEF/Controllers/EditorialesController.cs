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
        public async Task<IActionResult> Create([Bind("Nit,Nombres,Telefono,Direccion,Email,Sitioweb")] Editoriale editoriale)
        {
            if (ModelState.IsValid)
            {

                try
                {
                    //Mensaje de éxito
                    _context.Add(editoriale);
                    await _context.SaveChangesAsync();
                    TempData["Mensaje"] = "¡La editorial se ha creado exitosamente!";
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
                        ModelState.AddModelError("Nit",
                            "Ya existe un editorial con este NIT. Por favor, ingrese un NIT diferente.");
                        TempData["Mensaje"] = "Error: El NIT ingresado ya está registrado en el sistema.";
                        TempData["TipoMensaje"] = "error";
                    }
                    else
                    {
                        //Mensaje de error
                        ModelState.AddModelError(string.Empty,
                            "Ha ocurrido un error al crear la editorial. Por favor, intente nuevamente.");
                        TempData["Mensaje"] = "Error al crear la editorial.";
                        TempData["TipoMensaje"] = "error";
                    }
                }
                catch (Exception ex)
                {
                    //Mensaje de error
                    ModelState.AddModelError(string.Empty,
                        "Ha ocurrido un error inesperado. Por favor, intente nuevamente.");
                    TempData["Mensaje"] = "Error inesperado al crear la editorial.";
                    TempData["TipoMensaje"] = "error";
                }
            }
            return View(editoriale);
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
        public async Task<IActionResult> Edit(int id, [Bind("Nit,Nombres,Telefono,Direccion,Email,Sitioweb")] Editoriale editoriale)
        {
            if (id != editoriale.Nit)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //Mensaje de éxito
                    _context.Update(editoriale);
                    await _context.SaveChangesAsync();
                    TempData["Mensaje"] = "¡La editorial se ha actualizado exitosamente!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EditorialeExists(editoriale.Nit))
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
            return View(editoriale);
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
