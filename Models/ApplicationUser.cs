using Microsoft.AspNetCore.Identity;

namespace Inmobiliaria_Grippo_Soto.Models
{
    /// <summary>
    /// Usuario de la aplicación extendiendo IdentityUser para permitir
    /// agregar propiedades personalizadas (Nombre, Apellido, Foto).
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? FotoPerfil { get; set; } // Ruta de la imagen
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime? FechaUltimaModificacion { get; set; }
        public bool Estado { get; set; } = true; // Para borrado lógico
        
        public string NombreCompleto => $"{Nombre} {Apellido}".Trim();
    }
}


