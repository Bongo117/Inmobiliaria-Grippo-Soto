using Inmobiliaria_.Net_Core.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<RepositorioPropietario>();
builder.Services.AddScoped<RepositorioInquilino>();
builder.Services.AddScoped<RepositorioContrato>();
builder.Services.AddScoped<RepositorioInmueble>();
builder.Services.AddScoped<RepositorioPago>();
builder.Services.AddScoped<RepositorioTipoInmueble>();
builder.Services.AddScoped<RepositorioUsuario>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Usuarios/Login";
        options.LogoutPath = "/Usuarios/Logout";
        options.AccessDeniedPath = "/Usuarios/AccesoDenegado";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SoloAdmin", policy =>
        policy.RequireRole("Administrador"));
    
    options.AddPolicy("SoloAdminParaEliminar", policy =>
        policy.RequireRole("Administrador"));
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();