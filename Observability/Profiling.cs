/**
 * Observability/Profiling.cs
 *
 * Profiling DISABLED temporarily.
 *
 * Reason:
 * Pyroscope .NET SDK API is not stable and breaks build.
 * Tracing & metrics are prioritized.
 */

namespace EcommerceApp.Observability;

public static class Profiling
{
    /// <summary>
    /// Initialize profiling (disabled).
    /// This keeps build stable and CI green.
    /// </summary>
    public static void InitProfiling()
    {
        // Profiling intentionally disabled
        // Will be enabled later using supported .NET approach
    }
}
