using System.ComponentModel.DataAnnotations;
using Inmobiliaria_Grippo_Soto.Models;

namespace Inmobiliaria_.Net_Core.Models
{
    public class Inquilino : AuditableEntity
    {
        
        [Required(ErrorMessage = "El DNI es obligatorio")]
        public string Dni { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El apellido es obligatorio")]
        public string Apellido { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; } = string.Empty;
        
        [EmailAddress(ErrorMessage = "Email no válido")]
        public string? Email { get; set; }
        
        public string? Telefono { get; set; }
        
        public string? Domicilio { get; set; }
        
        
        public string NombreCompleto => $"{Nombre} {Apellido}".Trim();
    }
}