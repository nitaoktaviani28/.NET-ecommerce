/**
 * Observability/Metrics.cs
 *
 * OpenTelemetry metrics setup.
 * Push metrics to Alloy (OTLP HTTP) → Mimir.
 */

using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

namespace EcommerceApp.Observability;

public static class Metrics
{
    public static void AddMetrics(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .WithMetrics(builder =>
            {
                builder
                    .SetResourceBuilder(
                        ResourceBuilder.CreateDefault()
                            .AddService(
                                Env.ServiceName,
                                serviceVersion: Env.ServiceVersion))
                    // ASP.NET Core metrics (requests, latency)
                    .AddAspNetCoreInstrumentation()
                    // HttpClient metrics
                    .AddHttpClientInstrumentation()
                    // .NET runtime metrics (GC, CPU, threads)
                    .AddRuntimeInstrumentation()
                    // Export to Alloy → Mimir
                    .AddOtlpExporter(o =>
                    {
                        o.Endpoint = new Uri($"{Env.AlloyOtlpHttpEndpoint}/v1/metrics");
                    });
            });

        Console.WriteLine("✅ OpenTelemetry metrics enabled (push → Alloy → Mimir)");
    }
}
