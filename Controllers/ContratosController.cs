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
            try
            {
                if (ModelState.IsValid)
                {
                    if (contrato.FechaFin <= contrato.FechaInicio)
                    {
                        ModelState.AddModelError("FechaFin", "La fecha de finalización debe ser posterior a la fecha de inicio");
                    }
                    
                    if (repositorio.ExisteContratoEnFechas(contrato.InmuebleId, contrato.FechaInicio, contrato.FechaFin))
                    {
                        var contratosSuperpuestos = repositorio.ObtenerContratosSuperpuestos(contrato.InmuebleId, contrato.FechaInicio, contrato.FechaFin);
                        var mensaje = "Ya existe un contrato para ese inmueble en las fechas seleccionadas:\n";
                        foreach (var contratoSuperpuesto in contratosSuperpuestos)
                        {
                            mensaje += $"• Contrato ID: {contratoSuperpuesto.Id} - " +
                                      $"Inquilino: {contratoSuperpuesto.Inquilino?.Apellido}, {contratoSuperpuesto.Inquilino?.Nombre} - " +
                                      $"Período: {contratoSuperpuesto.FechaInicio:dd/MM/yyyy} a {contratoSuperpuesto.FechaFin:dd/MM/yyyy}\n";
                        }
                        ModelState.AddModelError("", mensaje.Trim());
                    }
                    
                    if (repositorio.ExisteContratoActivoParaInquilinoEInmueble(contrato.InquilinoId, contrato.InmuebleId))
                    {
                        var contratosExistentes = repositorio.ObtenerContratosActivosPorInquilinoEInmueble(contrato.InquilinoId, contrato.InmuebleId);
                        var contratoExistente = contratosExistentes.FirstOrDefault();
                        
                        if (contratoExistente != null)
                        {
                            var mensaje = $"⚠️ ADVERTENCIA: Ya existe un contrato activo para este inquilino en este inmueble. " +
                                        $"Contrato ID: {contratoExistente.Id}, " +
                                        $"Período: {contratoExistente.FechaInicio:dd/MM/yyyy} - {contratoExistente.FechaFin:dd/MM/yyyy}, " +
                                        $"Monto: ${contratoExistente.MontoMensual:N2}. " +
                                        $"¿Está seguro de que desea crear otro contrato?";
                            
                            ModelState.AddModelError("", mensaje);
                        }
                    }

                    if (ModelState.IsValid)
                    {
                        contrato.FechaCreacion = DateTime.Now;
                        contrato.UsuarioCreador = User?.Identity?.Name;

                        var id = repositorio.Alta(contrato);
                        TempData["Mensaje"] = $"Contrato creado exitosamente con ID: {id}";
                        return RedirectToAction(nameof(Index));
                    }
                }
                
                ViewBag.Inquilinos = repositorioInquilino.ObtenerTodos();
                ViewBag.Inmuebles = repositorioInmueble.ObtenerTodos();
                return View(contrato);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear contrato: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
                ModelState.AddModelError("", "Error al crear el contrato: " + ex.Message);
                ViewBag.Inquilinos = repositorioInquilino.ObtenerTodos();
                ViewBag.Inmuebles = repositorioInmueble.ObtenerTodos();
                return View(contrato);
            }
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
                if (contrato.FechaFin <= contrato.FechaInicio)
                {
                    ModelState.AddModelError("FechaFin", "La fecha de finalización debe ser posterior a la fecha de inicio");
                }
                
                if (repositorio.ExisteContratoEnFechas(contrato.InmuebleId, contrato.FechaInicio, contrato.FechaFin, contrato.Id))
                {
                    var contratosSuperpuestos = repositorio.ObtenerContratosSuperpuestos(contrato.InmuebleId, contrato.FechaInicio, contrato.FechaFin, contrato.Id);
                    var mensaje = "❌ Ya existe otro contrato para ese inmueble en las fechas seleccionadas:\n";
                    foreach (var contratoSuperpuesto in contratosSuperpuestos)
                    {
                        mensaje += $"• Contrato ID: {contratoSuperpuesto.Id} - " +
                                  $"Inquilino: {contratoSuperpuesto.Inquilino?.Apellido}, {contratoSuperpuesto.Inquilino?.Nombre} - " +
                                  $"Período: {contratoSuperpuesto.FechaInicio:dd/MM/yyyy} a {contratoSuperpuesto.FechaFin:dd/MM/yyyy}\n";
                    }
                    ModelState.AddModelError("", mensaje.Trim());
                }
                
                if (repositorio.ExisteContratoActivoParaInquilinoEInmueble(contrato.InquilinoId, contrato.InmuebleId, contrato.Id))
                {
                    var contratosExistentes = repositorio.ObtenerContratosActivosPorInquilinoEInmueble(contrato.InquilinoId, contrato.InmuebleId);
                    var contratoExistente = contratosExistentes.FirstOrDefault(c => c.Id != contrato.Id);
                    
                    if (contratoExistente != null)
                    {
                        var mensaje = $"⚠️ ADVERTENCIA: Ya existe otro contrato activo para este inquilino en este inmueble. " +
                                    $"Contrato ID: {contratoExistente.Id}, " +
                                    $"Período: {contratoExistente.FechaInicio:dd/MM/yyyy} - {contratoExistente.FechaFin:dd/MM/yyyy}, " +
                                    $"Monto: ${contratoExistente.MontoMensual:N2}. " +
                                    $"¿Está seguro de que desea continuar con la edición?";
                        
                        ModelState.AddModelError("", mensaje);
                    }
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
            var contrato = repositorio.ObtenerPorIdIncluyeInactivos(id);
            if (contrato == null)
            {
                return NotFound();
            }
            return View(contrato);
        }

        public IActionResult Renovar(int id)
        {
            var contrato = repositorio.ObtenerPorIdIncluyeInactivos(id);
            if (contrato == null)
            {
                return NotFound();
            }

            var nuevo = new Contrato
            {
                InquilinoId = contrato.InquilinoId,
                InmuebleId = contrato.InmuebleId,
                FechaInicio = contrato.FechaFin.AddDays(1),
                FechaFin = contrato.FechaFin.AddYears(1),
                MontoMensual = contrato.MontoMensual
            };

            ViewBag.Inquilinos = repositorioInquilino.ObtenerTodos();
            ViewBag.Inmuebles = repositorioInmueble.ObtenerTodos();
            ViewBag.ContratoOriginal = contrato;
            return View("Crear", nuevo);
        }

        [HttpPost]
        public IActionResult Renovar(int id, DateTime fechaInicio, DateTime fechaFin, decimal montoMensual)
        {
            var original = repositorio.ObtenerPorIdIncluyeInactivos(id);
            if (original == null)
            {
                return NotFound();
            }

            var contrato = new Contrato
            {
                InquilinoId = original.InquilinoId,
                InmuebleId = original.InmuebleId,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                MontoMensual = montoMensual,
                Estado = true,
                FechaCreacion = DateTime.Now,
                UsuarioCreador = User?.Identity?.Name
            };

            if (contrato.FechaFin <= contrato.FechaInicio)
            {
                ModelState.AddModelError("FechaFin", "La fecha de finalización debe ser posterior a la fecha de inicio");
            }
            if (repositorio.ExisteContratoEnFechas(contrato.InmuebleId, contrato.FechaInicio, contrato.FechaFin))
            {
                ModelState.AddModelError("", "Ya existe un contrato para ese inmueble en las fechas seleccionadas");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Inquilinos = repositorioInquilino.ObtenerTodos();
                ViewBag.Inmuebles = repositorioInmueble.ObtenerTodos();
                ViewBag.ContratoOriginal = original;
                return View("Crear", contrato);
            }

            try
            {
                repositorio.Alta(contrato);
                TempData["Mensaje"] = "Contrato renovado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al renovar el contrato: " + ex.Message);
                ViewBag.Inquilinos = repositorioInquilino.ObtenerTodos();
                ViewBag.Inmuebles = repositorioInmueble.ObtenerTodos();
                ViewBag.ContratoOriginal = original;
                return View("Crear", contrato);
            }
        }

        public IActionResult FinalizanEn(int dias = 30)
        {
            var hoy = DateTime.Today;
            var hasta = hoy.AddDays(dias);
            var todos = repositorio.ObtenerTodos();
            var proximos = todos.Where(c => c.Estado && c.FechaFin >= hoy && c.FechaFin <= hasta).ToList();
            ViewBag.Titulo = $"Contratos que finalizan en {dias} días";
            return View("Index", proximos);
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

        public IActionResult VigentesEntreFechas(DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            ViewBag.Resultados = null;
            ViewBag.FechaDesde = fechaDesde;
            ViewBag.FechaHasta = fechaHasta;

            if (fechaDesde.HasValue && fechaHasta.HasValue)
            {
                if (fechaHasta.Value < fechaDesde.Value)
                {
                    TempData["Error"] = "La fecha hasta debe ser posterior a la fecha desde";
                    return View();
                }

                var vigentes = repositorio.ObtenerVigentesEntreFechas(fechaDesde.Value, fechaHasta.Value);
                ViewBag.Resultados = vigentes;
            }
            return View();
        }

        [HttpPost]
        public IActionResult VigentesEntreFechasPost(DateTime fechaDesde, DateTime fechaHasta)
        {
            return RedirectToAction(nameof(VigentesEntreFechas), new { fechaDesde = fechaDesde.ToString("yyyy-MM-dd"), fechaHasta = fechaHasta.ToString("yyyy-MM-dd") });
        }

        public IActionResult Pagos(int id)
        {
            return RedirectToAction("PorContrato", "Pagos", new { contratoId = id });
        }

        public IActionResult PorInmueble(int inmuebleId)
        {
            var lista = repositorio.ObtenerPorInmueble(inmuebleId);
            ViewBag.Titulo = $"Contratos del Inmueble #{inmuebleId}";
            return View("Index", lista);
        }

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
        public IActionResult TerminarAnticipadamente(Contrato contrato)
        {
            var contratoOriginal = repositorio.ObtenerPorId(contrato.Id);
            if (contratoOriginal == null)
            {
                return NotFound();
            }
            
            if (contratoOriginal.TieneTerminacionAnticipada)
            {
                TempData["Error"] = "Este contrato ya fue terminado anticipadamente";
                return RedirectToAction(nameof(Index));
            }
            
            if (!contratoOriginal.EsVigente)
            {
                TempData["Error"] = "Solo se pueden terminar contratos vigentes";
                return RedirectToAction(nameof(Index));
            }
            
            if (contrato.FechaTerminacionAnticipada.HasValue && contrato.FechaTerminacionAnticipada.Value.Date < contratoOriginal.FechaInicio.Date)
            {
                ModelState.AddModelError("FechaTerminacionAnticipada", "La fecha de terminación no puede ser anterior a la fecha de inicio del contrato");
                return View(contratoOriginal);
            }
            
            if (contrato.FechaTerminacionAnticipada.HasValue && contrato.FechaTerminacionAnticipada.Value.Date >= contratoOriginal.FechaFin.Date)
            {
                ModelState.AddModelError("FechaTerminacionAnticipada", "La fecha de terminación debe ser anterior a la fecha de finalización original del contrato");
                return View(contratoOriginal);
            }
            
            if (string.IsNullOrWhiteSpace(contrato.MotivoTerminacion))
            {
                ModelState.AddModelError("MotivoTerminacion", "El motivo de terminación es obligatorio");
                return View(contratoOriginal);
            }
            
            try
            {
                contratoOriginal.FechaTerminacionAnticipada = contrato.FechaTerminacionAnticipada;
                contratoOriginal.MotivoTerminacion = contrato.MotivoTerminacion;
                contratoOriginal.MultaAplicada = contratoOriginal.MontoMultaCalculado;
                contratoOriginal.FechaAplicacionMulta = DateTime.Today;
                contratoOriginal.FechaTerminacionRegistro = DateTime.Now;
                contratoOriginal.UsuarioTerminacion = User?.Identity?.Name;
                
                repositorio.Modificacion(contratoOriginal);
                
                CrearPagoMulta(contratoOriginal);
                
                TempData["Mensaje"] = $"Contrato terminado anticipadamente. Multa aplicada: ${contratoOriginal.MultaAplicada:F2}";
                return RedirectToAction(nameof(Detalles), new { id = contratoOriginal.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al terminar el contrato: " + ex.Message);
                return View(contratoOriginal);
            }
        }

        public IActionResult CalcularMulta(int id, DateTime? fechaTerminacion = null)
        {
            var contrato = repositorio.ObtenerPorId(id);
            if (contrato == null)
            {
                return NotFound();
            }
            
            if (fechaTerminacion.HasValue)
            {
                var duracionTotal = contrato.DuracionDias;
                var tiempoTranscurrido = (fechaTerminacion.Value - contrato.FechaInicio).Days + 1;
                var porcentaje = (double)tiempoTranscurrido / duracionTotal * 100;
                var mesesMulta = porcentaje < 50 ? 2 : 1;
                var montoMulta = contrato.MontoMensual * mesesMulta;
                
                return Json(new
                {
                    montoMulta = montoMulta,
                    porcentajeTiempo = porcentaje,
                    mesesMulta = mesesMulta,
                    fechaInicio = contrato.FechaInicio.ToString("yyyy-MM-dd"),
                    fechaFin = contrato.FechaFin.ToString("yyyy-MM-dd"),
                    duracionDias = duracionTotal,
                    tiempoTranscurrido = tiempoTranscurrido
                });
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
                Console.WriteLine($"Error al crear pago de multa: {ex.Message}");
            }
        }
    }
}