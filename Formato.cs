using System.Globalization;

namespace TiendaOnline;

public static class Formato
{
    private static readonly CultureInfo Argentina = CultureInfo.GetCultureInfo("es-AR");

    public static string Precio(decimal? precio) =>
        precio.HasValue ? precio.Value.ToString("C2", Argentina) : string.Empty;
}
