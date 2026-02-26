/**
 * Observability/Profiling.cs
 * 
 * Equivalent to: observability/profiling.go
 * 
 * Pyroscope profiling setup.
 * Enables continuous CPU and memory profiling.
 */

using Pyroscope;

namespace EcommerceApp.Observability;

public static class Profiling
{
    /// <summary>
    /// Initialize Pyroscope profiling.
    /// Equivalent to initProfiling() in Go.
    /// 
    /// Enables:
    /// - CPU profiling
    /// - Allocation profiling
    /// </summary>
    public static void InitProfiling()
    {
        try
        {
            var serviceName = Env.GetEnv("OTEL_SERVICE_NAME", "dotnet-ecommerce");
            var pyroscopeEndpoint = Env.GetEnv(
                "PYROSCOPE_ENDPOINT",
                "http://pyroscope-distributor.monitoring.svc.cluster.local:4040"
            );

            PyroscopeAgent.Start(new PyroscopeConfig
            {
                ApplicationName = serviceName,
                ServerAddress = pyroscopeEndpoint,
            });

            Console.WriteLine("✅ Pyroscope profiling initialized");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Profiling init failed (non-fatal): {ex.Message}");
        }
    }
}
