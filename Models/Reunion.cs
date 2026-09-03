namespace CiudadDeportivaTudela.Models;

public class Reunion
{
    public long Id { get; set; }

    public DateOnly FechaPrevista { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public long? TipoReunionId { get; set; }

    // Si el tipo de reunión es de junta, nace no visible para el resto de socios;
    // la junta puede hacerla visible después si lo cree oportuno.
    public bool Visible { get; set; } = true;

    public TipoReunion? TipoReunion { get; set; }

    public List<AdjuntoReunion> Adjuntos { get; set; } = new();
}
