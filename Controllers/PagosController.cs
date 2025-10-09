using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Inmobiliaria_.Net_Core.Models;

namespace Inmobiliaria_.Net_Core.Controllers
{
    public class PagosController : Controller
    {
        private readonly RepositorioPago repositorio;
        private readonly RepositorioContrato repositorioContrato;

        public PagosController(RepositorioPago repositorio, RepositorioContrato repositorioContrato)
        {
            this.repositorio = repositorio;
            this.repositorioContrato = repositorioContrato;
        }

        public IActionResult Index()
        {
            var lista = repositorio.ObtenerTodos();
            return View(lista);
        }

        public IActionResult PorContrato(int contratoId)
        {
            // Incluir contratos inactivos para poder ver pagos históricos
            var contrato = repositorioContrato.ObtenerPorIdIncluyeInactivos(contratoId);
            if (contrato == null)
            {
                return NotFound();
            }

            var pagos = repositorio.ObtenerPorContrato(contratoId);
            ViewBag.Contrato = contrato;
            ViewBag.TotalPagos = repositorio.ObtenerTotalPagos(contratoId);
            ViewBag.SiguienteNumeroPago = repositorio.ObtenerSiguienteNumeroPago(contratoId);
            ViewBag.ContratoVigente = contrato.EsVigente;
            return View(pagos);
        }

        public IActionResult Crear(int? contratoId = null)
        {
            var pago = new Pago();
            
            if (contratoId.HasValue)
            {
                var contrato = repositorioContrato.ObtenerPorId(contratoId.Value);
                if (contrato == null)
                {
                    return NotFound();
                }
                
                pago.ContratoId = contratoId.Value;
                pago.NumeroPago = repositorio.ObtenerSiguienteNumeroPago(contratoId.Value);
                pago.FechaPago = DateTime.Today;
                pago.Importe = contrato.MontoMensual;
                
                ViewBag.Contrato = contrato;
            }
            
            ViewBag.Contratos = repositorioContrato.ObtenerVigentes();
            return View(pago);
        }

        [HttpPost]
        public IActionResult Crear(Pago pago)
        {
            if (ModelState.IsValid)
            {
                // Validar que el contrato existe
                var contrato = repositorioContrato.ObtenerPorId(pago.ContratoId);
                if (contrato == null)
                {
                    ModelState.AddModelError("ContratoId", "El contrato seleccionado no existe");
                }

                // Validar que no exista ya un pago con ese número para el contrato
                if (repositorio.ExisteNumeroPago(pago.ContratoId, pago.NumeroPago))
                {
                    ModelState.AddModelError("NumeroPago", "Ya existe un pago con ese número para este contrato");
                }

                // Validar que la fecha del pago esté dentro del rango del contrato
                if (contrato != null)
                {
                    if (pago.FechaPago < contrato.FechaInicio || pago.FechaPago > contrato.FechaFin.AddMonths(1))
                    {
                        ModelState.AddModelError("FechaPago", "La fecha del pago debe estar dentro del rango del contrato");
                    }
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        pago.FechaCreacion = DateTime.Now;
                        pago.UsuarioCreador = User?.Identity?.Name;
                        repositorio.Alta(pago);
                        TempData["Mensaje"] = "Pago registrado exitosamente";
                        
                        // Si se creó desde la vista de un contrato, volver a ella.
                        return RedirectToAction(nameof(PorContrato), new { contratoId = pago.ContratoId });
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Error al registrar el pago: " + ex.Message);
                    }
                }
            }

            ViewBag.Contratos = repositorioContrato.ObtenerVigentes();
            if (pago.ContratoId > 0)
            {
                ViewBag.Contrato = repositorioContrato.ObtenerPorId(pago.ContratoId);
            }
            return View(pago);
        }

        public IActionResult Editar(int id)
        {
            var pago = repositorio.ObtenerPorId(id);
            if (pago == null)
            {
                return NotFound();
            }

            // Verificar que el pago no esté anulado
            if (!pago.Estado)
            {
                TempData["Error"] = "No se puede editar un pago anulado";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Contratos = repositorioContrato.ObtenerVigentes();
            return View(pago);
        }

        [HttpPost]
        public IActionResult Editar(Pago pago)
        {
            if (ModelState.IsValid)
            {
                var pagoOriginal = repositorio.ObtenerPorId(pago.Id);
                if (pagoOriginal == null)
                {
                    return NotFound();
                }

                // Verificar que el pago no esté anulado
                if (!pagoOriginal.Estado)
                {
                    TempData["Error"] = "No se puede editar un pago anulado";
                    return RedirectToAction(nameof(Index));
                }

                try
                {
                    // Según la narrativa, solo se puede editar el detalle/concepto
                    pagoOriginal.Detalle = pago.Detalle;
                    repositorio.Modificacion(pagoOriginal);
                    TempData["Mensaje"] = "Pago actualizado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al actualizar el pago: " + ex.Message);
                }
            }

            ViewBag.Contratos = repositorioContrato.ObtenerVigentes();
            return View(pago);
        }

        public IActionResult Detalles(int id)
        {
            var pago = repositorio.ObtenerPorId(id);
            if (pago == null)
            {
                return NotFound();
            }
            return View(pago);
        }

        [Authorize(Policy = "SoloAdminParaEliminar")]
        public IActionResult Anular(int id)
        {
            var pago = repositorio.ObtenerPorId(id);
            if (pago == null)
            {
                return NotFound();
            }

            // Verificar que el pago no esté ya anulado
            if (!pago.Estado)
            {
                TempData["Error"] = "El pago ya está anulado";
                return RedirectToAction(nameof(Index));
            }

            return View(pago);
        }

        [HttpPost]
        [ActionName("Anular")]
        [Authorize(Policy = "SoloAdminParaEliminar")]
        public IActionResult AnularConfirmado(int id)
        {
            try
            {
                var pago = repositorio.ObtenerPorId(id);
                if (pago == null)
                {
                    return NotFound();
                }

                // Verificar que el pago no esté ya anulado
                if (!pago.Estado)
                {
                    TempData["Error"] = "El pago ya está anulado";
                    return RedirectToAction(nameof(PorContrato), new { contratoId = pago.ContratoId });
                }

                string usuarioAnulacion = User?.Identity?.Name ?? "Sistema";
                repositorio.AnularPago(id, usuarioAnulacion);
                TempData["Mensaje"] = "Pago anulado exitosamente";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al anular el pago: " + ex.Message;
            }

            // Siempre redirigir a la vista de pagos del contrato
            var pagoAnulado = repositorio.ObtenerPorId(id); // Obtener el pago para conseguir el ContratoId
            if (pagoAnulado != null)
            {
                return RedirectToAction(nameof(PorContrato), new { contratoId = pagoAnulado.ContratoId });
            }
            // Si el pago no se encuentra (caso muy raro), redirigir al index general
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Policy = "SoloAdminParaEliminar")]
        public IActionResult Eliminar(int id)
        {
            var pago = repositorio.ObtenerPorId(id);
            if (pago == null)
            {
                return NotFound();
            }
            return View(pago);
        }

        [HttpPost]
        [ActionName("Eliminar")]
        [Authorize(Policy = "SoloAdminParaEliminar")]
        public IActionResult EliminarConfirmado(int id)
        {
            try
            {
                var pago = repositorio.ObtenerPorId(id);
                if (pago == null)
                {
                    return NotFound();
                }

                repositorio.EliminarPago(id);
                TempData["Mensaje"] = "Pago eliminado permanentemente";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar el pago: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult CrearDesdeContrato(Pago pago, int contratoId)
        {
            pago.ContratoId = contratoId;
            
            if (ModelState.IsValid)
            {
                // Validar que el contrato existe
                var contrato = repositorioContrato.ObtenerPorId(contratoId);
                if (contrato == null)
                {
                    return NotFound();
                }

                // Validar que no exista ya un pago con ese número para el contrato
                if (repositorio.ExisteNumeroPago(pago.ContratoId, pago.NumeroPago))
                {
                    TempData["Error"] = "Ya existe un pago con ese número para este contrato";
                    return RedirectToAction(nameof(PorContrato), new { contratoId });
                }

                try
                {
                    pago.FechaCreacion = DateTime.Now;
                    pago.UsuarioCreador = User?.Identity?.Name;
                    repositorio.Alta(pago);
                    TempData["Mensaje"] = "Pago registrado exitosamente";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error al registrar el pago: " + ex.Message;
                }
            }
            else
            {
                TempData["Error"] = "Datos de pago inválidos";
            }

            return RedirectToAction(nameof(PorContrato), new { contratoId });
        }
    }
}
