using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria_.Net_Core.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        // Por simplicidad local usamos ClaveHash como texto plano según seed. Luego migrar a hash.
        [Required]
        public string ClaveHash { get; set; } = string.Empty;

        [Required]
        public string Rol { get; set; } = "Empleado"; // Administrador | Empleado

        public string? Apellido { get; set; }
        public string? Nombre { get; set; }
        public string? AvatarUrl { get; set; }
        public bool Estado { get; set; } = true;
        public DateTime FechaAlta { get; set; } = DateTime.Now;
    }
}


