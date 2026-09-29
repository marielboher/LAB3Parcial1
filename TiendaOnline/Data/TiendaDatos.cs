using System.Data;
using Microsoft.Data.SqlClient;
using TiendaOnline.Models;

namespace TiendaOnline.Data;

public class TiendaDatos
{
    private readonly string _connectionString;

    public TiendaDatos(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("TiendaOnline")
            ?? throw new InvalidOperationException("Falta la cadena de conexión 'TiendaOnline' en appsettings.json.");
    }

    public async Task<List<Categoria>> ObtenerCategoriasAsync()
    {
        const string sql = "SELECT idCategoria, descripcion FROM categorias ORDER BY descripcion";

        var categorias = new List<Categoria>();
        await using var conexion = new SqlConnection(_connectionString);
        await using var comando = new SqlCommand(sql, conexion);
        await conexion.OpenAsync();
        await using var lector = await comando.ExecuteReaderAsync();
        while (await lector.ReadAsync())
        {
            categorias.Add(new Categoria
            {
                IdCategoria = lector.GetInt32(0),
                Descripcion = lector.GetString(1)
            });
        }
        return categorias;
    }

    public async Task<int> InsertarProductoAsync(Producto producto)
    {
        const string sql = "INSERT INTO productos (nombre, precio, idCategoria) VALUES (@nombre, @precio, @idCategoria)";

        await using var conexion = new SqlConnection(_connectionString);
        await using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.Add("@nombre", SqlDbType.VarChar, 100).Value = producto.Nombre.Trim();
        comando.Parameters.Add("@precio", SqlDbType.Decimal).Value = producto.Precio!.Value;
        comando.Parameters["@precio"].Precision = 10;
        comando.Parameters["@precio"].Scale = 2;
        comando.Parameters.Add("@idCategoria", SqlDbType.Int).Value = producto.IdCategoria!.Value;
        await conexion.OpenAsync();
        return await comando.ExecuteNonQueryAsync();
    }
}
