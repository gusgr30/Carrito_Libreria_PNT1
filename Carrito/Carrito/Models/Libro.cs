using System;
using System.Collections.Generic;

namespace Carrito.Models;

public partial class Libro
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int GenreId { get; set; }

    public int AuthorId { get; set; }

    public int? PublisherId { get; set; }

    public int? PublishedYear { get; set; }

    public int? Pages { get; set; }

    public int? Price { get; set; }

    public int Stock { get; set; }

    public string? CoverUrl { get; set; }

    public string SynopsisEs { get; set; } = null!;

    public virtual Autor Author { get; set; } = null!;

    public virtual Genero Genre { get; set; } = null!;

    public virtual Editorial? Publisher { get; set; }
}
