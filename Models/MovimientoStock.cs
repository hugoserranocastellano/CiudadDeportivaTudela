namespace CiudadDeportivaTudela.Models;

public class MovimientoStock
{
    public long Id { get; set; }

    public long? ArticuloId { get; set; }

    public string? TipoMovimiento { get; set; }

    public int? Cantidad { get; set; }

    public long? TicketId { get; set; }

    public string? Observaciones { get; set; }

    public DateTime? CreatedAt { get; set; }

    public Articulo? Articulo { get; set; }

    public Ticket? Ticket { get; set; }
}
