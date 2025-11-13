using System;
using System.Collections.Generic;

namespace Carrito.Models;

public partial class Autor
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string? Country { get; set; }

    public virtual ICollection<Libro> Libros { get; set; } = new List<Libro>();
}
