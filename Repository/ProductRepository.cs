using EcommerceApp.Models;
using EcommerceApp.Observability;
using Npgsql;
using System.Diagnostics;

namespace EcommerceApp.Repository;

public class ProductRepository
{
    private readonly string _connectionString;

    public ProductRepository(IConfiguration configuration)
    {
        _connectionString =
            Environment.GetEnvironmentVariable("DATABASE_DSN")
            ?? "Host=postgres.app.svc.cluster.local;Database=shop;Username=postgres;Password=postgres";
    }

    public async Task<List<Product>> GetAllAsync()
    {
        using var activity =
            Tracing.ActivitySource.StartActivity(
                "list_products",
                ActivityKind.Internal);

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

    public async Task<Product?> GetByIdAsync(int id)
    {
        // 🔥 get_product (Go equivalent)
        using var activity =
            Tracing.ActivitySource.StartActivity(
                "get_product",
                ActivityKind.Internal);

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            "SELECT id, name, price FROM products WHERE id = $1",
            connection);

        cmd.Parameters.AddWithValue(id);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            return null;

        return new Product
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            Price = reader.GetDecimal(2)
        };
    }
}
