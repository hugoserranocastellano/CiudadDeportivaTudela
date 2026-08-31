namespace CiudadDeportivaTudela.Models;

public class CategoriaSocio
{
    public long Id { get; set; }

    // "SI" / "NO": si pertenece a la junta directiva. Determina qué puede hacer el socio en la app.
    public string Junta { get; set; } = string.Empty;

    public string? TipoSocio { get; set; }
}
