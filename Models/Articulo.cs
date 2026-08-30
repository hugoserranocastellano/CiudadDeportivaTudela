using System.ComponentModel.DataAnnotations;

namespace CiudadDeportivaTudela.Models;

public class Articulo
{
    public long Id { get; set; }

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    public string Descripcion { get; set; } = string.Empty;

    public string? UrlFoto { get; set; }

    [Range(0, 999999, ErrorMessage = "El precio no puede ser negativo.")]
    public decimal PrecioUnidad { get; set; }

    public string? Proveedor { get; set; }

    public int? StockInicial { get; set; } = 0;

    public int? StockActual { get; set; } = 0;

    public int? StockMinimo { get; set; } = 0;

    public bool? Activo { get; set; } = true;

    public ICollection<TicketLinea> Lineas { get; set; } = new List<TicketLinea>();

    public ICollection<MovimientoStock> Movimientos { get; set; } = new List<MovimientoStock>();
}
