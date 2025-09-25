namespace Inmobiliaria_Grippo_Soto.Models
{
    public interface IAuditoriaService
    {
        string? ObtenerUsuarioActual();
        void RegistrarCreacion(AuditableEntity entidad);
        void RegistrarModificacion(AuditableEntity entidad);
        void RegistrarEliminacion(AuditableEntity entidad);
    }
}
