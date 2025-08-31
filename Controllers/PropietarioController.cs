using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_.Net_Core.Models;

namespace Inmobiliaria_.Net_Core.Controllers
{
    public class PropietarioController : Controller
    {
        private readonly RepositorioPropietario repositorio;

        public PropietarioController(IConfiguration configuration)
        {
            repositorio = new RepositorioPropietario(configuration);
        }

        public IActionResult Index()
        {
            var lista = repositorio.ObtenerTodos();
            return View(lista);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Crear(Propietario propietario)
        {
            if (ModelState.IsValid)
            {
                repositorio.Alta(propietario);
                return RedirectToAction(nameof(Index));
            }
            return View(propietario);
        }
    }
}
