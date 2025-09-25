using Inmobiliaria_Grippo_Soto.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria_Grippo_Soto.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuariosController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsuariosController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var usuarios = await _userManager.Users
                .Where(u => u.Estado)
                .OrderBy(u => u.Email)
                .ToListAsync();

            var usuariosConRoles = new List<UsuarioConRolesViewModel>();
            foreach (var usuario in usuarios)
            {
                var roles = await _userManager.GetRolesAsync(usuario);
                usuariosConRoles.Add(new UsuarioConRolesViewModel
                {
                    Usuario = usuario,
                    Roles = roles.ToList()
                });
            }

            return View(usuariosConRoles);
        }

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            ViewBag.Roles = await _roleManager.Roles.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(CrearUsuarioViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = await _roleManager.Roles.ToListAsync();
                return View(model);
            }

            var usuario = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                Nombre = model.Nombre,
                Apellido = model.Apellido,
                PhoneNumber = model.Telefono,
                EmailConfirmed = true,
                FechaCreacion = DateTime.Now
            };

            var currentUser = await _userManager.GetUserAsync(User);

            var result = await _userManager.CreateAsync(usuario, model.Contrasena);
            if (result.Succeeded)
            {
                // Asignar rol
                if (!string.IsNullOrEmpty(model.RolSeleccionado))
                {
                    await _userManager.AddToRoleAsync(usuario, model.RolSeleccionado);
                }

                TempData["SuccessMessage"] = "Usuario creado correctamente.";
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            ViewBag.Roles = await _roleManager.Roles.ToListAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null || !usuario.Estado)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(usuario);
            var todosLosRoles = await _roleManager.Roles.ToListAsync();

            var model = new EditarUsuarioViewModel
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email!,
                Telefono = usuario.PhoneNumber,
                RolActual = roles.FirstOrDefault(),
                EmailConfirmed = usuario.EmailConfirmed
            };

            ViewBag.Roles = todosLosRoles;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(EditarUsuarioViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = await _roleManager.Roles.ToListAsync();
                return View(model);
            }

            var usuario = await _userManager.FindByIdAsync(model.Id);
            if (usuario == null || !usuario.Estado)
                return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);

            // Actualizar datos básicos
            usuario.Nombre = model.Nombre;
            usuario.Apellido = model.Apellido;
            usuario.PhoneNumber = model.Telefono;
            usuario.EmailConfirmed = model.EmailConfirmed;
            usuario.FechaUltimaModificacion = DateTime.Now;

            var result = await _userManager.UpdateAsync(usuario);
            if (result.Succeeded)
            {
                // Actualizar roles
                var rolesActuales = await _userManager.GetRolesAsync(usuario);
                await _userManager.RemoveFromRolesAsync(usuario, rolesActuales);
                
                if (!string.IsNullOrEmpty(model.RolSeleccionado))
                {
                    await _userManager.AddToRoleAsync(usuario, model.RolSeleccionado);
                }

                TempData["SuccessMessage"] = "Usuario actualizado correctamente.";
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            ViewBag.Roles = await _roleManager.Roles.ToListAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Detalles(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null || !usuario.Estado)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(usuario);

            var model = new UsuarioConRolesViewModel
            {
                Usuario = usuario,
                Roles = roles.ToList()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null || !usuario.Estado)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(usuario);

            var model = new UsuarioConRolesViewModel
            {
                Usuario = usuario,
                Roles = roles.ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
                return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            
            // No permitir que el admin se elimine a sí mismo
            if (usuario.Id == currentUser?.Id)
            {
                TempData["ErrorMessage"] = "No puedes eliminarte a ti mismo.";
                return RedirectToAction("Index");
            }

            // Borrado lógico
            usuario.Estado = false;
            usuario.FechaEliminacion = DateTime.Now;
            usuario.FechaUltimaModificacion = DateTime.Now;

            await _userManager.UpdateAsync(usuario);

            TempData["SuccessMessage"] = "Usuario eliminado correctamente.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestablecerContrasena(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null || !usuario.Estado)
                return NotFound();

            var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);
            var nuevaContrasena = "TempPass123!"; // Contraseña temporal
            
            var result = await _userManager.ResetPasswordAsync(usuario, token, nuevaContrasena);
            if (result.Succeeded)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                usuario.FechaUltimaModificacion = DateTime.Now;
                await _userManager.UpdateAsync(usuario);

                TempData["SuccessMessage"] = $"Contraseña restablecida. Nueva contraseña temporal: {nuevaContrasena}";
            }
            else
            {
                TempData["ErrorMessage"] = "Error al restablecer la contraseña.";
            }

            return RedirectToAction("Detalles", new { id });
        }
    }

    // ============= VIEW MODELS =============

    public class UsuarioConRolesViewModel
    {
        public ApplicationUser Usuario { get; set; } = new();
        public List<string> Roles { get; set; } = new();
    }

    public class CrearUsuarioViewModel
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
        
        [Required(ErrorMessage = "La contraseña es requerida")]
        [StringLength(100, ErrorMessage = "La contraseña debe tener al menos {2} caracteres", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "La confirmación de contraseña es requerida")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        [Compare("Contrasena", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarContrasena { get; set; } = string.Empty;
        
        [Display(Name = "Rol")]
        public string? RolSeleccionado { get; set; }
    }

    public class EditarUsuarioViewModel
    {
        public string Id { get; set; } = string.Empty;
        
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres")]
        public string? Nombre { get; set; }
        
        [StringLength(50, ErrorMessage = "El apellido no puede exceder los 50 caracteres")]
        public string? Apellido { get; set; }
        
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Email no válido")]
        public string Email { get; set; } = string.Empty;
        
        [Phone(ErrorMessage = "Número de teléfono no válido")]
        public string? Telefono { get; set; }
        
        public string? RolActual { get; set; }
        
        [Display(Name = "Rol")]
        public string? RolSeleccionado { get; set; }
        
        [Display(Name = "Email Confirmado")]
        public bool EmailConfirmed { get; set; }
    }
}
