namespace CiudadDeportivaTudela.Models;

public class Socio
{
    public long Id { get; set; }

    // Se guarda con la letra; el acceso sólo compara los dígitos (ver SocioAuth.SoloDigitos).
    public string? Dni { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Apellidos { get; set; }

    public string? Telefono { get; set; }

    public string? Email { get; set; }

    public string? UrlFoto { get; set; }

    // Columna heredada, ya sin uso en el acceso (ver SocioAuth: usuario = teléfono, PIN = últimos 4 dígitos).
    public long? PinHash { get; set; }

    public long? CategoriaId { get; set; }

    public CategoriaSocio? CategoriaSocio { get; set; }

    public string? NumeroCuenta { get; set; }

    public bool? Activo { get; set; }

    public DateTime? FechaAlta { get; set; }

    public string? Direccion { get; set; }

    // Informativos para la junta: no condicionan el acceso.
    public DateTime? UltimoAcceso { get; set; }

    public DateTime? ValidezPin { get; set; }
}
