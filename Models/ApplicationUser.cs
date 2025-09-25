using Microsoft.AspNetCore.Identity;

namespace Inmobiliaria_Grippo_Soto.Models
{
    /// <summary>
    /// Usuario de la aplicación extendiendo IdentityUser para permitir
    /// agregar propiedades personalizadas a futuro (por ejemplo, Nombre).
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        // Propiedades personalizadas opcionales (ej.: Nombre, Apellido)
    }
}


