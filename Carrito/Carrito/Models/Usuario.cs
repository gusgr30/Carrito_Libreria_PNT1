using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Carrito.Models
{
    public class Usuario : Persona
    {
        [Required]
        public string PasswordHash { get; set; }

        public int? CarritoId { get; set; }

        public Carrito Carrito { get; set; }
        public List<Carrito> HistorialCompra { get; set; }
    }
}
