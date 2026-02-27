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
        Console.WriteLine("🔍 Initializing tracing...");

        services.AddOpenTelemetry()
            .WithTracing(tracer =>
            {
                tracer
                    // =========================
                    // RESOURCE (SERVICE METADATA)
                    // =========================
                    .SetResourceBuilder(
                        ResourceBuilder.CreateDefault()
                            .AddService(
                                serviceName: configuration["OTEL_SERVICE_NAME"] ?? "dotnet-ecommerce",
                                serviceVersion: "1.0.0"
                            )
                    )

                    // =========================
                    // ROOT HTTP SERVER SPAN
                    // =========================
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.RecordException = true;
                    })

                    // =========================
                    // HTTP CLIENT SPAN
                    // =========================
                    .AddHttpClientInstrumentation()

                    // =========================
                    // POSTGRES (Npgsql.OpenTelemetry)
                    // WAJIB AddSource("Npgsql")
                    // =========================
                    .AddSource("Npgsql")

                    // =========================
                    // EXPORTER → ALLOY → TEMPO
                    // =========================
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(
                            configuration["OTEL_EXPORTER_OTLP_TRACES_ENDPOINT"]
                            ?? "http://alloy.monitoring.svc.cluster.local:4318/v1/traces"
                        );
                    });
            });

        Console.WriteLine("✅ Tracing initialized");
    }
}
