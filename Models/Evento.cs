namespace CiudadDeportivaTudela.Models;

public class Evento
{
    public long Id { get; set; }

    public string Descripcion { get; set; } = string.Empty;

    public DateTime? Fecha { get; set; }
}
