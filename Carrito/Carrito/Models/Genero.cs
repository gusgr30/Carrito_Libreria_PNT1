using System;
using System.Collections.Generic;

namespace Carrito.Models;

public partial class Genero
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Libro> Libros { get; set; } = new List<Libro>();
}
