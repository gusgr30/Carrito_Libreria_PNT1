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
                agregarLibroAlCarrito(id, usuario);

                _context.SaveChanges();

                contarLibrosCarrito(usuario);

                if (!string.IsNullOrEmpty(returnUrl))// nos devuelve a la URL donde estabamos!
                    resultado = Redirect(returnUrl);
                else
                    resultado = RedirectToAction("Index", "Home");
            }
            catch (NullReferenceException)
            {
                TempData["Error"] = "Debes iniciar sesión o registrarte para agregar libros al carrito.";
                resultado = RedirectToAction("Login", "Cuenta");
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                resultado = Redirect(returnUrl);
            }


            return resultado;
        }

        private void agregarLibroAlCarrito(int id, Usuario usuario)
        {
            //busco el libro en el contexto de la BD
            var libroAgregar = _context.Libros.Find(id);

            //Pregunto si el libro agregado esta en el carrito del usuario recibido por parametro
            var carritoLibroExistente = usuario.Carrito.Libros
                .FirstOrDefault(x => x.LibroId == id);

            //si el carritoLibro existe, obtengo la cantidad actual en el carrito, sino 0
            var cantidadCarritoExistente = carritoLibroExistente != null ? carritoLibroExistente.Cantidad : 0;

            //evaluo stock y en caso de no haber lanza excepcion. Se pasa el libro y la cantidad que se quiere agregar (cantidad existente + 1)
            if (!hayStock(libroAgregar, cantidadCarritoExistente + 1))
            {
                throw new InvalidOperationException($"Lo sentimos, el libro '{libroAgregar.Title}' está agotado");
            }

            //Si existe en el carrito, se aumenta la propiedad cantidad -- carritoLibroExistente refiere a CarritoLibro
            //Si no existe se crea un nuevo CarritoLibro y se lo agrega a Libros (refiere a una List<CarritoLibro>)
            if (carritoLibroExistente != null)
            {
                carritoLibroExistente.Cantidad++;
            }
            else
            {
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
        }

        private bool hayStock(Libro libro, int cantidad)
        {
            return libro.Stock >= cantidad;
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
            } catch (NullReferenceException)
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

            foreach (var cl in usuario.Carrito.Libros)
            {
                _context.Libros.Find(cl.LibroId).Stock -= cl.Cantidad;
            }

            if (usuario.HistorialCompra == null)
            {
                usuario.HistorialCompra = new List<Carrito.Models.Carrito>();
            }
            usuario.HistorialCompra.Add(usuario.Carrito);
            usuario.Carrito = null;
            _context.SaveChanges();
            setContadorCarrito(0);
            return View();
        }
        public IActionResult Eliminar(int id)
        {
            var usuario = traerUsuario();
            var libroEliminar = usuario.Carrito.Libros.
                FirstOrDefault(x => x.LibroId == id);

            if (libroEliminar.Cantidad > 1)
            {
                libroEliminar.Cantidad--;
            }
            else
            {
                usuario.Carrito.Libros.Remove(libroEliminar);
            }
            _context.SaveChanges();
            contarLibrosCarrito(usuario);

            return RedirectToAction("Index", "Carrito");
        }
        private void contarLibrosCarrito(Usuario usuario)
        {
            //calculo la cantidad de libros en el carrito
            int cantidadLibros = usuario.Carrito.Libros.Sum(li => li.Cantidad);
            setContadorCarrito(cantidadLibros);
        }
        private void setContadorCarrito(int cantidadLibros)
        {
            //guardo la cantidad en la sesion
            HttpContext.Session.SetInt32("CantLibros", cantidadLibros);
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
}
