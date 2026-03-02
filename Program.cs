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
// ERROR HANDLING
// =========================
app.UseExceptionHandler("/Home/Error");
app.UseStatusCodePagesWithReExecute("/Home/Error");

// =========================
// STATIC FILES (MVC)
// =========================
app.UseStaticFiles();

// =========================
// ROUTING
// =========================
app.UseRouting();

// =========================
// MVC ROUTES (SATU-SATUNYA ROUTE)
// =========================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

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
