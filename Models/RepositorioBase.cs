using Microsoft.Extensions.Configuration;

namespace Inmobiliaria_.Net_Core.Models
{
    public abstract class RepositorioBase
    {
        protected readonly IConfiguration configuration;
        protected readonly string connectionString;

        protected RepositorioBase(IConfiguration configuration)
        {
            this.configuration = configuration;
            connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }
    }
}