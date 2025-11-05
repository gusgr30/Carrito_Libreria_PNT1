using System.ComponentModel.DataAnnotations;

namespace Carrito.Models
{
    public class Telefono
    {
        [Key]
        public int TelefonoID { get; set; }

        public String Numero { get; set; }
        public Persona Persona { get; set; }
        public int PersonaId { get; set; }
    }
}