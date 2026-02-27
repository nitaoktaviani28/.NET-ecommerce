using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace EcommerceApp.Observability;

public static class Tracing
{
    public static void InitTracing(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOpenTelemetry()
            .WithTracing(tracer =>
            {
                tracer
                    // =========================
                    // RESOURCE / SERVICE NAME
                    // =========================
                    .SetResourceBuilder(
                        ResourceBuilder.CreateDefault()
                            .AddService(
                                Environment.GetEnvironmentVariable("OTEL_SERVICE_NAME")
                                ?? "dotnet-ecommerce"
                            )
                    )

                    // =========================
                    // HTTP TRACING
                    // =========================
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.RecordException = true;
                    })

                    // =========================
                    // CONSOLE EXPORTER (DEBUG)
                    // =========================
                    .AddConsoleExporter()

                    // =========================
                    // OTLP EXPORTER → ALLOY
                    // =========================
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(
                            Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT")
                            ?? "http://alloy.monitoring.svc.cluster.local:4318"
                        );

                        options.Protocol =
                            OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                    });
            });
    }
}
