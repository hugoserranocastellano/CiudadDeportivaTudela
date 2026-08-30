using CiudadDeportivaTudela.Components;
using CiudadDeportivaTudela.Data;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

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

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Health check de Render. A propósito no toca la base de datos: si Supabase
// tarda en responder no queremos que Render reinicie el contenedor.
app.MapGet("/healthz", () => Results.Text("ok"));

app.Run();
