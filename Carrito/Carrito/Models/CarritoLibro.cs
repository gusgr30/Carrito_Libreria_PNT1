using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Carrito.Models
{
    public class CarritoLibro
    {
        
        [Key]
        public int CarritoId { get; set; }
        
        [Key]
        public int LibroId { get; set; }

        public Carrito Carrito {  get; set; }
        public Libro Libro { get; set; }
        public int Cantidad { get; set; }
    }
}