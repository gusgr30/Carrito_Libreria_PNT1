using System.ComponentModel.DataAnnotations;

namespace Carrito.Models
{
    public class Autor
    {
        [Key]
        public int AutorId { get; set; }

        [Required]
        public string Nombre { get; set; }

        public List<Libro> Libros { get; set; }

    }
}