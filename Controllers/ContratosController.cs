using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Inmobiliaria_.Net_Core.Models;

namespace Inmobiliaria_.Net_Core.Controllers
{
    public class ContratosController : Controller
    {
        private readonly RepositorioContrato repositorio;
        private readonly RepositorioInquilino repositorioInquilino;
        private readonly RepositorioInmueble repositorioInmueble;
        private readonly RepositorioPago repositorioPago;

        public ContratosController(RepositorioContrato repositorio, 
            RepositorioInquilino repositorioInquilino, 
            RepositorioInmueble repositorioInmueble,
            RepositorioPago repositorioPago)
        {
            this.repositorio = repositorio;
            this.repositorioInquilino = repositorioInquilino;
            this.repositorioInmueble = repositorioInmueble;
            this.repositorioPago = repositorioPago;
        }

        public IActionResult Index()
        {
            var lista = repositorio.ObtenerTodos();
            return View(lista);
        }

        public IActionResult Crear()
        {
            ViewBag.Inquilinos = repositorioInquilino.ObtenerTodos();
            ViewBag.Inmuebles = repositorioInmueble.ObtenerTodos();
            return View();
        }

        [HttpPost]
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
                        // Auditoría
                        contrato.FechaCreacion = DateTime.Now;
                        contrato.UsuarioCreador = User?.Identity?.Name;

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
            // Permitir ver detalles aun si el contrato no está activo
            var contrato = repositorio.ObtenerPorIdIncluyeInactivos(id);
            if (contrato == null)
            {
                return NotFound();
            }
            return View(contrato);
        }

        [Authorize(Policy = "SoloAdminParaEliminar")]
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
        [Authorize(Policy = "SoloAdminParaEliminar")]
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

        public IActionResult Pagos(int id)
        {
            return RedirectToAction("PorContrato", "Pagos", new { contratoId = id });
        }

        // Métodos para terminación anticipada
        public IActionResult TerminarAnticipadamente(int id)
        {
            var contrato = repositorio.ObtenerPorId(id);
            if (contrato == null)
            {
                return NotFound();
            }
            
            if (contrato.TieneTerminacionAnticipada)
            {
                TempData["Error"] = "Este contrato ya fue terminado anticipadamente";
                return RedirectToAction(nameof(Index));
            }
            
            if (!contrato.EsVigente)
            {
                TempData["Error"] = "Solo se pueden terminar contratos vigentes";
                return RedirectToAction(nameof(Index));
            }
            
            return View(contrato);
        }

        [HttpPost]
        public IActionResult TerminarAnticipadamente(int id, DateTime fechaTerminacion, string motivo)
        {
            var contrato = repositorio.ObtenerPorId(id);
            if (contrato == null)
            {
                return NotFound();
            }
            
            if (contrato.TieneTerminacionAnticipada)
            {
                TempData["Error"] = "Este contrato ya fue terminado anticipadamente";
                return RedirectToAction(nameof(Index));
            }
            
            if (!contrato.EsVigente)
            {
                TempData["Error"] = "Solo se pueden terminar contratos vigentes";
                return RedirectToAction(nameof(Index));
            }
            
            // Validar que la fecha de terminación sea posterior a la fecha de inicio
            if (fechaTerminacion < contrato.FechaInicio)
            {
                ModelState.AddModelError("FechaTerminacionAnticipada", "La fecha de terminación debe ser posterior a la fecha de inicio del contrato");
                return View(contrato);
            }
            
            // Validar que la fecha de terminación sea anterior a la fecha de fin original
            if (fechaTerminacion >= contrato.FechaFin)
            {
                ModelState.AddModelError("FechaTerminacionAnticipada", "La fecha de terminación debe ser anterior a la fecha de finalización original del contrato");
                return View(contrato);
            }
            
            if (string.IsNullOrWhiteSpace(motivo))
            {
                ModelState.AddModelError("MotivoTerminacion", "El motivo de terminación es obligatorio");
                return View(contrato);
            }
            
            try
            {
                // Actualizar el contrato con la terminación anticipada
                contrato.FechaTerminacionAnticipada = fechaTerminacion;
                contrato.MotivoTerminacion = motivo;
                contrato.MultaAplicada = contrato.MontoMultaCalculado;
                contrato.FechaAplicacionMulta = DateTime.Today;
                contrato.FechaTerminacionRegistro = DateTime.Now;
                contrato.UsuarioTerminacion = User?.Identity?.Name;
                
                repositorio.Modificacion(contrato);
                
                // Crear pago de multa automáticamente
                CrearPagoMulta(contrato);
                
                TempData["Mensaje"] = $"Contrato terminado anticipadamente. Multa aplicada: ${contrato.MultaAplicada:F2}";
                return RedirectToAction(nameof(Detalles), new { id = contrato.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al terminar el contrato: " + ex.Message);
                return View(contrato);
            }
        }

        public IActionResult CalcularMulta(int id)
        {
            var contrato = repositorio.ObtenerPorId(id);
            if (contrato == null)
            {
                return NotFound();
            }
            
            return Json(new
            {
                montoMulta = contrato.MontoMultaCalculado,
                porcentajeTiempo = contrato.PorcentajeTiempoTranscurrido,
                mesesMulta = contrato.PorcentajeTiempoTranscurrido < 50 ? 2 : 1,
                fechaInicio = contrato.FechaInicio.ToString("yyyy-MM-dd"),
                fechaFin = contrato.FechaFin.ToString("yyyy-MM-dd"),
                duracionDias = contrato.DuracionDias
            });
        }

        private void CrearPagoMulta(Contrato contrato)
        {
            try
            {
                var pagoMulta = new Pago
                {
                    ContratoId = contrato.Id,
                    NumeroPago = repositorioPago.ObtenerSiguienteNumeroPago(contrato.Id),
                    FechaPago = DateTime.Today,
                    Detalle = $"Multa por terminación anticipada - {contrato.MotivoTerminacion}",
                    Importe = contrato.MultaAplicada!.Value,
                    Estado = true,
                    FechaCreacion = DateTime.Now,
                    UsuarioCreador = "Sistema"
                };

                repositorioPago.Alta(pagoMulta);
            }
            catch (Exception ex)
            {
                // Log del error pero no interrumpir el flujo principal
                // En un entorno de producción, se debería usar un logger apropiado
                Console.WriteLine($"Error al crear pago de multa: {ex.Message}");
            }
        }
    }
}