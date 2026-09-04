using System.ComponentModel.DataAnnotations;

namespace CiudadDeportivaTudela.Models;

public class DocumentoOficial
{
    public long Id { get; set; }

    [Required(ErrorMessage = "El título es obligatorio.")]
    public string Titulo { get; set; } = string.Empty;

    public DateOnly? FechaValidez { get; set; }

    public string Url { get; set; } = string.Empty;
}
