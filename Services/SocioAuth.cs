using System.Security.Claims;
using CiudadDeportivaTudela.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace CiudadDeportivaTudela.Services;

/// <summary>
/// Identificación del socio: usuario = teléfono, contraseña = los 4 últimos dígitos de ese
/// mismo teléfono. No hay ningún PIN independiente que gestionar.
/// </summary>
public static class SocioAuth
{
    public const string Scheme = CookieAuthenticationDefaults.AuthenticationScheme;

    public const string ClaimDni = "dni";

    // "SI" si el socio pertenece a la junta directiva (CategoriaSocio.Junta); determina qué ve y
    // qué puede hacer en la app. Los que no son de junta (JUNTA=NO, categoría 7) van capados.
    public const string ClaimJunta = "junta";

    /// <summary>
    /// El DNI se teclea con o sin la letra y a veces con espacios o guiones; sólo se compara la
    /// parte numérica.
    /// </summary>
    public static string SoloDigitos(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return string.Empty;
        }

        return new string(valor.Where(char.IsDigit).ToArray());
    }

    /// <summary>
    /// El teléfono se teclea con o sin prefijo internacional; sólo se comparan los últimos 9
    /// dígitos (longitud de un móvil español sin prefijo).
    /// </summary>
    public static bool TelefonoCoincide(Socio socio, string? telefonoTecleado)
    {
        var esperado = SoloDigitos(socio.Telefono);
        var recibido = SoloDigitos(telefonoTecleado);

        if (esperado.Length == 0 || recibido.Length == 0)
        {
            return false;
        }

        return UltimosDigitos(esperado, 9) == UltimosDigitos(recibido, 9);
    }

    /// <summary>
    /// La contraseña es siempre los 4 últimos dígitos del teléfono registrado: no hay PIN que
    /// mantener aparte.
    /// </summary>
    public static bool PinCoincide(Socio socio, string? pinTecleado)
    {
        var digitosTelefono = SoloDigitos(socio.Telefono);
        var pinRecibido = SoloDigitos(pinTecleado);

        if (digitosTelefono.Length < 4 || pinRecibido.Length == 0)
        {
            return false;
        }

        return UltimosDigitos(digitosTelefono, 4) == pinRecibido;
    }

    private static string UltimosDigitos(string digitos, int cantidad) =>
        digitos.Length > cantidad ? digitos[^cantidad..] : digitos;

    public static bool EsJunta(ClaimsPrincipal usuario) =>
        string.Equals(usuario.FindFirst(ClaimJunta)?.Value, "SI", StringComparison.OrdinalIgnoreCase);

    public static long? SocioId(ClaimsPrincipal usuario)
    {
        var valor = usuario.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return long.TryParse(valor, out var id) ? id : null;
    }

    public static ClaimsPrincipal CrearPrincipal(Socio socio)
    {
        var nombreCompleto = string.Join(' ', new[] { socio.Nombre, socio.Apellidos }
            .Where(p => !string.IsNullOrWhiteSpace(p)));

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, socio.Id.ToString()),
            new(ClaimTypes.Name, string.IsNullOrWhiteSpace(nombreCompleto) ? $"Socio {socio.Id}" : nombreCompleto),
            new(ClaimDni, socio.Dni ?? string.Empty),
            new(ClaimJunta, socio.CategoriaSocio?.Junta ?? "NO"),
        };

        return new ClaimsPrincipal(new ClaimsIdentity(claims, Scheme));
    }
}
