using Carrito.Data;
using Carrito.Models;
using Carrito.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Carrito.Controllers
{
    public class CuentaController : Controller
    {
        private readonly AppDbContext _context;

        public CuentaController(AppDbContext context)
        {
            _context = context;
        }

        // ==============================
        // LOGOUT
        // ==============================
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // ==============================
        // LOGIN GET
        // ==============================
        public IActionResult Login()
        {
            return View();
        }

        // ==============================
        // LOGIN POST
        // ==============================
        [HttpPost]
        public IActionResult Login(LoginViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var usuario = _context.Usuarios
                .FirstOrDefault(u => u.Email == vm.Email);

            if (usuario == null)
            {
                ModelState.AddModelError("", "Email incorrecto.");
                return View(vm);
            }

            string hashIngresado = Hash(vm.Password);

            if (usuario.PasswordHash != hashIngresado)
            {
                ModelState.AddModelError("", "Contraseña incorrecta.");
                return View(vm);
            }

            // guarda la sesion en la cookie para que todas las solicitudes las genere en ese usuario
            HttpContext.Session.SetInt32("UsuarioId", usuario.PersonaId);
            Console.WriteLine("SESSION GUARDADA: " + usuario.PersonaId);//test

            var carritoDb = _context.Carritos
                .Include(c => c.Libros)
                .FirstOrDefault(c => c.PersonaId == usuario.PersonaId && c.Activo == true);

            if(carritoDb != null)
            {
                int cantidad = carritoDb.Libros.Sum(cl => cl.Cantidad);
                HttpContext.Session.SetInt32("CantLibros", cantidad);
            }
            else
            {
                HttpContext.Session.SetInt32("CantLibros", 0);
            }

            return RedirectToAction("Index", "Home");
        }

        // ==============================
        // REGISTRO GET
        // ==============================
        public IActionResult Registro()
        {
            return View();
        }

        // ==============================
        // REGISTRO POST
        // ==============================
        [HttpPost]
        public IActionResult Registro(RegistroViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            // Validar email único
            var existe = _context.Personas.Any(p => p.Email == vm.Email);
            if (existe)
            {
                ModelState.AddModelError("", "El email ya está registrado.");
                return View(vm);
            }

            // Crear usuario
            var usuario = new Usuario
            {
                Nombre = vm.Nombre,
                Email = vm.Email,
                Telefono = vm.Telefono,
                PasswordHash = Hash(vm.Password)
            };

            _context.Usuarios.Add(usuario);

            _context.SaveChanges();

            // Iniciar sesión automática
            HttpContext.Session.SetInt32("UsuarioId", usuario.PersonaId);
            Console.WriteLine("LOGIN SESSION GUARDADA: " + usuario.PersonaId);

            return RedirectToAction("Index", "Home");
        }

        // ==============================
        // HISTORIAL DE COMPRAS
        // ==============================
        public IActionResult Historial()
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            // Traer solo los carritos finalizados del usuario
            var historial = _context.Carritos
                .Where(c => c.PersonaId == usuarioId && c.Activo == false)
                .Include(c => c.Libros)
                    .ThenInclude(cl => cl.Libro)
                .ToList();

            return View(historial);
        }



        // ==============================
        // SHA256
        // ==============================
        private string Hash(string texto)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(texto);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
