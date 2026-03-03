using Serilog;

namespace EcommerceApp.Observability;

public static class ObservabilityInit
{
    public static void Init(WebApplicationBuilder builder)
    {
        Console.WriteLine("🔍 Initializing observability...");

        // =========================
        // LOGGING (Serilog → Loki)
        // =========================
        Logging.Init();
        builder.Host.UseSerilog();

        // =========================
        // TRACING (OTel → Alloy → Tempo)
        // =========================
        builder.Services.AddTracing();

        // =========================
        // METRICS (OTel → Alloy → Mimir)
        // =========================
        builder.Services.AddOtelMetrics();

        // =========================
        // PROFILING (Pyroscope)
        // =========================
        Profiling.InitProfiling();

        Console.WriteLine("✅ Observability initialized");
    }

    public static void Shutdown()
    {
        Log.CloseAndFlush();
    }
}
