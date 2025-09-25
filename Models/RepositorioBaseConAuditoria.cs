using Microsoft.Extensions.Configuration;

namespace Inmobiliaria_Grippo_Soto.Models
{
    public abstract class RepositorioBaseConAuditoria<T> where T : AuditableEntity
    {
        protected readonly IConfiguration configuration;
        protected readonly string connectionString;
        protected readonly IAuditoriaService _auditoriaService;

        protected RepositorioBaseConAuditoria(IConfiguration configuration, IAuditoriaService auditoriaService)
        {
            this.configuration = configuration;
            this._auditoriaService = auditoriaService;
            connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        protected virtual void PrepararParaCreacion(T entidad)
        {
            _auditoriaService.RegistrarCreacion(entidad);
        }

        protected virtual void PrepararParaModificacion(T entidad)
        {
            _auditoriaService.RegistrarModificacion(entidad);
        }

        protected virtual void PrepararParaEliminacion(T entidad)
        {
            _auditoriaService.RegistrarEliminacion(entidad);
        }
    }
}
