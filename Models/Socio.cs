namespace CiudadDeportivaTudela.Models;

public class Socio
{
    public long Id { get; set; }

    public int NumeroSocio { get; set; }

    public string? Dni { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Apellidos { get; set; }

    public string? Telefono { get; set; }

    public string? Email { get; set; }

    public string? UrlFoto { get; set; }

    public string PinHash { get; set; } = string.Empty;

    public string? PatronHash { get; set; }

    public string Categoria { get; set; } = "socio";

    public string? Cargo { get; set; }

    public string? NumeroCuenta { get; set; }

    public bool? Activo { get; set; }

    public DateTime? FechaAlta { get; set; }
}