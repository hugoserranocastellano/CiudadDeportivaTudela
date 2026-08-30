namespace CiudadDeportivaTudela.Models;

// La tabla eventos existe en Supabase pero no se ha modelado todavía, así que
// EventoId se guarda como valor suelto (sin navegación) hasta que exista Evento.
public class EventoInscripcion
{
    public long Id { get; set; }

    public long? EventoId { get; set; }

    public long? SocioId { get; set; }

    public bool? Acude { get; set; } = true;

    public bool? ListaEspera { get; set; } = false;

    public DateTime? FechaInscripcion { get; set; }

    public Socio? Socio { get; set; }
}
