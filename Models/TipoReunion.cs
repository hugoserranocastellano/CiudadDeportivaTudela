namespace CiudadDeportivaTudela.Models;

public class TipoReunion
{
    public long Id { get; set; }

    public string Descripcion { get; set; } = string.Empty;

    public bool Junta { get; set; }
}
