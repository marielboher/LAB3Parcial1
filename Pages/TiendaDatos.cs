using System.Data;
using Microsoft.Data.SqlClient;
namespace TiendaOnline.Pages;

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

    public async Task<bool> ExisteCategoriaAsync(int idCategoria)
    {
        const string sql = "SELECT COUNT(*) FROM categorias WHERE idCategoria = @idCategoria";

        await using var conexion = new SqlConnection(_connectionString);
        await using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.Add("@idCategoria", SqlDbType.Int).Value = idCategoria;
        await conexion.OpenAsync();
        return (int)(await comando.ExecuteScalarAsync())! > 0;
    }

    public async Task<List<Producto>> ObtenerProductosAsync(string? nombre = null, int? idCategoria = null)
    {
        const string sql = @"
            SELECT p.idProducto, p.nombre, p.precio, p.idCategoria, c.descripcion
            FROM productos p
            INNER JOIN categorias c ON c.idCategoria = p.idCategoria
            WHERE (@nombre IS NULL OR p.nombre LIKE '%' + @nombre + '%')
              AND (@idCategoria IS NULL OR p.idCategoria = @idCategoria)
            ORDER BY p.nombre";

        var productos = new List<Producto>();
        await using var conexion = new SqlConnection(_connectionString);
        await using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.Add("@nombre", SqlDbType.VarChar, 100).Value =
            string.IsNullOrWhiteSpace(nombre) ? DBNull.Value : nombre.Trim();
        comando.Parameters.Add("@idCategoria", SqlDbType.Int).Value =
            idCategoria.HasValue ? idCategoria.Value : DBNull.Value;
        await conexion.OpenAsync();
        await using var lector = await comando.ExecuteReaderAsync();
        while (await lector.ReadAsync())
        {
            productos.Add(LeerProducto(lector));
        }
        return productos;
    }

    public async Task<Producto?> ObtenerProductoAsync(int idProducto)
    {
        const string sql = @"
            SELECT p.idProducto, p.nombre, p.precio, p.idCategoria, c.descripcion
            FROM productos p
            INNER JOIN categorias c ON c.idCategoria = p.idCategoria
            WHERE p.idProducto = @idProducto";

        await using var conexion = new SqlConnection(_connectionString);
        await using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.Add("@idProducto", SqlDbType.Int).Value = idProducto;
        await conexion.OpenAsync();
        await using var lector = await comando.ExecuteReaderAsync();
        return await lector.ReadAsync() ? LeerProducto(lector) : null;
    }

    public async Task<int> InsertarProductoAsync(Producto producto)
    {
        const string sql = "INSERT INTO productos (nombre, precio, idCategoria) VALUES (@nombre, @precio, @idCategoria)";

        await using var conexion = new SqlConnection(_connectionString);
        await using var comando = new SqlCommand(sql, conexion);
        AgregarParametrosProducto(comando, producto);
        await conexion.OpenAsync();
        return await comando.ExecuteNonQueryAsync();
    }

    public async Task<int> ActualizarProductoAsync(Producto producto)
    {
        const string sql = @"
            UPDATE productos
            SET nombre = @nombre, precio = @precio, idCategoria = @idCategoria
            WHERE idProducto = @idProducto";

        await using var conexion = new SqlConnection(_connectionString);
        await using var comando = new SqlCommand(sql, conexion);
        AgregarParametrosProducto(comando, producto);
        comando.Parameters.Add("@idProducto", SqlDbType.Int).Value = producto.IdProducto;
        await conexion.OpenAsync();
        return await comando.ExecuteNonQueryAsync();
    }

    public async Task<int> EliminarProductosAsync(IReadOnlyList<int> idsProductos)
    {
        if (idsProductos.Count == 0)
            return 0;

        var nombresParametros = idsProductos.Select((_, i) => "@id" + i).ToList();
        var sql = $"DELETE FROM productos WHERE idProducto IN ({string.Join(", ", nombresParametros)})";

        await using var conexion = new SqlConnection(_connectionString);
        await using var comando = new SqlCommand(sql, conexion);
        for (int i = 0; i < idsProductos.Count; i++)
        {
            comando.Parameters.Add(nombresParametros[i], SqlDbType.Int).Value = idsProductos[i];
        }
        await conexion.OpenAsync();
        return await comando.ExecuteNonQueryAsync();
    }

    private static void AgregarParametrosProducto(SqlCommand comando, Producto producto)
    {
        comando.Parameters.Add("@nombre", SqlDbType.VarChar, 100).Value = producto.Nombre.Trim();
        var precio = comando.Parameters.Add("@precio", SqlDbType.Decimal);
        precio.Precision = 10;
        precio.Scale = 2;
        precio.Value = producto.Precio!.Value;
        comando.Parameters.Add("@idCategoria", SqlDbType.Int).Value = producto.IdCategoria!.Value;
    }

    private static Producto LeerProducto(SqlDataReader lector) => new()
    {
        IdProducto = lector.GetInt32(0),
        Nombre = lector.GetString(1),
        Precio = lector.GetDecimal(2),
        IdCategoria = lector.GetInt32(3),
        Categoria = lector.GetString(4)
    };
}
