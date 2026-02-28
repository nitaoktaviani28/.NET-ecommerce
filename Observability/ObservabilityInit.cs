/**
 * Observability/ObservabilityInit.cs
 *
 * Single entry point untuk seluruh observability.
 * Dipanggil SEKALI dari Program.cs.
 */

namespace EcommerceApp.Observability;

public static class ObservabilityInit
{
    /// <summary>
    /// Init observability services (Tracing, Profiling, Metrics)
    /// </summary>
    public static void Init(WebApplicationBuilder builder)
    {
        Console.WriteLine("🔍 Initializing observability...");

        // 🔥 Tracing (OpenTelemetry → Tempo)
        builder.Services.InitTracing(builder.Configuration);

        // 🔥 Profiling (Pyroscope)
        Profiling.InitProfiling();

        Console.WriteLine("✅ Observability initialized");
    }

    /// <summary>
    /// Init metrics middleware (after app build)
    /// </summary>
    public static void InitMetrics(WebApplication app)
    {
        app.InitMetrics();
    }
}
