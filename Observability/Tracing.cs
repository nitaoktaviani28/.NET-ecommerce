using OpenTelemetry;
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
            .ConfigureResource(resource =>
            {
                resource.AddService(
                    serviceName: "dotnet-ecommerce",
                    serviceVersion: "1.0.0"
                );
            })
            .WithTracing(tracing =>
            {
                tracing
                    // =========================
                    // HTTP SERVER (ROOT SPAN)
                    // =========================
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.RecordException = true;
                    })

                    // =========================
                    // HTTP CLIENT (OPTIONAL)
                    // =========================
                    .AddHttpClientInstrumentation()

                    // =========================
                    // POSTGRESQL (INI KUNCI UTAMA)
                    // =========================
                    .AddNpgsql()

                    // =========================
                    // EXPORTER → ALLOY / TEMPO
                    // =========================
                    .AddOtlpExporter(opt =>
                    {
                        opt.Endpoint = new Uri(
                            configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]
                            ?? "http://alloy.monitoring.svc.cluster.local:4318"
                        );
                    });
            });
    }
}
