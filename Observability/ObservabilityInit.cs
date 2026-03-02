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

        // 🔥 Logging → Loki (direct, no agent)
        Logging.Init();
        builder.Host.UseSerilog();

        // 🔥 Tracing → Alloy → Tempo
        builder.Services.AddTracing();

        // 🔥 Metrics → Alloy → Mimir
        builder.Services.AddOtelMetrics();

        Console.WriteLine("✅ Observability initialized");
    }
}
