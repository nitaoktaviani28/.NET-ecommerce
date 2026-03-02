using EcommerceApp.Observability;

var builder = WebApplication.CreateBuilder(args);

// =========================
// OBSERVABILITY
// =========================
ObservabilityInit.Init(builder);

// =========================
// MVC + API
// =========================
builder.Services.AddControllersWithViews();

var app = builder.Build();

// =========================
// ERROR HANDLING (WAJIB)
// =========================
app.UseExceptionHandler("/Home/Error");
app.UseStatusCodePagesWithReExecute("/Home/Error");

// =========================
// STATIC FILES (WAJIB UNTUK VIEW)
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
// MVC CONTROLLERS (VIEW)
// =========================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// =========================
// ROOT HEALTH (BIAR GAK KOSONG)
// =========================
app.MapGet("/health", () => Results.Ok("OK"));

// =========================
// START SERVER
// =========================
app.Run("http://0.0.0.0:8080");
