using System.ComponentModel.DataAnnotations;

namespace TiendaOnline.Models;

public class Producto
{
    public int IdProducto { get; set; }

    [Required(ErrorMessage = "Ingrese el nombre.")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingrese el precio.")]
    [Range(typeof(decimal), "0", "99999999.99", ErrorMessage = "El precio debe estar entre 0 y 99.999.999,99.")]
    [Display(Name = "Precio")]
    public decimal? Precio { get; set; }

    [Required(ErrorMessage = "Seleccione una categoría.")]
    [Display(Name = "Categoría")]
    public int? IdCategoria { get; set; }

    public string? Categoria { get; set; }
}
