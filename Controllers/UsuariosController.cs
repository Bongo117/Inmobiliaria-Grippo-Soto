using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        
        [AllowAnonymous]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Registro(Usuario usuario, string Password, string ConfirmPassword)
        {
            try
            {
                if (string.IsNullOrEmpty(Password) || Password != ConfirmPassword)
                {
                    ViewBag.Error = "Las contraseñas no coinciden";
                    return View(usuario);
                }

                if (string.IsNullOrEmpty(usuario.Email))
                {
                    ViewBag.Error = "El email es obligatorio";
                    return View(usuario);
                }

                if (repositorioUsuario.ExisteEmail(usuario.Email))
                {
                    ViewBag.Error = "Ya existe un usuario con ese email";
                    return View(usuario);
                }
                
                usuario.ClaveHash = Password; // En producción, usar hash
                usuario.Rol = "Empleado"; // Por defecto, rol empleado
                usuario.Estado = true;
                
                int res = repositorioUsuario.Alta(usuario);
                if (res > 0)
                {
                    TempData["Mensaje"] = "Registro exitoso. Ahora puede iniciar sesión.";
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    ViewBag.Error = "Error al registrar el usuario. Intente nuevamente.";
                    return View(usuario);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al procesar el registro: " + ex.Message;
                return View(usuario);
            }
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
                new Claim(ClaimTypes.Role, usuario.Rol),
                new Claim("NombreCompleto", $"{usuario.Nombre} {usuario.Apellido}".Trim()),
                new Claim("AvatarUrl", usuario.AvatarUrl ?? "")
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
        public IActionResult Crear(Usuario usuario, string Password, string ConfirmPassword)
        {
            try
            {
                if (string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(ConfirmPassword))
                {
                    ModelState.AddModelError("Password", "La contraseña y su confirmación son obligatorias.");
                    return View(usuario);
                }

                if (Password != ConfirmPassword)
                {
                    ModelState.AddModelError("Password", "Las contraseñas no coinciden.");
                    return View(usuario);
                }

                usuario.ClaveHash = Password; // En producción hash!
                if (ModelState.ContainsKey("ClaveHash"))
                    ModelState.Remove("ClaveHash");

                usuario.Estado = true;

                if (!ModelState.IsValid)
                {
                    return View(usuario);
                }

                if (repositorioUsuario.ExisteEmail(usuario.Email))
                {
                    ModelState.AddModelError("Email", "Ya existe un usuario con ese email");
                    return View(usuario);
                }

                repositorioUsuario.Alta(usuario);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al crear el usuario: " + ex.Message);
                return View(usuario);
            }
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
            try
            {
                if (usuario == null) return BadRequest();

                if (usuario.Id == 0)
                {
                    if (int.TryParse(Request.Form["Id"], out var idFromForm))
                        usuario.Id = idFromForm;
                    else
                        return BadRequest("Id de usuario faltante.");
                }

                var usuarioDb = repositorioUsuario.ObtenerPorId(usuario.Id);
                if (usuarioDb == null) return NotFound();

                if (ModelState.ContainsKey("ClaveHash"))
                    ModelState.Remove("ClaveHash");
                usuario.ClaveHash = usuarioDb.ClaveHash;

                if (string.IsNullOrEmpty(usuario.AvatarUrl))
                    usuario.AvatarUrl = usuarioDb.AvatarUrl;

                if (!ModelState.IsValid)
                {
                    return View(usuario);
                }

                if (repositorioUsuario.ExisteEmail(usuario.Email, usuario.Id))
                {
                    ModelState.AddModelError("Email", "Ya existe un usuario con ese email");
                    return View(usuario);
                }

                var rows = repositorioUsuario.Modificacion(usuario);

                if (rows <= 0)
                {
                    ModelState.AddModelError("", "No se pudieron guardar los cambios. Revise los datos e intente nuevamente.");
                    return View(usuario);
                }

                var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (idClaim != null && int.TryParse(idClaim, out var currentUserId) && currentUserId == usuario.Id)
                {
                    var usuarioActualizado = repositorioUsuario.ObtenerPorId(usuario.Id);
                    if (usuarioActualizado != null)
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, usuarioActualizado.Email),
                            new Claim(ClaimTypes.NameIdentifier, usuarioActualizado.Id.ToString()),
                            new Claim(ClaimTypes.Role, usuarioActualizado.Rol),
                            new Claim("NombreCompleto", $"{usuarioActualizado.Nombre} {usuarioActualizado.Apellido}".Trim()),
                            new Claim("AvatarUrl", usuarioActualizado.AvatarUrl ?? "")
                        };
                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var authProperties = new AuthenticationProperties { IsPersistent = true };
                        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity), authProperties).GetAwaiter().GetResult();
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al editar el usuario: " + ex.Message);
                return View(usuario);
            }
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
        public async Task<IActionResult> Perfil(Usuario usuario, IFormFile avatarFile)
        {
            try
            {
                var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (idClaim == null) return NotFound();
                usuario.Id = int.Parse(idClaim);

                var usuarioDb = repositorioUsuario.ObtenerPorId(usuario.Id);
                if (usuarioDb == null) return NotFound();

                if (avatarFile != null && avatarFile.Length > 0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var fileExtension = Path.GetExtension(avatarFile.FileName).ToLowerInvariant();
                    
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("avatarFile", "Solo se permiten archivos JPG, JPEG, PNG y GIF");
                        return View(usuarioDb);
                    }

                    if (avatarFile.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("avatarFile", "El archivo no puede ser mayor a 5MB");
                        return View(usuarioDb);
                    }

                    var fileName = $"avatar_{usuario.Id}_{DateTime.Now:yyyyMMddHHmmss}{fileExtension}";
                    var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatars");
                    
                    if (!Directory.Exists(uploadsPath))
                    {
                        Directory.CreateDirectory(uploadsPath);
                    }

                    var filePath = Path.Combine(uploadsPath, fileName);
                    
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await avatarFile.CopyToAsync(stream);
                    }

                    if (!string.IsNullOrEmpty(usuarioDb.AvatarUrl))
                    {
                        var prevPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", usuarioDb.AvatarUrl.TrimStart('/'));
                        if (System.IO.File.Exists(prevPath))
                        {
                            try { System.IO.File.Delete(prevPath); } catch { /* no bloquear si falla borrado */ }
                        }
                    }

                    usuarioDb.AvatarUrl = $"/uploads/avatars/{fileName}";
                }

                // Actualizar otros campos (apellido/nombre)
                usuarioDb.Apellido = usuario.Apellido;
                usuarioDb.Nombre = usuario.Nombre;
                // NO cambiar ClaveHash ni Rol aquí (a menos que sea administrador y lo autorices)

                repositorioUsuario.Modificacion(usuarioDb);
                
                // Actualizar los claims de la sesión para reflejar cambios
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuarioDb.Email),
                    new Claim(ClaimTypes.NameIdentifier, usuarioDb.Id.ToString()),
                    new Claim(ClaimTypes.Role, usuarioDb.Rol),
                    new Claim("NombreCompleto", $"{usuarioDb.Nombre} {usuarioDb.Apellido}".Trim()),
                    new Claim("AvatarUrl", usuarioDb.AvatarUrl ?? "")
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), authProperties);

                TempData["Mensaje"] = "Perfil actualizado correctamente";
                return RedirectToAction(nameof(Perfil));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al actualizar el perfil: " + ex.Message);
                var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var usuarioDb = idClaim != null ? repositorioUsuario.ObtenerPorId(int.Parse(idClaim)) : null;
                return View(usuarioDb ?? usuario);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EliminarAvatar()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (idClaim == null) return NotFound();
            var id = int.Parse(idClaim);

            var usuario = repositorioUsuario.ObtenerPorId(id);
            if (usuario == null) return NotFound();

            // Eliminar archivo físico si existe
            if (!string.IsNullOrEmpty(usuario.AvatarUrl))
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", usuario.AvatarUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    try
                    {
                        System.IO.File.Delete(filePath);
                    }
                    catch
                    {
                        // No bloquear si falla el borrado físico
                    }
                }
            }

            // Quitar referencia en la base
            usuario.AvatarUrl = null;
            repositorioUsuario.Modificacion(usuario);

            // Actualizar claims (para que la sesión ya no tenga AvatarUrl)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Email),
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Role, usuario.Rol),
                new Claim("NombreCompleto", $"{usuario.Nombre} {usuario.Apellido}".Trim()),
                new Claim("AvatarUrl", "")
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties { IsPersistent = true };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authProperties);

            TempData["Mensaje"] = "Foto de perfil eliminada correctamente";
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
            try
            {
                if (string.IsNullOrEmpty(claveActual) || string.IsNullOrEmpty(nuevaClave))
                {
                    ViewBag.Error = "Ambos campos son obligatorios";
                    return View();
                }
                
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
                
                if (nuevaClave == claveActual)
                {
                    ViewBag.Error = "La nueva contraseña debe ser diferente a la actual";
                    return View();
                }
                
                int resultado = repositorioUsuario.CambiarClave(id, nuevaClave);
                if (resultado > 0)
                {
                    ViewBag.Mensaje = "Contraseña actualizada correctamente";
                    // Actualizar la sesión con la nueva clave
                    usuario.ClaveHash = nuevaClave;
                }
                else
                {
                    ViewBag.Error = "No se pudo actualizar la contraseña";
                }
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cambiar la contraseña: " + ex.Message;
                return View();
            }
        }
    }
}
