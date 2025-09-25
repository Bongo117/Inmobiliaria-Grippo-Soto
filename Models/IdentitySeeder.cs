using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Inmobiliaria_Grippo_Soto.Models
{
    /// <summary>
    /// Inicializa roles requeridos y un usuario administrador por defecto.
    /// </summary>
    public static class IdentitySeeder
    {
        public const string RolAdministrador = "Administrador";
        public const string RolEmpleado = "Empleado";

        public static async Task SeedAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            // Crear roles si no existen
            if (!await roleManager.RoleExistsAsync(RolAdministrador))
                await roleManager.CreateAsync(new IdentityRole(RolAdministrador));

            if (!await roleManager.RoleExistsAsync(RolEmpleado))
                await roleManager.CreateAsync(new IdentityRole(RolEmpleado));

            // Crear usuario admin por defecto si no existe
            const string adminEmail = "admin@inmo.local";
            const string adminPassword = "Admin123!"; // Cambiar en producción

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, RolAdministrador);
                }
            }
        }
    }
}


