using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_.Net_Core.Models;

namespace Inmobiliaria_.Net_Core.Controllers
{
    public class InmuebleController : Controller
    {
        private readonly RepositorioInmueble repositorio;
        private readonly RepositorioPropietario repositorioPropietario;
        private readonly RepositorioTipoInmueble repositorioTipoInmueble;

        public InmuebleController(RepositorioInmueble repositorio, RepositorioPropietario repositorioPropietario, RepositorioTipoInmueble repositorioTipoInmueble)
        {
            this.repositorio = repositorio;
            this.repositorioPropietario = repositorioPropietario;
            this.repositorioTipoInmueble = repositorioTipoInmueble;
        }

        public IActionResult Index()
        {
            var lista = repositorio.ObtenerTodos();
            return View(lista);
        }

        public IActionResult Crear()
        {
            ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();
            ViewBag.TiposInmuebles = repositorioTipoInmueble.ObtenerTodos();
            return View();
        }

        [HttpPost]
        public IActionResult Crear(Inmueble inmueble)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    repositorio.Alta(inmueble);
                    TempData["Mensaje"] = "Inmueble creado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al crear el inmueble: " + ex.Message);
                }
            }
            
            ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();
            ViewBag.TiposInmuebles = repositorioTipoInmueble.ObtenerTodos();
            return View(inmueble);
        }

        public IActionResult Editar(int id)
        {
            var inmueble = repositorio.ObtenerPorId(id);
            if (inmueble == null)
            {
                return NotFound();
            }
            
            ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();
            ViewBag.TiposInmuebles = repositorioTipoInmueble.ObtenerTodos();
            return View(inmueble);
        }

        [HttpPost]
        public IActionResult Editar(Inmueble inmueble)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    repositorio.Modificacion(inmueble);
                    TempData["Mensaje"] = "Inmueble actualizado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al actualizar el inmueble: " + ex.Message);
                }
            }
            
            ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();
            ViewBag.TiposInmuebles = repositorioTipoInmueble.ObtenerTodos();
            return View(inmueble);
        }

        public IActionResult Detalles(int id)
        {
            var inmueble = repositorio.ObtenerPorId(id);
            if (inmueble == null)
            {
                return NotFound();
            }
            return View(inmueble);
        }

        public IActionResult Eliminar(int id)
        {
            var inmueble = repositorio.ObtenerPorId(id);
            if (inmueble == null)
            {
                return NotFound();
            }
            
            // Verificar si tiene contratos vigentes
            if (repositorio.TieneContratosVigentes(id))
            {
                TempData["Error"] = "No se puede eliminar el inmueble porque tiene contratos vigentes";
                return RedirectToAction(nameof(Index));
            }
            
            return View(inmueble);
        }

        [HttpPost]
        [ActionName("Eliminar")]
        public IActionResult EliminarConfirmado(int id)
        {
            try
            {
                // Verificar nuevamente antes de eliminar
                if (repositorio.TieneContratosVigentes(id))
                {
                    TempData["Error"] = "No se puede eliminar el inmueble porque tiene contratos vigentes";
                    return RedirectToAction(nameof(Index));
                }
                
                repositorio.Baja(id);
                TempData["Mensaje"] = "Inmueble eliminado exitosamente";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar el inmueble: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}