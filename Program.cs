/**
 * Program.cs
 *
 * Entry point ASP.NET Core e-commerce app.
 * Observability (LGTM via Alloy) diinisialisasi SEKALI di sini.
 */

using EcommerceApp.Observability;
using EcommerceApp.Repository;
using Microsoft.Extensions.Hosting;

// =========================
// 🔥 WAJIB: ENABLE NPGSQL OPENTELEMETRY
// =========================
AppContext.SetSwitch(
    "Npgsql.EnableActivitySource",
    true);

var builder = WebApplication.CreateBuilder(args);

// =========================
// OBSERVABILITY (SINGLE ENTRY POINT)
// =========================
builder.Services.AddTracing();   // → Tempo via Alloy (OTLP gRPC)
builder.Services.AddMetrics();   // → Mimir via Alloy (OTLP HTTP)

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
// MIDDLEWARE
// =========================
app.UseRouting();

// MVC routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// =========================
// INISIALISASI DATABASE
// =========================
// Dijalankan SETELAH pipeline siap
using (var scope = app.Services.CreateScope())
{
    var dbInitializer =
        scope.ServiceProvider.GetRequiredService<DbInitializer>();
    await dbInitializer.InitializeAsync();
}

// =========================
// GRACEFUL SHUTDOWN (TRACE FLUSH FRIENDLY)
// =========================
var lifetime =
    app.Services.GetRequiredService<IHostApplicationLifetime>();

lifetime.ApplicationStopping.Register(() =>
{
    Console.WriteLine("Application shutting down gracefully...");
    Thread.Sleep(2000);
});

// =========================
// START SERVER
// =========================
Console.WriteLine("🚀 E-commerce ASP.NET Core app starting on :8080");
app.Run("http://0.0.0.0:8080");
