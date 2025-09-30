using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_.Net_Core.Models;

namespace Inmobiliaria_.Net_Core.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly RepositorioUsuario repositorioUsuario;

        public UsuariosController(RepositorioUsuario repositorioUsuario)
        {
            this.repositorioUsuario = repositorioUsuario;
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Index()
        {
            var usuarios = repositorioUsuario.ObtenerTodos();
            return View(usuarios);
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string email, string password, string? returnUrl = null)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Email y contraseña son obligatorios";
                return View();
            }

            var usuario = repositorioUsuario.ObtenerPorEmail(email);
            if (usuario == null || usuario.ClaveHash != password)
            {
                ViewBag.Error = "Credenciales inválidas";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Email),
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authProperties);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public IActionResult AccesoDenegado()
        {
            return View();
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult Crear(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                if (repositorioUsuario.ExisteEmail(usuario.Email))
                {
                    ModelState.AddModelError("Email", "Ya existe un usuario con ese email");
                    return View(usuario);
                }
                repositorioUsuario.Alta(usuario);
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Editar(int id)
        {
            var usuario = repositorioUsuario.ObtenerPorId(id);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult Editar(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                if (repositorioUsuario.ExisteEmail(usuario.Email, usuario.Id))
                {
                    ModelState.AddModelError("Email", "Ya existe un usuario con ese email");
                    return View(usuario);
                }
                repositorioUsuario.Modificacion(usuario);
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            var usuario = repositorioUsuario.ObtenerPorId(id);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        [HttpPost, ActionName("Eliminar")]
        [Authorize(Roles = "Administrador")]
        public IActionResult EliminarConfirmado(int id)
        {
            repositorioUsuario.Baja(id);
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public IActionResult Perfil()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (idClaim == null) return NotFound();
            var id = int.Parse(idClaim);
            var usuario = repositorioUsuario.ObtenerPorId(id);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Perfil(Usuario usuario)
        {
            // Los empleados sólo pueden editar sus datos personales, no rol/email si así se desea.
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (idClaim == null) return NotFound();
            usuario.Id = int.Parse(idClaim);
            repositorioUsuario.Modificacion(usuario);
            return RedirectToAction(nameof(Perfil));
        }

        [Authorize]
        public IActionResult CambiarClave()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult CambiarClave(string claveActual, string nuevaClave)
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (idClaim == null) return NotFound();
            var id = int.Parse(idClaim);
            var usuario = repositorioUsuario.ObtenerPorId(id);
            if (usuario == null) return NotFound();
            if (usuario.ClaveHash != claveActual)
            {
                ViewBag.Error = "La clave actual no es correcta";
                return View();
            }
            repositorioUsuario.CambiarClave(id, nuevaClave);
            ViewBag.Mensaje = "Contraseña actualizada";
            return View();
        }
    }
}


