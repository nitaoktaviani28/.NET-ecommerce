using System.Diagnostics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Exporter;

namespace EcommerceApp.Observability;

public static class Tracing
{
    // 🔥 ActivitySource GLOBAL (dipakai semua layer app)
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
                    // 🔥 APP SPANS
                    .AddSource("dotnet-ecommerce")

                    // 🔥 DB SPANS (INI KUNCI AGAR db.Query MUNCUL)
                    .AddSource("Npgsql")

                    // Root HTTP span
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.RecordException = true;
                    })

                    // Outgoing HTTP
                    .AddHttpClientInstrumentation()

                    // DEBUG (boleh dihapus nanti)
                    .AddConsoleExporter()

                    // Export ke Tempo
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(otlpEndpoint);
                        options.Protocol = OtlpExportProtocol.Grpc;
                        options.TimeoutMilliseconds = 10000;
                    })

                    // Jangan sampling dulu
                    .SetSampler(new AlwaysOnSampler());
            });
    }
}
