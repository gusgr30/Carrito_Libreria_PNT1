using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Carrito.Models
{
    public class Carrito
    {
        [Key]
        public int CarritoId { get; set; }

        //public Usuario Usuario { get; set; }
        public int PersonaId { get; set; }
        public List<CarritoLibro> Libros { get; set; }

        public bool Activo { get; set; } = true;

    }
}
