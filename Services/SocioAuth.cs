using System.Security.Claims;
using CiudadDeportivaTudela.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace CiudadDeportivaTudela.Services;

/// <summary>
/// Identificación del socio: usuario = número de socio, contraseña = teléfono.
/// </summary>
public static class SocioAuth
{
    public const string Scheme = CookieAuthenticationDefaults.AuthenticationScheme;

    public const string ClaimNumeroSocio = "numero_socio";

    /// <summary>
    /// Los teléfonos se teclean con espacios, guiones o prefijo (+34 948 ...), así que
    /// se comparan sólo los dígitos y, si hay prefijo, los 9 últimos.
    /// </summary>
    public static string SoloDigitos(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return string.Empty;
        }

        var digitos = new string(valor.Where(char.IsDigit).ToArray());

        return digitos.Length > 9 ? digitos[^9..] : digitos;
    }

    public static bool TelefonoCoincide(Socio socio, string? telefonoTecleado)
    {
        var esperado = SoloDigitos(socio.Telefono);
        var recibido = SoloDigitos(telefonoTecleado);

        return esperado.Length > 0 && esperado == recibido;
    }

    public static ClaimsPrincipal CrearPrincipal(Socio socio)
    {
        var nombreCompleto = string.Join(' ', new[] { socio.Nombre, socio.Apellidos }
            .Where(p => !string.IsNullOrWhiteSpace(p)));

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, socio.Id.ToString()),
            new(ClaimTypes.Name, string.IsNullOrWhiteSpace(nombreCompleto)
                ? $"Socio {socio.NumeroSocio}"
                : nombreCompleto),
            new(ClaimNumeroSocio, socio.NumeroSocio.ToString()),
            new(ClaimTypes.Role, string.IsNullOrWhiteSpace(socio.Categoria) ? "socio" : socio.Categoria),
        };

        if (!string.IsNullOrWhiteSpace(socio.Cargo))
        {
            claims.Add(new Claim("cargo", socio.Cargo));
        }

        return new ClaimsPrincipal(new ClaimsIdentity(claims, Scheme));
    }
}
