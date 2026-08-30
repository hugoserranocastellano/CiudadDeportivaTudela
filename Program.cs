using CiudadDeportivaTudela.Components;
using CiudadDeportivaTudela.Data;
using CiudadDeportivaTudela.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// El socio entra con su número de socio y el teléfono como contraseña; la sesión
// se guarda en una cookie para que sobreviva a recargas y reconexiones.
builder.Services.AddAuthentication(SocioAuth.Scheme)
    .AddCookie(SocioAuth.Scheme, options =>
    {
        options.Cookie.Name = "CiudadDeportiva.Socio";
        options.LoginPath = "/";
        options.LogoutPath = "/logout";
        options.AccessDeniedPath = "/";
        options.ExpireTimeSpan = TimeSpan.FromHours(12);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

// Render termina el TLS en su proxy y reenvía la petición por HTTP. Sin esto,
// Request.Scheme sería "http" y las URLs absolutas saldrían mal.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    // La IP del proxy de Render no se conoce de antemano.
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

var rawConnectionString = builder.Configuration.GetConnectionString("Supabase");
if (string.IsNullOrWhiteSpace(rawConnectionString))
{
    throw new InvalidOperationException("Falta la cadena de conexión 'Supabase'.");
}

// Acepta tanto el formato clave=valor como la URI postgresql:// que copia Supabase.
var connectionString = PostgresConnectionString.Normalize(rawConnectionString);

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

// Sin esto, un 28P01 no permite distinguir entre contraseña mala y usuario mal formado.
// El resumen no incluye la contraseña, sólo su longitud.
app.Logger.LogInformation("Conexión a Supabase: {Resumen}", PostgresConnectionString.Describe(connectionString));

app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    // Nada de UseHttpsRedirection aquí: Render ya redirige HTTP -> HTTPS en el borde,
    // y hacerlo otra vez dentro rompería el health check interno, que llega por HTTP.
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Cerrar sesión. Es POST a propósito: un enlace GET lo dispararía cualquier
// prefetch del navegador y echaría al socio sin que lo haya pedido.
app.MapPost("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(SocioAuth.Scheme);
    return Results.LocalRedirect("/");
});

// Health check de Render. A propósito no toca la base de datos: si Supabase
// tarda en responder no queremos que Render reinicie el contenedor.
app.MapGet("/healthz", () => Results.Text("ok"));

app.Run();
