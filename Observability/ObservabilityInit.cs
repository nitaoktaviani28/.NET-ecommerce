/**
 * Observability/ObservabilityInit.cs
 * 
 * Equivalent to: observability/init.go
 * 
 * Single entry point untuk inisialisasi observability.
 * Fungsi Init() dipanggil sekali di Program.cs.
 */

namespace EcommerceApp.Observability;

public static class ObservabilityInit
{
    /// <summary>
    /// Initialize all observability components.
    /// Equivalent to Init() in Go.
    /// 
    /// This is the ONLY function that Program.cs calls.
    /// </summary>
    public static void Init(WebApplicationBuilder builder)
    {
        Console.WriteLine("🔍 Initializing observability...");

        // Initialize tracing (OpenTelemetry → Tempo via Alloy)
        builder.Services.InitTracing(builder.Configuration);

        // Initialize profiling (Pyroscope)
        Profiling.InitProfiling();

        Console.WriteLine("✅ Observability initialized");
    }

    /// <summary>
    /// Initialize metrics middleware.
    /// Called after app is built.
    /// </summary>
    public static void InitMetrics(WebApplication app)
    {
        app.InitMetrics();
    }
}
