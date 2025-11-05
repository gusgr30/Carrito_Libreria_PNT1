using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Carrito.Models
{
    public class Libro
    {
        public int LibroId { get; set; }
        public Autor Autor { get; set; }
        public float Precio { get; set; }
        public int Stock { get; set; }
        public Genero Genero { get; set; }
        public List<Valoracion> Valoraciones { get; set; }
        public CarritoLibro CarritoLibro { get; set; }

        public void disminuirStock(int cantidad)
        {
            actualizarStock(-cantidad);
        }
        public void actualizarStock(int cantidad)
        {
            this.Stock += cantidad;
        }
        public boolean hayStock()
        {
            return Stock > 0;
        }
    }
}
