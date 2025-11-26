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
        public IActionResult Agregar(int id, string returnUrl)
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

                //calculo la cantidad de libros en el carrito
                int cantidadLibros = usuario.Carrito.Libros.Sum(li => li.Cantidad);

                //guardo la cantidad en la sesion
                HttpContext.Session.SetInt32("CantLibros", cantidadLibros);

                if (!string.IsNullOrEmpty(returnUrl))// nos devuelve a la URL donde estabamos!
                    resultado = Redirect(returnUrl);
                else
                    resultado = RedirectToAction("Index", "Home");
            } catch (Exception)
            {
                TempData["Error"] = "Debes iniciar sesión o registrarte para agregar libros al carrito.";
                resultado = RedirectToAction("Login", "Cuenta");
            }


            return resultado;
        }

        // Ver qué hay en el carrito
        public IActionResult Index()
        {
            IActionResult resultado;
            try
            {
                var usuario = traerUsuario();
                if(usuario.Carrito == null)
                {
                    resultado = View();
                }
                else
                {
                    var listaVisual = usuario.Carrito.Libros;
                    resultado = View(listaVisual);

                }
            } catch (Exception)
            {
                TempData["Error"] = "Debes iniciar sesión o registrarte para tener un carrito.";
                resultado = RedirectToAction("Login", "Cuenta");
            }

            return resultado;
        }
        public IActionResult CompraExitosa()
        {
            var usuario = traerUsuario();
            usuario.Carrito.Activo = false;

            foreach(var cl in usuario.Carrito.Libros)
            {
                _context.Libros.Find(cl.LibroId).Stock -= cl.Cantidad;
            }

            if(usuario.HistorialCompra == null)
            {
                usuario.HistorialCompra = new List<Carrito.Models.Carrito>();
            }
            usuario.HistorialCompra.Add(usuario.Carrito);
            usuario.Carrito = null;
            _context.SaveChanges();
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

            //calculo la cantidad de libros en el carrito
            int cantidadLibros = usuario.Carrito.Libros.Sum(li => li.Cantidad);

            //guardo la cantidad en la sesion
            HttpContext.Session.SetInt32("CantLibros", cantidadLibros);

            return RedirectToAction("Index", "Carrito");
        }

        private Usuario traerUsuario()
        {
            int? idUsuarioSession = HttpContext.Session.GetInt32("UsuarioId");
            if(idUsuarioSession == null)
            {
                throw new NullReferenceException();
            }

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
