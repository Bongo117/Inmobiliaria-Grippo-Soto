using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Inmobiliaria_Grippo_Soto.Models
{
    /// <summary>
    /// DbContext de Identity + tablas propias (si las hubiera a futuro).
    /// Usa Pomelo MySql como proveedor EF Core.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSet adicionales de la app podrían ir aquí
        // public DbSet<Entidad> Entidades { get; set; }
    }
}


