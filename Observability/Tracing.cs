/**
 * Observability/Tracing.cs
 *
 * Equivalent to: observability/tracing.go
 *
 * OpenTelemetry tracing setup.
 * Exports traces via OTLP HTTP to Grafana Tempo through Alloy.
 */

using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.Npgsql; // 🔥 WAJIB untuk AddNpgsql()

namespace EcommerceApp.Observability;

public static class Tracing
{
    /// <summary>
    /// Initialize OpenTelemetry tracing.
    /// Equivalent to initTracing() in Go.
    ///
    /// Auto-instruments:
    /// - ASP.NET Core (HTTP)
    /// - HttpClient
    /// - Npgsql (PostgreSQL)
    /// </summary>
    public static void InitTracing(this IServiceCollection services, IConfiguration configuration)
    {
        // Nama service (akan muncul di Grafana Tempo)
        var serviceName = Env.GetEnv(
            "OTEL_SERVICE_NAME",
            "dotnet-ecommerce"
        );

        // Endpoint OTLP (Grafana Alloy / Collector)
        var otlpEndpoint = Env.GetEnv(
            "OTEL_EXPORTER_OTLP_ENDPOINT",
            "http://alloy.monitoring.svc.cluster.local:4318"
        );

        services.AddOpenTelemetry()
            .ConfigureResource(resource =>
                resource.AddService(serviceName))
            .WithTracing(tracerProviderBuilder =>
                tracerProviderBuilder
                    // HTTP masuk (Controller)
                    .AddAspNetCoreInstrumentation()

                    // HTTP keluar (HttpClient)
                    .AddHttpClientInstrumentation()

                    // PostgreSQL (Npgsql)
                    .AddNpgsql()

                    // Export ke Tempo via OTLP HTTP
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri($"{otlpEndpoint}/v1/traces");
                        options.Protocol = OtlpExportProtocol.HttpProtobuf;
                    })

                    // Sampling: ambil semua trace (bagus untuk demo & learning)
                    .SetSampler(new AlwaysOnSampler())
            );

        Console.WriteLine($"✅ Tracing initialized, exporting to: {otlpEndpoint}");
    }
}
