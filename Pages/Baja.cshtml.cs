using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace TiendaOnline.Pages;

public class BajaModel : PageModel
{
    private readonly TiendaDatos _datos;

    public BajaModel(TiendaDatos datos)
    {
        _datos = datos;
    }

    [BindProperty]
    public List<int> Seleccionados { get; set; } = new();

    public List<Producto> Productos { get; set; } = new();

    public bool Confirmando { get; set; }

    [TempData]
    public string? Mensaje { get; set; }

    public async Task OnGetAsync(int? id)
    {
        if (id.HasValue)
            Seleccionados.Add(id.Value);

        Productos = await _datos.ObtenerProductosAsync();
    }

    public async Task<IActionResult> OnPostSeleccionarAsync()
    {
        var todos = await _datos.ObtenerProductosAsync();

        if (Seleccionados.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Seleccione al menos un producto para eliminar.");
            Productos = todos;
            return Page();
        }

        Productos = todos.Where(p => Seleccionados.Contains(p.IdProducto)).ToList();
        Confirmando = true;
        return Page();
    }

    public async Task<IActionResult> OnPostConfirmarAsync()
    {
        if (Seleccionados.Count == 0)
            return RedirectToPage();

        int eliminados;
        try
        {
            eliminados = await _datos.EliminarProductosAsync(Seleccionados.Distinct().ToList());
        }
        catch (SqlException ex)
        {
            ModelState.AddModelError(string.Empty, "No se pudieron eliminar los productos: " + ex.Message);
            Productos = await _datos.ObtenerProductosAsync();
            return Page();
        }

        Mensaje = eliminados == 1
            ? "Se eliminó 1 producto correctamente."
            : $"Se eliminaron {eliminados} productos correctamente.";
        return RedirectToPage();
    }
}
