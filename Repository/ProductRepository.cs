/**
 * Repository/ProductRepository.cs
 * 
 * Equivalent to: ProductRepository in Go
 * 
 * Product data access.
 * TIDAK ADA kode tracing di sini - otomatis oleh OpenTelemetry.
 */

using EcommerceApp.Models;
using Npgsql;

namespace EcommerceApp.Repository;

public class ProductRepository
{
    private readonly string _connectionString;

    public ProductRepository(IConfiguration configuration)
    {
        _connectionString = Environment.GetEnvironmentVariable("DATABASE_DSN") 
            ?? "Host=postgres.app.svc.cluster.local;Database=shop;Username=postgres;Password=postgres";
    }

    /// <summary>
    /// Get all products.
    /// Equivalent to GetProducts() in Go.
    /// Query akan di-trace otomatis oleh OpenTelemetry Npgsql instrumentation.
    /// </summary>
    public async Task<List<Product>> GetAllAsync()
    {
        var products = new List<Product>();

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            "SELECT id, name, price FROM products ORDER BY id",
            connection);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            products.Add(new Product
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Price = reader.GetDecimal(2)
            });
        }

        return products;
    }

    /// <summary>
    /// Get product by ID.
    /// Equivalent to GetProduct() in Go.
    /// Query akan di-trace otomatis oleh OpenTelemetry Npgsql instrumentation.
    /// </summary>
    public async Task<Product?> GetByIdAsync(int id)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            "SELECT id, name, price FROM products WHERE id = $1",
            connection);
        cmd.Parameters.AddWithValue(id);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Product
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Price = reader.GetDecimal(2)
            };
        }

        return null;
    }
}
