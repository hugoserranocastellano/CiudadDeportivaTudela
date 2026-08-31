using System.Security.Claims;
using CiudadDeportivaTudela.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace CiudadDeportivaTudela.Services;

/// <summary>
/// Identificación del socio: usuario = DNI sin letra, contraseña = PIN numérico (columna pin_hash).
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

    public static bool DniCoincide(Socio socio, string? dniTecleado)
    {
        var esperado = SoloDigitos(socio.Dni);
        var recibido = SoloDigitos(dniTecleado);

        return esperado.Length > 0 && esperado == recibido;
    }

    public static bool PinCoincide(Socio socio, long? pinTecleado)
    {
        return socio.PinHash.HasValue && pinTecleado.HasValue && socio.PinHash == pinTecleado;
    }

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
