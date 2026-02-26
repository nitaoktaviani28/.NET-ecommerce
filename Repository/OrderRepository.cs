/**
 * Repository/OrderRepository.cs
 * 
 * Equivalent to: OrderRepository in Go
 * 
 * Order data access.
 * TIDAK ADA kode tracing di sini - otomatis oleh OpenTelemetry.
 */

using EcommerceApp.Models;
using EcommerceApp.Observability;
using Npgsql;

namespace EcommerceApp.Repository;

public class OrderRepository
{
    private readonly string _connectionString;

    public OrderRepository(IConfiguration configuration)
    {
        _connectionString = Environment.GetEnvironmentVariable("DATABASE_DSN") 
            ?? "Host=postgres.app.svc.cluster.local;Database=shop;Username=postgres;Password=postgres";
    }

    /// <summary>
    /// Create new order.
    /// Equivalent to CreateOrder() in Go.
    /// Query akan di-trace otomatis oleh OpenTelemetry Npgsql instrumentation.
    /// </summary>
    public async Task<int> CreateAsync(int productId, int quantity, decimal total)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            "INSERT INTO orders (product_id, quantity, total) VALUES ($1, $2, $3) RETURNING id",
            connection);
        cmd.Parameters.AddWithValue(productId);
        cmd.Parameters.AddWithValue(quantity);
        cmd.Parameters.AddWithValue(total);

        var orderId = (int)(await cmd.ExecuteScalarAsync() ?? 0);

        // Increment custom metric
        Metrics.OrdersCreatedTotal.Inc();

        return orderId;
    }

    /// <summary>
    /// Get order by ID.
    /// Equivalent to GetOrder() in Go.
    /// Query akan di-trace otomatis oleh OpenTelemetry Npgsql instrumentation.
    /// </summary>
    public async Task<Order?> GetByIdAsync(int id)
    {
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
