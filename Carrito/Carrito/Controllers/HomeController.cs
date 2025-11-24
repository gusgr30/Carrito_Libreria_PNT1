using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Carrito.Data;
using Carrito.Models;
using System.Linq;

namespace Carrito.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        // 1. trae la BD como parametro del constructor
        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int pagina = 1)
        {
            int cantidadPorPagina = 16;

            // 2. se consulta la BD para traer los libros con sus relaciones
            var libros = _context.Libros
                                 .Include(l => l.Author)    // Traemos al autor
                                 .Include(l => l.Genre)     // Traemos el género
                                 .Include(l => l.Publisher) // Traemos la editorial
                                 .Skip((pagina - 1) * cantidadPorPagina) //dependiendo la pagina que trae saltea de a 16 libros
                                 .Take(cantidadPorPagina)   // Tomo los primeros 16 libros de la BD
                                 .ToList();

            // Guardamos la pagina actual en la ViewBag para que la vista sepa dónde está
            ViewBag.PaginaActual = pagina;

            // 3. se pasa la variable 'libros' a la vista
            return View(libros);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
