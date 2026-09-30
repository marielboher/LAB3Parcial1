using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TiendaOnline.Pages;

public class ConsultaModel : PageModel
{
    private readonly TiendaDatos _datos;

    public ConsultaModel(TiendaDatos datos)
    {
        _datos = datos;
    }

    [BindProperty(SupportsGet = true)]
    public string? Nombre { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? IdCategoria { get; set; }

    public List<Producto> Productos { get; set; } = new();

    public SelectList Categorias { get; set; } = default!;

    public bool HayFiltros => !string.IsNullOrWhiteSpace(Nombre) || IdCategoria.HasValue;

    public async Task OnGetAsync()
    {
        var categorias = await _datos.ObtenerCategoriasAsync();
        Categorias = new SelectList(categorias, nameof(Categoria.IdCategoria), nameof(Categoria.Descripcion));
        Productos = await _datos.ObtenerProductosAsync(Nombre, IdCategoria);
    }
}
