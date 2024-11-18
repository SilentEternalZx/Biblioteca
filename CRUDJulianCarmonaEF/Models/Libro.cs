using System;
using System.Collections.Generic;

namespace CRUDJulianCarmonaEF.Models;

//Modelo libro
public partial class Libro
{
    public string Isbn { get; set; } = null!;

    public string Titulo { get; set; } = null!;

    public string? Descripcion { get; set; }

    public string? NombreAutor { get; set; }

    public DateTime? Publicacion { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public int? CodigoCategoria { get; set; }

    public int? NitEditorial { get; set; }

    public virtual Categoria? CodigoCategoriaNavigation { get; set; }

    public virtual ICollection<LibrosAutor> LibrosAutors { get; set; } = new List<LibrosAutor>();

    public virtual Editoriale? NitEditorialNavigation { get; set; }
}
