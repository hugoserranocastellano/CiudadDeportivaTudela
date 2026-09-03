namespace CiudadDeportivaTudela.Models;

public class Evento
{
    public long Id { get; set; }

    public string Descripcion { get; set; } = string.Empty;

    public DateTime Fecha { get; set; } = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);

    public int? VoluntariosNecesarios { get; set; }

    public DateOnly? FechaTopeApuntarse { get; set; }

    public bool? Obligatorio { get; set; } = false;

    public string? Estado { get; set; } = "abierto";
}
