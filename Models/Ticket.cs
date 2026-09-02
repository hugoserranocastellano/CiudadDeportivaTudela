namespace CiudadDeportivaTudela.Models;

public class Ticket
{
    public long Id { get; set; }

    public DateTime? Fecha { get; set; }

    public long? SocioId { get; set; }

    public decimal? ImporteTotal { get; set; } = 0;

    public bool? Revisado { get; set; } = false;

    public string? UrlFotoTicket { get; set; }

    // "abierto" = borrador en curso (se puede recuperar y seguir editando),
    // "pagado" = cerrado tras pasar por el flujo de pago.
    public string Estado { get; set; } = "abierto";

    public string? FormaPago { get; set; }

    public DateTime? CreatedAt { get; set; }

    public Socio? Socio { get; set; }

    public ICollection<TicketLinea> Lineas { get; set; } = new List<TicketLinea>();
}
