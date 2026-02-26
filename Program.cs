/**
 * Program.cs
 * 
 * Equivalent to: main.go
 * 
 * Entry point aplikasi ASP.NET Core e-commerce.
 * Observability diinisialisasi SEKALI di sini.
 */

using EcommerceApp.Observability;
using EcommerceApp.Repository;

var builder = WebApplication.CreateBuilder(args);

// =========================
// INISIALISASI OBSERVABILITY (SINGLE ENTRY POINT)
// =========================
// Equivalent to observability.Init() in Go main.go
ObservabilityInit.Init(builder);

// =========================
// SERVICES
// =========================
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ProductRepository>();
builder.Services.AddScoped<OrderRepository>();
builder.Services.AddSingleton<DbInitializer>();

// =========================
// BUILD APP
// =========================
var app = builder.Build();

// =========================
// INISIALISASI DATABASE
// =========================
// Equivalent to repository.Init() in Go main.go
using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
    await dbInitializer.InitializeAsync();
}

// =========================
// MIDDLEWARE
// =========================
app.UseRouting();

// Initialize metrics middleware
ObservabilityInit.InitMetrics(app);

// =========================
// MAP CONTROLLERS
// =========================
app.MapControllers();

// =========================
// GRACEFUL SHUTDOWN
// =========================
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStopping.Register(() =>
{
    Console.WriteLine("Application shutting down gracefully...");
});

// =========================
// START SERVER
// =========================
Console.WriteLine("🚀 E-commerce ASP.NET Core app starting on :8080");
app.Run("http://0.0.0.0:8080");
