using Microsoft.AspNetCore.Mvc;
using Carrito.Models;
using Microsoft.EntityFrameworkCore;

namespace Carrito.Controllers
{
    public class LibroController : Controller
    {
        private readonly Data.AppDbContext _context;

        public LibroController(Data.AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var lista = _context.Libros
                .Include(l => l.Author)
                .Include(l => l.Genre)
                .Include(l => l.Publisher)
                .ToList();

            return View(lista);
        }

        public IActionResult Buscar(string titulo)
        {
            var query = _context.Libros
                .Include(l => l.Author)
                .Include(l => l.Genre)
                .Include(l => l.Publisher)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(titulo))
            {
                query = query.Where(l => l.Title.Contains(titulo));
            }

            return View("Buscar", query.ToList());
        }

        public IActionResult PorGenero(int id)
        {
            var libros = _context.Libros
                .Where(l => l.GenreId == id)
                .Include(l => l.Author)
                .Include(l => l.Genre)
                .Include(l => l.Publisher)
                .ToList();

            return View("Buscar", libros);  // reutilizamos la vista Buscar
        }

    }
}
