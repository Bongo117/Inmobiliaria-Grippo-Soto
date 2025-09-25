using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Inmobiliaria_Grippo_Soto.Models
{
    public class AuditoriaService : IAuditoriaService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuditoriaService(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public string? ObtenerUsuarioActual()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context?.User?.Identity?.IsAuthenticated == true)
            {
                return _userManager.GetUserId(context.User);
            }
            return null;
        }

        public void RegistrarCreacion(AuditableEntity entidad)
        {
            var userId = ObtenerUsuarioActual();
            entidad.FechaCreacion = DateTime.Now;
            entidad.CreadoPor = userId;
            entidad.Estado = true;
        }

        public void RegistrarModificacion(AuditableEntity entidad)
        {
            var userId = ObtenerUsuarioActual();
            entidad.FechaModificacion = DateTime.Now;
            entidad.ModificadoPor = userId;
        }

        public void RegistrarEliminacion(AuditableEntity entidad)
        {
            var userId = ObtenerUsuarioActual();
            entidad.FechaEliminacion = DateTime.Now;
            entidad.EliminadoPor = userId;
            entidad.Estado = false;
        }
    }
}
