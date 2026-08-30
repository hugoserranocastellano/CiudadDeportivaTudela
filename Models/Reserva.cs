using System.ComponentModel.DataAnnotations;

namespace CiudadDeportivaTudela.Models;

public class Reserva
{
    public long Id { get; set; }

    [Required(ErrorMessage = "La fecha es obligatoria.")]
    public DateOnly Fecha { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    public long? SocioId { get; set; }

    [Range(1, 500, ErrorMessage = "Indica al menos un comensal.")]
    public int? Comensales { get; set; } = 1;

    public bool? Limpieza { get; set; } = false;

    public string? TipoReserva { get; set; }

    public string? Estado { get; set; } = "activa";

    public DateTime? CreatedAt { get; set; }

    public Socio? Socio { get; set; }

    public ICollection<ReservaMesa> ReservaMesas { get; set; } = new List<ReservaMesa>();
}
