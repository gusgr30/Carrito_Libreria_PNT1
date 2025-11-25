using Carrito.Data;
using Carrito.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

        //// ACCIÓN 1: Agregar un libro al carrito
        public IActionResult Agregar(int libroId)
        {
            //creo el resultado del metodo para manejo de excepciones
            IActionResult resultado;

            //Busco al usuario y traigo su carrito con los libros que tiene
            var usuario = _context.Personas
                .OfType<Usuario>()
                .Include(u => u.Carrito)
                .ThenInclude(c => c.Libros)
                .FirstOrDefault(u => u.Email == User.Identity.Name);
            try
            {
                if(usuario.Carrito == null)
                {
                    // Crear carrito asociado
                    var nuevoCarrito = new Carrito.Models.Carrito
                    {
                        PersonaId = usuario.PersonaId,
                        Activo = true
                    };
                    usuario.Carrito = nuevoCarrito;
                    _context.Carritos.Add(nuevoCarrito);

                    _context.SaveChanges();
                }
                //Pregunto si el libro agregado esta en el carrito
                var libroExistente = usuario.Carrito.Libros
                    .FirstOrDefault(x => x.LibroId == libroId);

                //Si existe en el carrito, se aumenta la cantidad -- libroExistente refiere a CarritoLibro
                //Si no existe se crea un nuevo CarritoLibro y se lo agrega a Libros (refiere a una List<CarritoLibro>)
                if (libroExistente != null)
                {
                    libroExistente.Cantidad++;
                }
                else
                {
                    var libroAgregar = new CarritoLibro
                    {
                        LibroId = libroId,
                        Cantidad = 1
                    };

                    usuario.Carrito.Libros.Add(libroAgregar);
                }

                _context.SaveChanges();
                resultado = RedirectToAction("Index", "Home");
            }catch (NullReferenceException)
            {
                resultado = RedirectToAction("Registro", "Cuenta");
            }


                return resultado;
        }

        //public IActionResult Agregar(int id)
        //{
        //    // 1. Intentamos leer lo que ya hay en la memoria (Sesión)
        //    var carritoSession = HttpContext.Session.GetString("CarritoTemporal");
        //    List<ItemCarritoTemporal> items;

        //    // 2. Si está vacío o es nulo, creamos una lista nueva
        //    if (string.IsNullOrEmpty(carritoSession))
        //    {
        //        items = new List<ItemCarritoTemporal>();
        //    }
        //    else
        //    {
        //        // Si ya había algo, lo convertimos de texto a Lista de C#
        //        items = JsonSerializer.Deserialize<List<ItemCarritoTemporal>>(carritoSession);
        //    }

        //    // 3. Buscamos si el libro ya estaba en la lista
        //    var itemExistente = items.FirstOrDefault(x => x.LibroId == id);

        //    if (itemExistente != null)
        //    {
        //        // Si ya existe, solo sumamos cantidad
        //        itemExistente.Cantidad++;
        //    }
        //    else
        //    {
        //        // Si es nuevo, lo agregamos
        //        items.Add(new ItemCarritoTemporal { LibroId = id, Cantidad = 1 });
        //    }

        //    // 4. ¡EL PASO CLAVE! Guardamos la lista actualizada de vuelta en la Sesión
        //    // (Si falta esta línea, el carrito siempre queda igual)
        //    HttpContext.Session.SetString("CarritoTemporal", JsonSerializer.Serialize(items));

        //    // 5. Volvemos al catálogo
        //    return RedirectToAction("Index", "Home");
        //}

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
