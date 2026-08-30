using System.ComponentModel.DataAnnotations;

namespace CiudadDeportivaTudela.Models;

public class TicketLinea
{
    public long Id { get; set; }

    public long? TicketId { get; set; }

    public long? ArticuloId { get; set; }

    public decimal? PrecioUnidad { get; set; }

    [Range(1, 9999, ErrorMessage = "Las unidades deben ser al menos 1.")]
    public int? Unidades { get; set; } = 1;

    public decimal? Subtotal { get; set; }

    public Ticket? Ticket { get; set; }

    public Articulo? Articulo { get; set; }
}
