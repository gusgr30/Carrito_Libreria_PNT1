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
        //DETALLE DE UN LIBRO
        public IActionResult Index(int id)
        {
            var libro = _context.Libros.Find(id);
            return View(libro);
        }
        //BUSCAR LIBROS (LISTADO)
        public IActionResult Buscar(string titulo)
        {
            var query = _context.Libros
                .Include(l => l.Author)
                .Include(l => l.Genre)
                .Include(l => l.Publisher)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(titulo))
            {
                titulo = titulo.ToLower();

                query = query.Where(l =>
                    l.Title.ToLower().Contains(titulo) ||
                    l.Author.FullName.ToLower().Contains(titulo)
                );
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
