using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_Grippo_Soto.Models;
using Inmobiliaria_.Net_Core.Models;
using Microsoft.AspNetCore.Authorization;

namespace Inmobiliaria_Grippo_Soto.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly RepositorioInmueble _repoInmueble;
    private readonly RepositorioContrato _repoContrato;
    private readonly RepositorioInquilino _repoInquilino;
    private readonly RepositorioPropietario _repoPropietario;

    public HomeController(ILogger<HomeController> logger, RepositorioInmueble repoInmueble, RepositorioContrato repoContrato, RepositorioInquilino repoInquilino, RepositorioPropietario repoPropietario)
    {
        _logger = logger;
        _repoInmueble = repoInmueble;
        _repoContrato = repoContrato;
        _repoInquilino = repoInquilino;
        _repoPropietario = repoPropietario;
    }

    public IActionResult Index()
    {
        var todosLosInmuebles = _repoInmueble.ObtenerTodos();
        var contratosVigentes = _repoContrato.ObtenerVigentes();
        
        var inmueblesConContratosVigentes = contratosVigentes.Select(c => c.InmuebleId).Distinct().ToList();
        
        var inmueblesDisponibles = todosLosInmuebles.Count(i => i.Estado && !inmueblesConContratosVigentes.Contains(i.Id));
        
        var inmueblesOcupados = inmueblesConContratosVigentes.Count;
        
        var viewModel = new DashboardViewModel
        {
            InmueblesDisponibles = inmueblesDisponibles,
            InmueblesOcupados = inmueblesOcupados,
            TotalInmuebles = todosLosInmuebles.Count,
            ContratosVigentes = contratosVigentes.Count,
            InquilinosActivos = _repoInquilino.ObtenerTodos().Count,
            PropietariosActivos = _repoPropietario.ObtenerTodos().Count,
            ContratosPorVencer = _repoContrato.ObtenerTodos().Where(c => c.Estado && c.FechaFin >= DateTime.Today && c.FechaFin <= DateTime.Today.AddDays(30)).ToList()
        };
        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
