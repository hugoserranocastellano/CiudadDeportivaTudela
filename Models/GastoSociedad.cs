using System.ComponentModel.DataAnnotations;

namespace CiudadDeportivaTudela.Models;

public class GastoSociedad
{
    public long Id { get; set; }

    public long? CategoriaId { get; set; }

    [Required(ErrorMessage = "La fecha es obligatoria.")]
    public DateOnly Fecha { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [Range(typeof(decimal), "0", "9999999", ErrorMessage = "La cantidad no puede ser negativa.")]
    public decimal Cantidad { get; set; }

    public string? Descripcion { get; set; }

    public CategoriaGasto? Categoria { get; set; }
}
