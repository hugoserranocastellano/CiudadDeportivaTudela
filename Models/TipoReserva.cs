using System.ComponentModel.DataAnnotations;

namespace CiudadDeportivaTudela.Models;

public class TipoReserva
{
    public long Id { get; set; }

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    public string Descripcion { get; set; } = string.Empty;

    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
