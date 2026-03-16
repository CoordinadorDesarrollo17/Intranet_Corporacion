using Capa_Datos;
using Capa_Datos.FarmacovigilanciaD;
using Capa_Datos.Menu;
using Capa_Datos.UsuarioD;
using Capa_Modelo.Login;
using Capa_Negocio.FarmacovigilanciaN;
using Capa_Negocio.Identity;
using Capa_Negocio.Menu;
using Capa_Negocio.UsuarioN;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);

//JC
var politicaUsuariosAutenticados = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

builder.Services.AddControllersWithViews(opciones =>
{
    opciones.Filters.Add(new AuthorizeFilter(politicaUsuariosAutenticados)); //pasamos la política para que se aplique globalmente a todos los controladores y métodos.
});
builder.Services.AddHttpContextAccessor(); //permite acceder al HttpContext actual (la petición en curso) desde clases donde normalmente no lo tendrías disponible como repositorios,servicios,etc , en cambio en el controlador si tenemos acceso porque hereda de ControllerBase 
//Usamos Identity
builder.Services.AddTransient<IUserStore<Usuario>, UsuarioStore>(); //Cada vez que alguien pida un IUserStore<Usuario>, entregamos una instancia de nuestra clase UsuarioStore
builder.Services.AddIdentityCore<Usuario>(opciones =>
{
    //reglas de validacion de password

    opciones.Password.RequireDigit = false; //no requiere numeros
    opciones.Password.RequireLowercase = false; //no requiere minusculas
    opciones.Password.RequireUppercase = false; //no requiere mayusculas
    opciones.Password.RequireNonAlphanumeric = false; //no requiere alfanumerico

    //MensajesDeErrorIdentity es una clase que se encuentra en la carpeta de servicios, con esta clase traducimos al español los mensajes de error
    //AddDefaultTokenProviders Genera tokens temporales, utiles para reestablecer contraseña
})
.AddErrorDescriber<MensajesDeErrorIdentity>()
.AddDefaultTokenProviders()
.AddUserStore<UsuarioStore>();

//Usamos Cookie
builder.Services.AddTransient<SignInManager<Usuario>>();

//Aplicamos Cookie, este codigo permite que nuestra aplicacion entienda el uso de cookies para autenticacion
/* En resumen hace lo siguiente: 
        Voy a usar autenticación basada en cookies para toda la aplicación.
        Cuando un usuario se loguee, guardaré un ticket de autenticación en una cookie (Identity.Application).
        En cada request, leeré esa cookie para saber quién es el usuario.
        Si no tiene cookie y entra a una página protegida, lo reto (Challenge) mandándolo al login.
        Y cuando cierre sesión, borraré esa cookie.
  */
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme; //cómo autenticar al usuario en cada request (aquí: usando la cookie de Identity).
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme; //qué hacer si un usuario no está autenticado y entra a una página [Authorize] (Identity lo redirige al login).
    options.DefaultSignOutScheme = IdentityConstants.ApplicationScheme; //qué esquema usar para cerrar sesión (también cookies).
}).AddCookie(IdentityConstants.ApplicationScheme, opciones =>
{
    opciones.LoginPath = "/Login/login"; // Si alguien intenta acceder a una acción protegida con [Authorize] y no está autenticado, lo redirigimos a esta URL”.
    opciones.AccessDeniedPath = "/Home/Index";    // <- Redirige al Home si no tiene permiso
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddSingleton<DapperContext>(); //Cadena de conexion

builder.Services.AddTransient<IMenuDatos, MenuDatos>();
builder.Services.AddTransient<IMenuNegocio, MenuNegocio>();

builder.Services.AddTransient<IUsuarioDatos , UsuarioDatos >();
builder.Services.AddTransient<IUsuarioNegocio, UsuarioNegocio>();

builder.Services.AddTransient<IFarmacovigilanciaDatos, FarmacovigilanciaDatos>();
builder.Services.AddTransient<IFarmacovigilanciaNegocio, FarmacovigilanciaNegocio>();
//JC END



var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
