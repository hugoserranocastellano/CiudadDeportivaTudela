namespace CiudadDeportivaTudela.Models;

public class Ticket
{
    public long Id { get; set; }

    public DateTime? Fecha { get; set; }

    public long? SocioId { get; set; }

    public decimal? ImporteTotal { get; set; } = 0;

    public bool? Revisado { get; set; } = false;

    public string? UrlFotoTicket { get; set; }

    public DateTime? CreatedAt { get; set; }

    public Socio? Socio { get; set; }

    public ICollection<TicketLinea> Lineas { get; set; } = new List<TicketLinea>();
}
