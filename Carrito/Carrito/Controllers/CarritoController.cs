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
        public IActionResult Agregar(int id)
        {
            //creo el resultado del metodo para manejo de excepciones
            IActionResult resultado;

            try
            {
                var usuario = traerUsuario();

                if (usuario.Carrito == null)
                {
                    // Crear carrito asociado
                    var nuevoCarrito = new Carrito.Models.Carrito
                    {
                        PersonaId = usuario.PersonaId,
                        Activo = true
                    };

                    usuario.Carrito = nuevoCarrito;
                    usuario.Carrito.Libros = new List<CarritoLibro>();
                    _context.Carritos.Add(nuevoCarrito);

                    _context.SaveChanges();
                }
                //Pregunto si el libro agregado esta en el carrito
                var libroExistente = usuario.Carrito.Libros
                    .FirstOrDefault(x => x.LibroId == id);

                //Si existe en el carrito, se aumenta la cantidad -- libroExistente refiere a CarritoLibro
                //Si no existe se crea un nuevo CarritoLibro y se lo agrega a Libros (refiere a una List<CarritoLibro>)
                if (libroExistente != null)
                {
                    libroExistente.Cantidad++;
                }
                else
                {
                    var libroAgregar = _context.Libros.Find(id);
                    var carritoLibro = new CarritoLibro
                    {
                        Carrito = usuario.Carrito,
                        CarritoId = usuario.Carrito.CarritoId,
                        Libro = libroAgregar,
                        LibroId = libroAgregar.Id,
                        Cantidad = 1
                    };

                    usuario.Carrito.Libros.Add(carritoLibro);
                }

                _context.SaveChanges();
                resultado = RedirectToAction("Index", "Home");
            } catch (Exception)
            {
                resultado = RedirectToAction("Registro", "Cuenta");
            }


            return resultado;
        }

        // Ver qué hay en el carrito
        public IActionResult Index()
        {
            var usuario = traerUsuario();
            var listaVisual = usuario.Carrito.Libros;

            return View(listaVisual);
        }
        public IActionResult CompraExitosa()
        {
            return View();
        }
        public IActionResult Eliminar(int id)
        {
            var usuario = traerUsuario();
            var libroEliminar = usuario.Carrito.Libros.
                FirstOrDefault(x => x.LibroId == id);

            if(libroEliminar.Cantidad > 1)
            {
                libroEliminar.Cantidad--;
            }
            else
            {
                usuario.Carrito.Libros.Remove(libroEliminar);
            }
            _context.SaveChanges();

                return RedirectToAction("Index", "Carrito");
        }

        private Usuario traerUsuario()
        {
            int? idUsuarioSession = HttpContext.Session.GetInt32("UsuarioId");
            var usuario = _context.Personas
                .OfType<Usuario>()
                .Include(u => u.Carrito)
                .ThenInclude(c => c.Libros)
                .ThenInclude(cl => cl.Libro)
                .FirstOrDefault(u => u.PersonaId == idUsuarioSession.Value);

            return usuario;
        }
    }
    public class ItemCarritoTemporal
    {
        public int LibroId { get; set; }
        public int Cantidad { get; set; }
    }
}
