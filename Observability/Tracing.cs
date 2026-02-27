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
                    // RESOURCE (SERVICE NAME)
                    // =========================
                    .SetResourceBuilder(
                        ResourceBuilder.CreateDefault()
                            .AddService(
                                serviceName:
                                    Environment.GetEnvironmentVariable("OTEL_SERVICE_NAME")
                                    ?? "dotnet-ecommerce"
                            )
                    )

                    // =========================
                    // HTTP SERVER TRACING
                    // =========================
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.RecordException = true;
                    })

                    // =========================
                    // POSTGRESQL (Npgsql) TRACING
                    // =========================
                    // WAJIB agar query muncul di Tempo
                    .AddNpgsql()

                    // =========================
                    // OTLP EXPORTER → ALLOY → TEMPO
                    // =========================
                    .AddOtlpExporter(options =>
                    {
                        // Endpoint Alloy di AKS
                        options.Endpoint = new Uri(
                            Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT")
                            ?? "http://alloy.monitoring.svc.cluster.local:4318"
                        );

                        // WAJIB karena pakai port 4318 (HTTP)
                        options.Protocol =
                            OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                    });
            });
    }
}
