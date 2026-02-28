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
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// =========================
// 🔥 WAJIB: ENABLE NPGSQL OPENTELEMETRY
// (kalau ini tidak ada → db.Query TIDAK AKAN MUNCUL)
// =========================
AppContext.SetSwitch(
    "Npgsql.EnableActivitySource",
    true);

var builder = WebApplication.CreateBuilder(args);

// =========================
// INISIALISASI OBSERVABILITY (SINGLE ENTRY POINT)
// =========================
// OpenTelemetry + Tracing di-register DI SINI
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
// MIDDLEWARE
// =========================
app.UseRouting();

// Metrics (/metrics)
ObservabilityInit.InitMetrics(app);

// MVC routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// =========================
// INISIALISASI DATABASE (AFTER PIPELINE READY)
// =========================
// Dipindah SETELAH middleware & routing
// supaya tracing sudah stabil
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
