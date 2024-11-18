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
        public async Task<IActionResult> Create([Bind("CodigoCategoria,Nombre")] Categoria categoria)
        {
            if (ModelState.IsValid)
            {

                try
                {
                    //Mensaje de éxito
                    _context.Add(categoria);
                    await _context.SaveChangesAsync();
                    TempData["Mensaje"] = "¡La categoría se ha creado exitosamente!";
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
                        ModelState.AddModelError("CodigoCategoria",
                            "Ya existe una categoría con este ID. Por favor, ingrese un ID diferente.");
                        TempData["Mensaje"] = "Error: El ID ingresado ya está registrado en el sistema.";
                        TempData["TipoMensaje"] = "error";
                    }
                    else
                    {
                        //Mensaje de error
                        ModelState.AddModelError(string.Empty,
                            "Ha ocurrido un error al crear la categoría. Por favor, intente nuevamente.");
                        TempData["Mensaje"] = "Error al crear la categoría.";
                        TempData["TipoMensaje"] = "error";
                    }
                }
                catch (Exception ex)
                {
                    //Mensaje de error
                    ModelState.AddModelError(string.Empty,
                        "Ha ocurrido un error inesperado. Por favor, intente nuevamente.");
                    TempData["Mensaje"] = "Error inesperado al crear la categoría.";
                    TempData["TipoMensaje"] = "error";
                }
            }
            return View(categoria);
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
        public async Task<IActionResult> Edit(int id, [Bind("CodigoCategoria,Nombre")] Categoria categoria)
        {
            if (id != categoria.CodigoCategoria)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {     //Mensaje de éxito
                    _context.Update(categoria);
                    await _context.SaveChangesAsync();
                    TempData["Mensaje"] = "¡La categoría se ha actualizado exitosamente!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoriaExists(categoria.CodigoCategoria))
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
            return View(categoria);
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
