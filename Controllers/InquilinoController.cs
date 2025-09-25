using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Inmobiliaria_.Net_Core.Models;

namespace Inmobiliaria_.Net_Core.Controllers
{
    [Authorize]
    public class InquilinoController : Controller
    {
        private readonly RepositorioInquilino repositorio;

        public InquilinoController(RepositorioInquilino repositorio)
        {
            this.repositorio = repositorio;
        }

        public IActionResult Index()
        {
            var lista = repositorio.ObtenerTodos();
            return View(lista);
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult Crear(Inquilino inquilino)
        {
            if (ModelState.IsValid)
            {
                if (repositorio.ExisteDni(inquilino.Dni))
                {
                    ModelState.AddModelError("Dni", "Ya existe un inquilino con ese DNI");
                    return View(inquilino);
                }

                try
                {
                    repositorio.Alta(inquilino);
                    TempData["Mensaje"] = "Inquilino creado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al crear el inquilino: " + ex.Message);
                }
            }
            return View(inquilino);
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Editar(int id)
        {
            var inquilino = repositorio.ObtenerPorId(id);
            if (inquilino == null)
            {
                return NotFound();
            }
            return View(inquilino);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult Editar(Inquilino inquilino)
        {
            if (ModelState.IsValid)
            {
                if (repositorio.ExisteDni(inquilino.Dni, inquilino.Id))
                {
                    ModelState.AddModelError("Dni", "Ya existe un inquilino con ese DNI");
                    return View(inquilino);
                }

                try
                {
                    repositorio.Modificacion(inquilino);
                    TempData["Mensaje"] = "Inquilino actualizado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al actualizar el inquilino: " + ex.Message);
                }
            }
            return View(inquilino);
        }

        public IActionResult Detalles(int id)
        {
            var inquilino = repositorio.ObtenerPorId(id);
            if (inquilino == null)
            {
                return NotFound();
            }
            return View(inquilino);
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            var inquilino = repositorio.ObtenerPorId(id);
            if (inquilino == null)
            {
                return NotFound();
            }
            return View(inquilino);
        }

        [HttpPost]
        [ActionName("Eliminar")]
        [Authorize(Roles = "Administrador")]
        public IActionResult EliminarConfirmado(int id)
        {
            try
            {
                repositorio.Baja(id);
                TempData["Mensaje"] = "Inquilino eliminado exitosamente";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar el inquilino: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}