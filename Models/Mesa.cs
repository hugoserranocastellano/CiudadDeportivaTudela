using System.ComponentModel.DataAnnotations;

namespace CiudadDeportivaTudela.Models;

public class Mesa
{
    public long Id { get; set; }

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    public string Descripcion { get; set; } = string.Empty;

    public string? Estancia { get; set; }

    [Range(1, 500, ErrorMessage = "La capacidad debe ser al menos 1.")]
    public int Capacidad { get; set; } = 1;

    public bool? Activa { get; set; } = true;

    public ICollection<ReservaMesa> ReservaMesas { get; set; } = new List<ReservaMesa>();
}
