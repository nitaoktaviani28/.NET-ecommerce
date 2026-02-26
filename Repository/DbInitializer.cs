/**
 * Repository/DbInitializer.cs
 * 
 * Equivalent to: database initialization in Go repository
 * 
 * Database setup dan seeding.
 */

using Npgsql;

namespace EcommerceApp.Repository;

public class DbInitializer
{
    private readonly string _connectionString;

    public DbInitializer(IConfiguration configuration)
    {
        _connectionString = Environment.GetEnvironmentVariable("DATABASE_DSN") 
            ?? "Host=postgres.app.svc.cluster.local;Database=shop;Username=postgres;Password=postgres";
    }

    /// <summary>
    /// Initialize database tables dan seed data.
    /// Equivalent to setupDatabase() in Go.
    /// </summary>
    public async Task InitializeAsync()
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        // Create products table
        await using (var cmd = new NpgsqlCommand(@"
            CREATE TABLE IF NOT EXISTS products (
                id SERIAL PRIMARY KEY,
                name VARCHAR(255),
                price DECIMAL(10,2)
            )", connection))
        {
            await cmd.ExecuteNonQueryAsync();
        }

        // Create orders table
        await using (var cmd = new NpgsqlCommand(@"
            CREATE TABLE IF NOT EXISTS orders (
                id SERIAL PRIMARY KEY,
                product_id INTEGER,
                quantity INTEGER,
                total DECIMAL(10,2),
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            )", connection))
        {
            await cmd.ExecuteNonQueryAsync();
        }

        // Seed products if empty
        await using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM products", connection))
        {
            var count = (long)(await cmd.ExecuteScalarAsync() ?? 0L);
            
            if (count == 0)
            {
                var products = new[]
                {
                    ("Gaming Laptop", 15000000m),
                    ("Wireless Mouse", 300000m),
                    ("Mechanical Keyboard", 800000m),
                    ("4K Monitor", 3500000m)
                };

                foreach (var (name, price) in products)
                {
                    await using var insertCmd = new NpgsqlCommand(
                        "INSERT INTO products (name, price) VALUES ($1, $2)",
                        connection);
                    insertCmd.Parameters.AddWithValue(name);
                    insertCmd.Parameters.AddWithValue(price);
                    await insertCmd.ExecuteNonQueryAsync();
                }

                Console.WriteLine("✅ Sample products inserted");
            }
        }

        Console.WriteLine("✅ Database initialized");
    }
}
