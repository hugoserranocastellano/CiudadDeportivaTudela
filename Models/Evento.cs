namespace CiudadDeportivaTudela.Models;

public class Evento
{
    public long Id { get; set; }

    public string Descripcion { get; set; } = string.Empty;

    public DateOnly Fecha { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    public string? Caracteristicas { get; set; }

    public bool? Obligatorio { get; set; } = false;

    public int? PlazasMaximas { get; set; }
}
