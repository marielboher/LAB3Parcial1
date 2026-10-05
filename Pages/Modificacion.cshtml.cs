using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace TiendaOnline.Pages;

public class ModificacionModel : PageModel
{
    private readonly TiendaDatos _datos;

    public ModificacionModel(TiendaDatos datos)
    {
        _datos = datos;
    }

    [BindProperty]
    public Producto Producto { get; set; } = new();

    public SelectList Categorias { get; set; } = default!;

    public SelectList Productos { get; set; } = default!;

    public bool ProductoSeleccionado { get; set; }

    [TempData]
    public string? Mensaje { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            await CargarProductosAsync();
            return Page();
        }

        var producto = await _datos.ObtenerProductoAsync(id.Value);
        if (producto is null)
        {
            ModelState.AddModelError(string.Empty, $"No existe el producto con ID {id}.");
            await CargarProductosAsync();
            return Page();
        }

        Producto = producto;
        ProductoSeleccionado = true;
        await CargarCategoriasAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        ProductoSeleccionado = true;

        if (ModelState.IsValid && !await _datos.ExisteCategoriaAsync(Producto.IdCategoria!.Value))
        {
            ModelState.AddModelError("Producto.IdCategoria", "La categoría seleccionada no existe.");
        }

        if (!ModelState.IsValid)
        {
            await CargarCategoriasAsync();
            return Page();
        }

        var original = await _datos.ObtenerProductoAsync(Producto.IdProducto);
        if (original is null)
        {
            ModelState.AddModelError(string.Empty, "El producto ya no existe.");
            await CargarCategoriasAsync();
            return Page();
        }

        bool sinCambios = original.Nombre == Producto.Nombre.Trim()
                          && original.Precio == Producto.Precio
                          && original.IdCategoria == Producto.IdCategoria;
        if (sinCambios)
        {
            ModelState.AddModelError(string.Empty, "No se modificó ningún campo. Cambie al menos un dato antes de guardar.");
            await CargarCategoriasAsync();
            return Page();
        }

        try
        {
            await _datos.ActualizarProductoAsync(Producto);
        }
        catch (SqlException ex)
        {
            ModelState.AddModelError(string.Empty, "No se pudo actualizar el producto: " + ex.Message);
            await CargarCategoriasAsync();
            return Page();
        }

        Mensaje = $"Producto \"{Producto.Nombre.Trim()}\" actualizado correctamente.";
        return RedirectToPage(new { id = Producto.IdProducto });
    }

    private async Task CargarCategoriasAsync()
    {
        var categorias = await _datos.ObtenerCategoriasAsync();
        Categorias = new SelectList(categorias, nameof(Categoria.IdCategoria), nameof(Categoria.Descripcion));
    }

    private async Task CargarProductosAsync()
    {
        var productos = await _datos.ObtenerProductosAsync();
        Productos = new SelectList(
            productos.Select(p => new { p.IdProducto, Texto = $"{p.Nombre} ({p.Categoria})" }),
            "IdProducto", "Texto");
    }
}
