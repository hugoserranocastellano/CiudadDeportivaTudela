namespace CiudadDeportivaTudela.Models;

public class ReservaMesa
{
    public long Id { get; set; }

    public long? ReservaId { get; set; }

    public long? MesaId { get; set; }

    public Reserva? Reserva { get; set; }

    public Mesa? Mesa { get; set; }
}
