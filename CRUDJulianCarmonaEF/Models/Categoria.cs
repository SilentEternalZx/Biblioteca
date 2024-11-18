using System;
using System.Collections.Generic;

namespace CRUDJulianCarmonaEF.Models;
 

//Modelo categoría
public partial class Categoria
{
    public int CodigoCategoria { get; set; }

    public string? Nombre { get; set; }

    public virtual ICollection<Libro> Libros { get; set; } = new List<Libro>();
}
