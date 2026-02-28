using EcommerceApp.Models;
using EcommerceApp.Observability;
using Npgsql;
using System.Diagnostics;

namespace EcommerceApp.Repository;

public class OrderRepository
{
    private readonly string _connectionString;

    public OrderRepository(IConfiguration configuration)
    {
        _connectionString = Environment.GetEnvironmentVariable("DATABASE_DSN")
            ?? "Host=postgres.app.svc.cluster.local;Database=shop;Username=postgres;Password=postgres";
    }

    public async Task<int> CreateAsync(int productId, int quantity, decimal total)
    {
        // 🔥 INI YANG NYAMBUNGIN TRACE
        using var activity =
            Tracing.ActivitySource.StartActivity(
                "OrderRepository.Create",
                ActivityKind.Internal);

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            "INSERT INTO orders (product_id, quantity, total) VALUES ($1, $2, $3) RETURNING id",
            connection);
        cmd.Parameters.AddWithValue(productId);
        cmd.Parameters.AddWithValue(quantity);
        cmd.Parameters.AddWithValue(total);

        var orderId = (int)(await cmd.ExecuteScalarAsync() ?? 0);

        return orderId;
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        using var activity =
            Tracing.ActivitySource.StartActivity(
                "OrderRepository.GetById",
                ActivityKind.Internal);

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            "SELECT id, product_id, quantity, total, created_at FROM orders WHERE id = $1",
            connection);
        cmd.Parameters.AddWithValue(id);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Order
            {
                Id = reader.GetInt32(0),
                ProductId = reader.GetInt32(1),
                Quantity = reader.GetInt32(2),
                Total = reader.GetDecimal(3),
                CreatedAt = reader.GetDateTime(4)
            };
        }

        return null;
    }
}
