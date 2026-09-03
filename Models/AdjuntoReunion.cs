namespace CiudadDeportivaTudela.Models;

public class AdjuntoReunion
{
    public long Id { get; set; }

    public long? ReunionId { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    public bool Visible { get; set; }

    public Reunion? Reunion { get; set; }
}
