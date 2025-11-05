using System.ComponentModel.DataAnnotations;

namespace Carrito.Models
{
    public class Valoracion
    {
        [Key]
        public int ValoracionId { get; set; }

        [Required]
        [Range(1,5, ErrorMessage = "Error! El puntaje debe estar entre {1} y {2}")]
        public int Puntaje { get; set; }
        public string Comentario { get; set; }
        public Usuario Usuario { get; set; }
        public Libro Libro { get; set; }
        public int LibroId { get; set; }
        public int UsuarioId { get; set; }
    }
}