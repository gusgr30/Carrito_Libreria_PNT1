using Carrito.Data;
using Carrito.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Carrito.Controllers
{
    public class CarritoController : Controller
    {
        private readonly AppDbContext _context;

        //  Inyección del contexto de la base de datos
        public CarritoController(AppDbContext context)
        {
            _context = context;
        }

        
        //  ACCIÓN: AGREGAR UN LIBRO AL CARRITO
       
        public IActionResult Agregar(int id, string returnUrl)
        {
            IActionResult resultado;

            try
            {
                //  Obtengo el usuario logueado
                var usuario = traerUsuario();

                //  Si el usuario NO tiene carrito creado → se crea uno
                if (usuario.Carrito == null)
                {
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

                //  Agrego el libro al carrito usando un método interno
                var libroAgregado = agregarLibroAlCarrito(id, usuario);

                _context.SaveChanges();

                //  Actualizo el contador del carrito en la sesión
                contarLibrosCarrito(usuario);

                //  Mensaje visual para el usuario
                TempData["Success"] = $"El libro '{libroAgregado.Title}' fue agregado con éxito";

                //  Si venía de un returnUrl → vuelve a donde estaba
                if (!string.IsNullOrEmpty(returnUrl))
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

        
        //  MÉTODO PRIVADO: AGREGA O ACTUALIZA UN LIBRO EN EL CARRITO
       
        private Libro agregarLibroAlCarrito(int id, Usuario usuario)
        {
            //  Busco el libro en la base de datos
            var libroAgregar = _context.Libros.Find(id);

            //  Verifico si el libro YA está en el carrito
            var carritoLibroExistente = usuario.Carrito.Libros
                .FirstOrDefault(x => x.LibroId == id);

            //  Si existe, tomo su cantidad actual. Si no, arranca en 0
            var cantidadCarritoExistente = carritoLibroExistente != null
                ? carritoLibroExistente.Cantidad
                : 0;

            //  Antes de sumar, verifico que siga habiendo stock
            if (!hayStock(libroAgregar, cantidadCarritoExistente + 1))
            {
                throw new InvalidOperationException(
                    $"Lo sentimos, el libro '{libroAgregar.Title}' está agotado");
            }

            //  Si ya estaba en el carrito → aumento la cantidad
            if (carritoLibroExistente != null)
            {
                carritoLibroExistente.Cantidad++;
            }
            else
            {
                //  Si NO estaba → creo un nuevo CarritoLibro
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

            return libroAgregar;
        }

        //  Verifico que haya stock suficiente
        private bool hayStock(Libro libro, int cantidad)
        {
            return libro.Stock >= cantidad;
        }

       
        //  ACCIÓN: MOSTRAR LO QUE HAY EN EL CARRITO
       
        public IActionResult Index()
        {
            IActionResult resultado;

            try
            {
                var usuario = traerUsuario();

                // Si el usuario no tiene carrito → mostrar vista vacía
                if (usuario.Carrito == null)
                {
                    resultado = View();
                }
                else
                {
                    //  Se obtiene la lista visual de CarritoLibro
                    var listaVisual = usuario.Carrito.Libros;

                    //  GUARDO EL TOTAL PARA USARLO EN LA PANTALLA DE PAGO
                    TempData["TotalCompra"] = listaVisual.Sum(x => x.Libro.Price * x.Cantidad);

                    //  Devuelvo la vista con la lista de libros
                    resultado = View(listaVisual);
                }
            }
            catch (NullReferenceException)
            {
                TempData["Error"] = "Debes iniciar sesión o registrarte para tener un carrito.";
                resultado = RedirectToAction("Login", "Cuenta");
            }

            return resultado;
        }

       
        //  ACCIÓN: MARCAR COMPRA EXITOSA (CIERRA EL CARRITO)
      
        public IActionResult CompraExitosa()
        {
            var usuario = traerUsuario();

            //  Se marca el carrito como "finalizado"
            usuario.Carrito.Activo = false;

            //  Se descuenta stock en la base
            foreach (var cl in usuario.Carrito.Libros)
            {
                _context.Libros.Find(cl.LibroId).Stock -= cl.Cantidad;
            }

            // 🧾 Guarda carrito en historial
            if (usuario.HistorialCompra == null)
            {
                usuario.HistorialCompra = new List<Carrito.Models.Carrito>();
            }

            usuario.HistorialCompra.Add(usuario.Carrito);

            // Se limpia el carrito actual
            usuario.Carrito = null;

            _context.SaveChanges();

            //  Reset del contador del carrito
            setContadorCarrito(0);

            TempData["Success"] = "Compra finalizada con éxito";

            return View();
        }

       
        //  ACCIÓN: ELIMINAR LIBRO DEL CARRITO
       
        public IActionResult Eliminar(int id)
        {
            var usuario = traerUsuario();

            var libroEliminar = usuario.Carrito.Libros
                .FirstOrDefault(x => x.LibroId == id);

            //  Si hay más de 1 → resto cantidad
            if (libroEliminar.Cantidad > 1)
            {
                libroEliminar.Cantidad--;
            }
            else
            {
                //  Si tiene 1 solo → lo elimino del carrito
                usuario.Carrito.Libros.Remove(libroEliminar);
            }

            _context.SaveChanges();

            //  Actualizo el contador de libros
            contarLibrosCarrito(usuario);

            return RedirectToAction("Index", "Carrito");
        }

        
        //  CONTADOR DE LIBROS DEL CARRITO (SE MUESTRA EN EL HEADER)
       
        private void contarLibrosCarrito(Usuario usuario)
        {
            int cantidadLibros = usuario.Carrito.Libros.Sum(li => li.Cantidad);
            setContadorCarrito(cantidadLibros);
        }

        private void setContadorCarrito(int cantidadLibros)
        {
            HttpContext.Session.SetInt32("CantLibros", cantidadLibros);
        }

       
        // OBTENER USUARIO LOGUEADO (CON SU CARRITO Y SUS LIBROS)
       
        private Usuario traerUsuario()
        {
            int? idUsuarioSession = HttpContext.Session.GetInt32("UsuarioId");

            //  Si no hay sesión → error
            if (idUsuarioSession == null)
            {
                throw new NullReferenceException();
            }

            //  Busco al usuario en la BD y traigo:
            // - Carrito
            // - Lista de CarritoLibros
            // - Libro dentro de cada CarritoLibro
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