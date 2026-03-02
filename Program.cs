using EcommerceApp.Observability;
using EcommerceApp.Repository;
using Microsoft.Extensions.Hosting;

// =========================
// 🔥 ENABLE NPGSQL OPENTELEMETRY
// =========================
AppContext.SetSwitch(
    "Npgsql.EnableActivitySource",
    true);

var builder = WebApplication.CreateBuilder(args);

// =========================
// OBSERVABILITY (OTLP → ALLOY)
// =========================
ObservabilityInit.Init(builder);

// =========================
// SERVICES
// =========================
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ProductRepository>();
builder.Services.AddScoped<OrderRepository>();
builder.Services.AddSingleton<DbInitializer>();

var app = builder.Build();

// =========================
// ERROR HANDLING (WAJIB BIAR TIDAK BLANK)
// =========================
app.UseExceptionHandler("/Home/Error");
app.UseStatusCodePagesWithReExecute("/Home/Error");

// =========================
// STATIC FILES (WAJIB UNTUK MVC)
// =========================
app.UseStaticFiles();

// =========================
// ROUTING
// =========================
app.UseRouting();

// =========================
// API CONTROLLERS
// =========================
app.MapControllers();

// =========================
// MVC CONTROLLERS
// =========================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// =========================
// ROOT & HEALTH (DEBUG + OPS FRIENDLY)
// =========================
app.MapGet("/", () => Results.Ok("🚀 E-commerce app is running"));
app.MapGet("/health", () => Results.Ok("OK"));

// =========================
// INIT DATABASE
// =========================
using (var scope = app.Services.CreateScope())
{
    var dbInitializer =
        scope.ServiceProvider.GetRequiredService<DbInitializer>();
    await dbInitializer.InitializeAsync();
}

// =========================
// GRACEFUL SHUTDOWN
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
