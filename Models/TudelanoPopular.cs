using System.ComponentModel.DataAnnotations;

namespace CiudadDeportivaTudela.Models;

public class TudelanoPopular
{
    public long Id { get; set; }

    [Required(ErrorMessage = "El año es obligatorio.")]
    public string Anio { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Nombre { get; set; } = string.Empty;

    public string? UrlFoto { get; set; }
}
