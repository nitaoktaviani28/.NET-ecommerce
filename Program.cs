using EcommerceApp.Observability;

var builder = WebApplication.CreateBuilder(args);

// 🔥 SINGLE ENTRY POINT
ObservabilityInit.Init(builder);

// services lain
builder.Services.AddControllersWithViews();
// ...

var app = builder.Build();

app.UseRouting();
app.MapControllers();

app.Run("http://0.0.0.0:8080");
