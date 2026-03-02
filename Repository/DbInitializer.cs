using Npgsql;

namespace EcommerceApp.Repository;

public class DbInitializer
{
    private readonly string _connectionString;

    public DbInitializer(IConfiguration configuration)
    {
        // PRIORITY:
        // 1. DATABASE_DSN (Docker / K8s)
        // 2. ConnectionStrings:Default
        // 3. Local fallback (Docker Compose)
        _connectionString =
            Environment.GetEnvironmentVariable("DATABASE_DSN")
            ?? configuration.GetConnectionString("Default")
            ?? "Host=postgres;Port=5432;Database=ecommerce;Username=postgres;Password=postgres";
    }

    public async Task InitializeAsync()
    {
        const int maxRetry = 10;
        var delay = TimeSpan.FromSeconds(2);

        for (var attempt = 1; attempt <= maxRetry; attempt++)
        {
            try
            {
                await using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                await CreateTables(connection);
                await SeedData(connection);

                Console.WriteLine("✅ Database initialized successfully");
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"⚠️ DB connection failed (attempt {attempt}/{maxRetry}): {ex.Message}");

                if (attempt == maxRetry)
                    throw;

                await Task.Delay(delay);
            }
        }
    }

    private static async Task CreateTables(NpgsqlConnection connection)
    {
        await using (var cmd = new NpgsqlCommand(@"
            CREATE TABLE IF NOT EXISTS products (
                id SERIAL PRIMARY KEY,
                name TEXT,
                price INT
            )", connection))
        {
            await cmd.ExecuteNonQueryAsync();
        }

        await using (var cmd = new NpgsqlCommand(@"
            CREATE TABLE IF NOT EXISTS orders (
                id SERIAL PRIMARY KEY,
                product_id INT
            )", connection))
        {
            await cmd.ExecuteNonQueryAsync();
        }
    }

    private static async Task SeedData(NpgsqlConnection connection)
    {
        await using var countCmd =
            new NpgsqlCommand("SELECT COUNT(*) FROM products", connection);

        var count = (long)(await countCmd.ExecuteScalarAsync() ?? 0);

        if (count > 0) return;

        var products = new[]
        {
            ("Keyboard", 300000),
            ("Mouse", 150000),
            ("Monitor", 2000000)
        };

        foreach (var (name, price) in products)
        {
            await using var insertCmd =
                new NpgsqlCommand(
                    "INSERT INTO products (name, price) VALUES ($1, $2)",
                    connection);

            insertCmd.Parameters.AddWithValue(name);
            insertCmd.Parameters.AddWithValue(price);
            await insertCmd.ExecuteNonQueryAsync();
        }

        Console.WriteLine("✅ Seed data inserted");
    }
}
