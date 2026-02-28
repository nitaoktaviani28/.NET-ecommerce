using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Exporter;

namespace EcommerceApp.Observability;

public static class Tracing
{
    public static void InitTracing(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // 🔥 Service name (harus konsisten di Grafana Tempo)
        var serviceName = "dotnet-ecommerce";

        // 🔥 Direct to Tempo Distributor (gRPC)
        var otlpEndpoint =
            "http://tempo-distributor.monitoring.svc.cluster.local:4317";

        services.AddOpenTelemetry()
            .ConfigureResource(resource =>
                resource.AddService(serviceName))
            .WithTracing(tracing =>
            {
                tracing
                    // Jangan sampling biar semua trace masuk
                    .SetSampler(new AlwaysOnSampler())

                    // Auto instrumentation ASP.NET Core
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.RecordException = true;
                    })

                    // Auto trace outgoing HTTP
                    .AddHttpClientInstrumentation()

                    // DEBUG sementara (boleh dihapus nanti)
                    .AddConsoleExporter()

                    // 🔥 Kirim ke Tempo
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(otlpEndpoint);
                        options.Protocol = OtlpExportProtocol.Grpc;
                        options.TimeoutMilliseconds = 10000;
                    });
            });
    }
}
