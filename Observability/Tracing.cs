using System.Diagnostics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Exporter;

namespace EcommerceApp.Observability;

public static class Tracing
{
    // 🔥 ActivitySource GLOBAL (dipakai semua layer)
    public static readonly ActivitySource ActivitySource =
        new("dotnet-ecommerce");

    public static void InitTracing(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var serviceName = "dotnet-ecommerce";

        var otlpEndpoint =
            "http://tempo-distributor.monitoring.svc.cluster.local:4317";

        services.AddOpenTelemetry()
            .ConfigureResource(resource =>
                resource.AddService(serviceName))
            .WithTracing(tracing =>
            {
                tracing
                    // 🔥 WAJIB: register ActivitySource
                    .AddSource("dotnet-ecommerce")

                    // Root span: incoming HTTP request
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.RecordException = true;
                    })

                    // Outgoing HTTP (kalau ada)
                    .AddHttpClientInstrumentation()

                    // DEBUG (sementara)
                    .AddConsoleExporter()

                    // Export ke Tempo
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(otlpEndpoint);
                        options.Protocol = OtlpExportProtocol.Grpc;
                        options.TimeoutMilliseconds = 10000;
                    })

                    // Jangan sampling dulu biar kelihatan semua
                    .SetSampler(new AlwaysOnSampler());
            });
    }
}
