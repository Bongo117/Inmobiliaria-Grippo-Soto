using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Inmobiliaria_.Net_Core.Models;

namespace Inmobiliaria_.Net_Core.Controllers
{
    public class TipoInmuebleController : Controller
    {
        private readonly RepositorioTipoInmueble repositorio;

        public TipoInmuebleController(RepositorioTipoInmueble repositorio)
        {
            this.repositorio = repositorio;
        }

        public IActionResult Index(string? id, string? nombre, string? uso, bool? esComercial, bool? estado)
        {
            var lista = repositorio.ObtenerTodos(true); // Obtener todos, incluyendo inactivos

            if (!string.IsNullOrEmpty(id) && int.TryParse(id, out int idFilter))
            {
                lista = lista.Where(t => t.Id == idFilter).ToList();
            }
            if (!string.IsNullOrEmpty(nombre))
            {
                lista = lista.Where(t => t.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            if (!string.IsNullOrEmpty(uso))
            {
                lista = lista.Where(t => t.UsoPermitido.Equals(uso, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            if (esComercial.HasValue)
            {
                lista = lista.Where(t => t.EsComercial == esComercial.Value).ToList();
            }
            if (estado.HasValue)
            {
                lista = lista.Where(t => t.Estado == estado.Value).ToList();
            }

            ViewBag.FiltroId = id;
            ViewBag.FiltroNombre = nombre;
            ViewBag.FiltroUso = uso;
            ViewBag.FiltroEsComercial = esComercial;
            ViewBag.FiltroEstado = estado;

            return View(lista);
        }

        public IActionResult Crear()
        {
            ViewBag.UsoPermitido = new List<string> { "Residencial", "Comercial", "Mixto" };
            return View();
        }

        [HttpPost]
        public IActionResult Crear(TipoInmueble tipoInmueble)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    repositorio.Alta(tipoInmueble);
                    TempData["Mensaje"] = "Tipo de inmueble creado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al crear el tipo de inmueble: " + ex.Message);
                }
            }
            
            ViewBag.UsoPermitido = new List<string> { "Residencial", "Comercial", "Mixto" };
            return View(tipoInmueble);
        }

        public IActionResult Editar(int id)
        {
            var tipoInmueble = repositorio.ObtenerPorId(id, true); // Incluir inactivos
            if (tipoInmueble == null)
            {
                return NotFound();
            }
            
            ViewBag.UsoPermitido = new List<string> { "Residencial", "Comercial", "Mixto" };
            return View(tipoInmueble);
        }

        [HttpPost]
        public IActionResult Editar(TipoInmueble tipoInmueble)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    repositorio.Modificacion(tipoInmueble);
                    TempData["Mensaje"] = "Tipo de inmueble actualizado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al actualizar el tipo de inmueble: " + ex.Message);
                }
            }
            
            ViewBag.UsoPermitido = new List<string> { "Residencial", "Comercial", "Mixto" };
            return View(tipoInmueble);
        }

        public IActionResult Detalles(int id)
        {
            var tipoInmueble = repositorio.ObtenerPorId(id, true); // Incluir inactivos
            if (tipoInmueble == null)
            {
                return NotFound();
            }
            return View(tipoInmueble);
        }

        [Authorize(Policy = "SoloAdminParaEliminar")]
        public IActionResult Eliminar(int id)
        {
            var tipoInmueble = repositorio.ObtenerPorId(id, true); // Incluir inactivos
            if (tipoInmueble == null)
            {
                return NotFound();
            }
            
            // Verificar si tiene inmuebles asignados
            if (repositorio.TieneInmueblesAsignados(id))
            {
                TempData["Error"] = "No se puede eliminar el tipo de inmueble porque tiene inmuebles asignados";
                return RedirectToAction(nameof(Index));
            }
            
            return View(tipoInmueble);
        }

        [HttpPost]
        [ActionName("Eliminar")]
        [Authorize(Policy = "SoloAdminParaEliminar")]
        public IActionResult EliminarConfirmado(int id)
        {
            try
            {
                // Verificar nuevamente antes de eliminar
                if (repositorio.TieneInmueblesAsignados(id))
                {
                    TempData["Error"] = "No se puede eliminar el tipo de inmueble porque tiene inmuebles asignados";
                    return RedirectToAction(nameof(Index));
                }
                
                repositorio.Baja(id);
                TempData["Mensaje"] = "Tipo de inmueble eliminado exitosamente";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar el tipo de inmueble: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
