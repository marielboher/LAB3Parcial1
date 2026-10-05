using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace TiendaOnline.Pages;

public class Producto : IValidatableObject
{
    private static readonly CultureInfo Argentina = CultureInfo.GetCultureInfo("es-AR");

    public int IdProducto { get; set; }

    [Required(ErrorMessage = "Ingrese el nombre.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres.")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingrese el precio.")]
    [Range(typeof(decimal), "0.01", "99999999.99", ErrorMessage = "El precio debe ser mayor a 0 (máximo 99.999.999,99).")]
    [Display(Name = "Precio")]
    public decimal? Precio { get; set; }

    [Required(ErrorMessage = "Seleccione una categoría.")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione una categoría.")]
    [Display(Name = "Categoría")]
    public int? IdCategoria { get; set; }

    public string? Categoria { get; set; }

    public string PrecioTexto => Precio.HasValue ? Precio.Value.ToString("C2", Argentina) : string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Precio.HasValue && decimal.Round(Precio.Value, 2) != Precio.Value)
        {
            yield return new ValidationResult("El precio puede tener como máximo 2 decimales.", new[] { nameof(Precio) });
        }
    }
}
