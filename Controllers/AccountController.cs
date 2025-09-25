using Inmobiliaria_Grippo_Soto.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria_Grippo_Soto.Controllers
{
    /// <summary>
    /// Controlador de autenticación: Login/Logout y AccessDenied.
    /// </summary>
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        public class LoginViewModel
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public bool RememberMe { get; set; }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(model.Email))
                ModelState.AddModelError(nameof(model.Email), "El email es requerido.");
            if (string.IsNullOrWhiteSpace(model.Password))
                ModelState.AddModelError(nameof(model.Password), "La contraseña es requerida.");

            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Credenciales inválidas.");
                return View(model);
            }

            // Intenta iniciar sesión con email y contraseña
            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Credenciales inválidas.");
            return View(model);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            // Muestra confirmación de cierre de sesión
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // ============= GESTIÓN DE PERFIL =============
        
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditarPerfil()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            var model = new EditarPerfilViewModel
            {
                Nombre = user.Nombre,
                Apellido = user.Apellido,
                Email = user.Email!,
                Telefono = user.PhoneNumber,
                FotoPerfil = user.FotoPerfil
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarPerfil(EditarPerfilViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            // Actualizar datos básicos
            user.Nombre = model.Nombre;
            user.Apellido = model.Apellido;
            user.PhoneNumber = model.Telefono;
            user.FechaUltimaModificacion = DateTime.Now;

            // Manejar subida de foto
            if (model.ArchivoFoto != null && model.ArchivoFoto.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "perfiles");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{user.Id}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(model.ArchivoFoto.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Eliminar foto anterior si existe
                if (!string.IsNullOrEmpty(user.FotoPerfil))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.FotoPerfil.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                        System.IO.File.Delete(oldFilePath);
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ArchivoFoto.CopyToAsync(stream);
                }

                user.FotoPerfil = $"/uploads/perfiles/{fileName}";
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Perfil actualizado correctamente.";
                return RedirectToAction("EditarPerfil");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarFoto()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            if (!string.IsNullOrEmpty(user.FotoPerfil))
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.FotoPerfil.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                user.FotoPerfil = null;
                user.FechaUltimaModificacion = DateTime.Now;
                await _userManager.UpdateAsync(user);
            }

            TempData["SuccessMessage"] = "Foto eliminada correctamente.";
            return RedirectToAction("EditarPerfil");
        }

        [HttpGet]
        [Authorize]
        public IActionResult CambiarContrasena()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarContrasena(CambiarContrasenaViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            var result = await _userManager.ChangePasswordAsync(user, model.ContrasenaActual, model.NuevaContrasena);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["SuccessMessage"] = "Contraseña cambiada correctamente.";
                return RedirectToAction("EditarPerfil");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }
    }

    // ============= VIEW MODELS =============
    
    public class EditarPerfilViewModel
    {
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres")]
        public string? Nombre { get; set; }
        
        [StringLength(50, ErrorMessage = "El apellido no puede exceder los 50 caracteres")]
        public string? Apellido { get; set; }
        
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Email no válido")]
        public string Email { get; set; } = string.Empty;
        
        [Phone(ErrorMessage = "Número de teléfono no válido")]
        public string? Telefono { get; set; }
        
        public string? FotoPerfil { get; set; }
        
        [Display(Name = "Foto de Perfil")]
        public IFormFile? ArchivoFoto { get; set; }
    }

    public class CambiarContrasenaViewModel
    {
        [Required(ErrorMessage = "La contraseña actual es requerida")]
        [Display(Name = "Contraseña Actual")]
        public string ContrasenaActual { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "La nueva contraseña es requerida")]
        [StringLength(100, ErrorMessage = "La contraseña debe tener al menos {2} caracteres", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva Contraseña")]
        public string NuevaContrasena { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "La confirmación de contraseña es requerida")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Nueva Contraseña")]
        [Compare("NuevaContrasena", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarContrasena { get; set; } = string.Empty;
    }
}


