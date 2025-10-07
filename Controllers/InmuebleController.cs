using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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

        public IActionResult Index(bool? disponibles = null)
        {
            var lista = repositorio.ObtenerTodos();
            if (disponibles.HasValue)
            {
                if (disponibles.Value)
                {
                    lista = lista.Where(i => i.Estado).ToList();
                }
                else
                {
                    lista = lista.Where(i => !i.Estado).ToList();
                }
                ViewBag.FiltroDisponibles = disponibles.Value ? "Sí" : "No";
            }
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

        public IActionResult LibresEntreFechas()
        {
            ViewBag.Resultados = null;
            return View();
        }

        [HttpPost]
        public IActionResult LibresEntreFechas(DateTime fechaDesde, DateTime fechaHasta)
        {
            if (fechaHasta < fechaDesde)
            {
                TempData["Error"] = "La fecha hasta debe ser posterior a la fecha desde";
                ViewBag.Resultados = null;
                return View();
            }

            var libres = repositorio.ObtenerDisponiblesEntreFechas(fechaDesde, fechaHasta);
            ViewBag.Resultados = libres;
            ViewBag.FechaDesde = fechaDesde;
            ViewBag.FechaHasta = fechaHasta;
            return View();
        }

        [Authorize(Policy = "SoloAdminParaEliminar")]
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
        [Authorize(Policy = "SoloAdminParaEliminar")]
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