using Microsoft.AspNetCore.Mvc;
using Carrito.Models;
using Microsoft.EntityFrameworkCore;

namespace Carrito.Controllers
{
    public class LibroController : Controller
    {
        private readonly AppDbContext _context;

        public LibroController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var lista = _context.Libros.ToList();
            return View(lista);
        }

        public IActionResult Buscar(string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo))
            {
                return View("Index", _context.Libros.ToList());
            }

            var filtrados = _context.Libros
                .Where(l => l.Title.Contains(titulo))
                .ToList();

            return View("Index", filtrados);
        }
    }
}
