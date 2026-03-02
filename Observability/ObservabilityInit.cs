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
    /// Init observability services (Tracing & Metrics).
    /// Profiling Pyroscope diaktifkan via ENV (bukan code).
    /// </summary>
    public static void Init(WebApplicationBuilder builder)
    {
        Console.WriteLine("🔍 Initializing observability...");

        // 🔥 Tracing → Alloy → Tempo
        builder.Services.AddTracing();

        // 🔥 Metrics → Alloy → Mimir
        builder.Services.AddOtelMetrics();

        Console.WriteLine("✅ Observability initialized");
    }
}
