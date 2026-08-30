using System.ComponentModel.DataAnnotations;

namespace CiudadDeportivaTudela.Models;

public class CategoriaGasto
{
    public long Id { get; set; }

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    public string Descripcion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El tipo de categoría es obligatorio.")]
    public string TipoCategoria { get; set; } = string.Empty;

    public ICollection<GastoSociedad> Gastos { get; set; } = new List<GastoSociedad>();
}
