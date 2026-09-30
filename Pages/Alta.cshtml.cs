using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace TiendaOnline.Pages;

public class AltaModel : PageModel
{
    private readonly TiendaDatos _datos;

    public AltaModel(TiendaDatos datos)
    {
        _datos = datos;
    }

    [BindProperty]
    public Producto Producto { get; set; } = new();

    public SelectList Categorias { get; set; } = default!;

    [TempData]
    public string? Mensaje { get; set; }

    public async Task OnGetAsync()
    {
        await CargarCategoriasAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await CargarCategoriasAsync();
            return Page();
        }

        try
        {
            await _datos.InsertarProductoAsync(Producto);
        }
        catch (SqlException ex)
        {
            ModelState.AddModelError(string.Empty, "No se pudo guardar el producto: " + ex.Message);
            await CargarCategoriasAsync();
            return Page();
        }

        Mensaje = $"Producto \"{Producto.Nombre.Trim()}\" cargado correctamente.";
        return RedirectToPage();
    }

    private async Task CargarCategoriasAsync()
    {
        var categorias = await _datos.ObtenerCategoriasAsync();
        Categorias = new SelectList(categorias, nameof(Categoria.IdCategoria), nameof(Categoria.Descripcion));
    }
}
