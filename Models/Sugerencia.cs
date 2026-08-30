using System.ComponentModel.DataAnnotations;

namespace CiudadDeportivaTudela.Models;

public class Sugerencia
{
    public long Id { get; set; }

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    public string Descripcion { get; set; } = string.Empty;

    public long? SocioId { get; set; }

    public DateTime? Fecha { get; set; }

    public string? Respuesta { get; set; }

    public bool? Visible { get; set; } = true;

    public DateTime? FechaRespuesta { get; set; }

    public Socio? Socio { get; set; }
}
