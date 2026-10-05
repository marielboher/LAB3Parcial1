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

    [TempData]
    public string? Mensaje { get; set; }

    public async Task OnGetAsync()
    {
        Productos = await _datos.ObtenerProductosAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Seleccionados.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Seleccione al menos un producto para eliminar.");
            Productos = await _datos.ObtenerProductosAsync();
            return Page();
        }

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
