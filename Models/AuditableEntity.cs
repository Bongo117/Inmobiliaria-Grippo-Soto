namespace Inmobiliaria_Grippo_Soto.Models
{
    /// <summary>
    /// Clase base para entidades que requieren auditoría
    /// </summary>
    public abstract class AuditableEntity
    {
        public int Id { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public string? CreadoPor { get; set; } // UserId quien creó
        public DateTime? FechaModificacion { get; set; }
        public string? ModificadoPor { get; set; } // UserId quien modificó
        public DateTime? FechaEliminacion { get; set; }
        public string? EliminadoPor { get; set; } // UserId quien eliminó
        public bool Estado { get; set; } = true; // Para borrado lógico
    }
}
