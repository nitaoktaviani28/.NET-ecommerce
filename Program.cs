using EcommerceApp.Observability;
using EcommerceApp.Repository;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;

// =========================
// 🔥 ENABLE NPGSQL OPENTELEMETRY
// =========================
AppContext.SetSwitch(
    "Npgsql.EnableActivitySource",
    true);

var builder = WebApplication.CreateBuilder(args);

// =========================
// OBSERVABILITY (Logging, Tracing, Metrics)
// =========================
ObservabilityInit.Init(builder);

// =========================
// SERVICES
// =========================
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ProductRepository>();
builder.Services.AddScoped<OrderRepository>();
builder.Services.AddSingleton<DbInitializer>();

// =========================
// ADDING DATABASE CONTEXT AND INIT
// =========================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// =========================
// ERROR HANDLING
// =========================
app.UseExceptionHandler("/Home/Error");
app.UseStatusCodePagesWithReExecute("/Home/Error");

// =========================
// STATIC FILES
// =========================
app.UseStaticFiles();

// =========================
// ROUTING
// =========================
app.UseRouting();

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
// GRACEFUL SHUTDOWN (IMPORTANT)
// =========================
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

lifetime.ApplicationStopping.Register(() =>
{
    Console.WriteLine("Application shutting down gracefully...");
    ObservabilityInit.Shutdown(); // 🔥 WAJIB untuk membersihkan observability resources
    Thread.Sleep(2000); // Menunggu beberapa detik agar observability berhasil shutdown
});

// =========================
// START SERVER
// =========================
Console.WriteLine("🚀 E-commerce ASP.NET Core app starting on :8080");
app.Run("http://0.0.0.0:8080");
