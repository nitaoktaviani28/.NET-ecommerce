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
        var serviceName =
            Environment.GetEnvironmentVariable("OTEL_SERVICE_NAME")
            ?? "dotnet-ecommerce";

        // 🔥 DIRECT TO TEMPO (BUKAN ALLOY)
        var otlpEndpoint =
            Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT")
            ?? "http://tempo-distributor.monitoring.svc.cluster.local:4317";

        services.AddOpenTelemetry()
            .ConfigureResource(resource =>
                resource.AddService(serviceName))
            .WithTracing(tracing =>
            {
                tracing
                    .SetSampler(new AlwaysOnSampler())

                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.RecordException = true;
                        options.EnrichWithHttpRequest = (activity, request) =>
                        {
                            activity.SetTag("http.request_content_length", request.ContentLength);
                        };
                    })

                    // DEBUG (sementara, WAJIB ADA)
                    .AddConsoleExporter()

                    // 🔥 KIRIM LANGSUNG KE TEMPO
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(otlpEndpoint);
                        options.Protocol = OtlpExportProtocol.Grpc;
                        options.TimeoutMilliseconds = 10000;
                    });
            });
    }
}
