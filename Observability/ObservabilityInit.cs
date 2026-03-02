using Serilog;

namespace EcommerceApp.Observability;

public static class ObservabilityInit
{
    /// <summary>
    /// Init observability services (Logging, Tracing & Metrics).
    /// Dipanggil SEKALI dari Program.cs.
    /// </summary>
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

        Console.WriteLine("✅ Observability initialized");
    }

    /// <summary>
    /// Flush logs on shutdown (IMPORTANT for Loki).
    /// </summary>
    public static void Shutdown()
    {
        Log.CloseAndFlush();
    }
}
