using Carrito.Data;
using Carrito.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json; // Necesario para guardar la lista en la sesión

namespace Carrito.Controllers
{
    public class CarritoController : Controller
    {
        private readonly AppDbContext _context;

        public CarritoController(AppDbContext context)
        {
            _context = context;
        }

        // ACCIÓN 1: Agregar un libro al carrito
        public IActionResult Agregar(int id)
        {
            // 1. Intentamos leer lo que ya hay en la memoria (Sesión)
            var carritoSession = HttpContext.Session.GetString("CarritoTemporal");
            List<ItemCarritoTemporal> items;

            // 2. Si está vacío o es nulo, creamos una lista nueva
            if (string.IsNullOrEmpty(carritoSession))
            {
                items = new List<ItemCarritoTemporal>();
            }
            else
            {
                // Si ya había algo, lo convertimos de texto a Lista de C#
                items = JsonSerializer.Deserialize<List<ItemCarritoTemporal>>(carritoSession);
            }

            // 3. Buscamos si el libro ya estaba en la lista
            var itemExistente = items.FirstOrDefault(x => x.LibroId == id);

            if (itemExistente != null)
            {
                // Si ya existe, solo sumamos cantidad
                itemExistente.Cantidad++;
            }
            else
            {
                // Si es nuevo, lo agregamos
                items.Add(new ItemCarritoTemporal { LibroId = id, Cantidad = 1 });
            }

            // 4. ¡EL PASO CLAVE! Guardamos la lista actualizada de vuelta en la Sesión
            // (Si falta esta línea, el carrito siempre queda igual)
            HttpContext.Session.SetString("CarritoTemporal", JsonSerializer.Serialize(items));

            // 5. Volvemos al catálogo
            return RedirectToAction("Index", "Home");
        }

        // ACCIÓN 2: Ver qué hay en el carrito
        public IActionResult Index()
        {
            var carritoSession = HttpContext.Session.GetString("CarritoTemporal");
            var listaVisual = new List<CarritoLibro>();

            if (!string.IsNullOrEmpty(carritoSession))
            {
                var items = JsonSerializer.Deserialize<List<ItemCarritoTemporal>>(carritoSession);

                foreach (var item in items)
                {
                    var libroReal = _context.Libros.Find(item.LibroId);
                    if (libroReal != null)
                    {
                        // AQUÍ ESTÁ LA CLAVE: Convertimos el Libro en un CarritoLibro
                        listaVisual.Add(new CarritoLibro
                        {
                            Libro = libroReal,
                            Cantidad = item.Cantidad,
                            LibroId = libroReal.Id
                            // No asignamos CarritoId porque es temporal
                        });
                    }
                }
            }

            // Ahora enviamos 'listaVisual' que es List<CarritoLibro>
            return View(listaVisual);
        }
        public IActionResult CompraExitosa()
        {
            return View();
        }
    }
    public class ItemCarritoTemporal
    {
        public int LibroId { get; set; }
        public int Cantidad { get; set; }
    }
}
