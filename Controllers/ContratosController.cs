using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Inmobiliaria_.Net_Core.Models;

namespace Inmobiliaria_.Net_Core.Controllers
{
    [Authorize]
    public class ContratosController : Controller
    {
        private readonly RepositorioContrato repositorio;
        private readonly RepositorioInquilino repositorioInquilino;
        private readonly RepositorioInmueble repositorioInmueble;

        public ContratosController(RepositorioContrato repositorio, 
            RepositorioInquilino repositorioInquilino, 
            RepositorioInmueble repositorioInmueble)
        {
            this.repositorio = repositorio;
            this.repositorioInquilino = repositorioInquilino;
            this.repositorioInmueble = repositorioInmueble;
        }

        public IActionResult Index()
        {
            var lista = repositorio.ObtenerTodos();
            return View(lista);
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Crear()
        {
            ViewBag.Inquilinos = repositorioInquilino.ObtenerTodos();
            ViewBag.Inmuebles = repositorioInmueble.ObtenerTodos();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult Crear(Contrato contrato)
        {
            if (ModelState.IsValid)
            {
                // Validar que la fecha de fin sea posterior a la fecha de inicio
                if (contrato.FechaFin <= contrato.FechaInicio)
                {
                    ModelState.AddModelError("FechaFin", "La fecha de finalización debe ser posterior a la fecha de inicio");
                }
                
                // Validar que no exista otro contrato para ese inmueble en esas fechas
                if (repositorio.ExisteContratoEnFechas(contrato.InmuebleId, contrato.FechaInicio, contrato.FechaFin))
                {
                    ModelState.AddModelError("", "Ya existe un contrato para ese inmueble en las fechas seleccionadas");
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        repositorio.Alta(contrato);
                        TempData["Mensaje"] = "Contrato creado exitosamente";
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Error al crear el contrato: " + ex.Message);
                    }
                }
            }
            
            ViewBag.Inquilinos = repositorioInquilino.ObtenerTodos();
            ViewBag.Inmuebles = repositorioInmueble.ObtenerTodos();
            return View(contrato);
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Editar(int id)
        {
            var contrato = repositorio.ObtenerPorId(id);
            if (contrato == null)
            {
                return NotFound();
            }
            
            ViewBag.Inquilinos = repositorioInquilino.ObtenerTodos();
            ViewBag.Inmuebles = repositorioInmueble.ObtenerTodos();
            return View(contrato);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult Editar(Contrato contrato)
        {
            if (ModelState.IsValid)
            {
                // Validar que la fecha de fin sea posterior a la fecha de inicio
                if (contrato.FechaFin <= contrato.FechaInicio)
                {
                    ModelState.AddModelError("FechaFin", "La fecha de finalización debe ser posterior a la fecha de inicio");
                }
                
                // Validar que no exista otro contrato para ese inmueble en esas fechas (excluyendo el actual)
                if (repositorio.ExisteContratoEnFechas(contrato.InmuebleId, contrato.FechaInicio, contrato.FechaFin, contrato.Id))
                {
                    ModelState.AddModelError("", "Ya existe un contrato para ese inmueble en las fechas seleccionadas");
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        repositorio.Modificacion(contrato);
                        TempData["Mensaje"] = "Contrato actualizado exitosamente";
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Error al actualizar el contrato: " + ex.Message);
                    }
                }
            }
            
            ViewBag.Inquilinos = repositorioInquilino.ObtenerTodos();
            ViewBag.Inmuebles = repositorioInmueble.ObtenerTodos();
            return View(contrato);
        }

        public IActionResult Detalles(int id)
        {
            var contrato = repositorio.ObtenerPorId(id);
            if (contrato == null)
            {
                return NotFound();
            }
            return View(contrato);
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            var contrato = repositorio.ObtenerPorId(id);
            if (contrato == null)
            {
                return NotFound();
            }
            return View(contrato);
        }

        [HttpPost]
        [ActionName("Eliminar")]
        [Authorize(Roles = "Administrador")]
        public IActionResult EliminarConfirmado(int id)
        {
            try
            {
                repositorio.Baja(id);
                TempData["Mensaje"] = "Contrato eliminado exitosamente";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar el contrato: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Vigentes()
        {
            var lista = repositorio.ObtenerVigentes();
            ViewBag.Titulo = "Contratos Vigentes";
            return View("Index", lista);
        }
    }
}