using Microsoft.AspNetCore.Mvc;
using Carrito.Models;
using Carrito.Data;
using System.Linq;

namespace Carrito.Controllers
{
    public class LibrosController : Controller
    {
        // 1. Campo privado para guardar la conexión
        private readonly AppDbContext _context;

        // 2. CONSTRUCTOR: Aquí "pedimos" la base de datos al sistema
        public LibrosController(AppDbContext context)
        {
            _context = context;
        }

        // 3. ACCIÓN (MÉTODO): Aquí usamos la base de datos
        public IActionResult Index()
        {
            // Usamos _context (que llenamos en el constructor) para buscar los libros
            // El .ToList() ejecuta la consulta SQL real a la base de datos
            var listaDeLibros = _context.Libros.ToList();

            // Le pasamos la lista a la vista
            return View(listaDeLibros);
        }
    }
}