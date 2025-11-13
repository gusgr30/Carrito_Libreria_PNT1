using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Carrito.Models
{
    public class Usuario : Persona
    {
        [Key]
        public int UsuarioId { get; set; }
        public Carrito Carrito { get; set; }
        public List<Carrito> HistorialCompra { get; set; }

        //public void iniciarSesion()
        //public void finalizarCompra()
    }
}
