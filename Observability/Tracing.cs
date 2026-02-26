/**
 * Observability/Tracing.cs
 *
 * OpenTelemetry tracing setup.
 * Exports traces via OTLP HTTP to Grafana Tempo through Alloy.
 */

using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Exporter;

namespace EcommerceApp.Observability;

public static class Tracing
{
    public static void InitTracing(this IServiceCollection services, IConfiguration configuration)
    {
        var serviceName = Env.GetEnv(
            "OTEL_SERVICE_NAME",
            "dotnet-ecommerce"
        );

        var otlpEndpoint = Env.GetEnv(
            "OTEL_EXPORTER_OTLP_ENDPOINT",
            "http://alloy.monitoring.svc.cluster.local:4318"
        );

        services.AddOpenTelemetry()
            .ConfigureResource(resource =>
                resource.AddService(serviceName))
            .WithTracing(tracerProviderBuilder =>
                tracerProviderBuilder
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()

                    // ⬅️ PostgreSQL tracing otomatis via Npgsql.OpenTelemetry

                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri($"{otlpEndpoint}/v1/traces");
                        options.Protocol = OtlpExportProtocol.HttpProtobuf;
                    })
                    .SetSampler(new AlwaysOnSampler())
            );

        Console.WriteLine($"✅ Tracing initialized, exporting to: {otlpEndpoint}");
    }
}
