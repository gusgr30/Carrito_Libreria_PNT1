using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Carrito.Models
{
    public class Persona
    {
        [Key]
        public int PersonaId { get; set; }

        [Required(ErrorMessage = "Se debe ingresar un nombre")]
        public string Nombre { get; set; }
        public Email Email { get; set; }
        public List<Telefono> Telefono { get; set; }
        public Domicilio Domicilio { get; set; }

    }
}
