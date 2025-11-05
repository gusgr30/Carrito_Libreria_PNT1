using System.ComponentModel.DataAnnotations;

namespace Carrito.Models
{
    public class Domicilio
    {
        const string MENSAJE_ERROR = "El campo {0} es requerido";
        [Key]
        public int DomicilioId { get; set; }

        [Required(ErrorMessage = MENSAJE_ERROR)]
        public string Calle {  get; set; }
        public string Altura { get; set; }
        public string PisoDepto { get; set; }
        [Required(ErrorMessage = MENSAJE_ERROR)]
        public string CodigoPostal { get; set; }

        public Persona Persona { get; set; }
        public int PersonaId { get; set; }

    }
}