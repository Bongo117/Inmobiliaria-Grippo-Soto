using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_.Net_Core.Models;

namespace Inmobiliaria_.Net_Core.Controllers
{
    public class PropietarioController : Controller
    {
        private readonly RepositorioPropietario repositorio;

        public PropietarioController(RepositorioPropietario repositorio)
        {
            this.repositorio = repositorio;
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
                if (repositorio.ExisteDni(propietario.Dni))
                {
                    ModelState.AddModelError("Dni", "Ya existe un propietario con ese DNI");
                    return View(propietario);
                }

                try
                {
                    repositorio.Alta(propietario);
                    TempData["Mensaje"] = "Propietario creado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al crear el propietario: " + ex.Message);
                }
            }
            return View(propietario);
        }

        public IActionResult Editar(int id)
        {
            var propietario = repositorio.ObtenerPorId(id);
            if (propietario == null)
            {
                return NotFound();
            }
            return View(propietario);
        }

        [HttpPost]
        public IActionResult Editar(Propietario propietario)
        {
            if (ModelState.IsValid)
            {
                if (repositorio.ExisteDni(propietario.Dni, propietario.Id))
                {
                    ModelState.AddModelError("Dni", "Ya existe un propietario con ese DNI");
                    return View(propietario);
                }

                try
                {
                    repositorio.Modificacion(propietario);
                    TempData["Mensaje"] = "Propietario actualizado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al actualizar el propietario: " + ex.Message);
                }
            }
            return View(propietario);
        }

        public IActionResult Detalles(int id)
        {
            var propietario = repositorio.ObtenerPorId(id);
            if (propietario == null)
            {
                return NotFound();
            }
            return View(propietario);
        }

        public IActionResult Eliminar(int id)
        {
            var propietario = repositorio.ObtenerPorId(id);
            if (propietario == null)
            {
                return NotFound();
            }
            return View(propietario);
        }

        [HttpPost]
        [ActionName("Eliminar")]
        public IActionResult EliminarConfirmado(int id)
        {
            try
            {
                repositorio.Baja(id);
                TempData["Mensaje"] = "Propietario eliminado exitosamente";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar el propietario: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}