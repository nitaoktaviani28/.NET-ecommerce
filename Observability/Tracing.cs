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
        var serviceName = Env.GetEnv("OTEL_SERVICE_NAME", "dotnet-ecommerce");
        var otlpEndpoint = Env.GetEnv(
            "OTEL_EXPORTER_OTLP_ENDPOINT",
            "http://alloy.monitoring.svc.cluster.local:4318"
        );

        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(serviceName: serviceName))
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddNpgsql()
                .AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri($"{otlpEndpoint}/v1/traces");
                    options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                })
                .SetSampler(new AlwaysOnSampler()));

        Console.WriteLine($"✅ Tracing initialized, sending to: {otlpEndpoint}");
    }
}
