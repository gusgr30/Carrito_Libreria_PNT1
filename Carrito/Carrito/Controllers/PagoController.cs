using Microsoft.AspNetCore.Mvc;

namespace Carrito.Controllers
{
    public class PagoController : Controller
    {
        // Pantalla para elegir método de pago
        public IActionResult Metodo()
        {
            return View();
        }

        // Recibe el método elegido por URL y muestra la confirmación
        public IActionResult Confirmar(string metodo)
        {
            ViewBag.Metodo = metodo;   // Le paso el método a la vista
            return View();
        }
    }
}



