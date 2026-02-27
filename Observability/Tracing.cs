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
            .WithTracing(tracerProviderBuilder =>
            {
                tracerProviderBuilder
                    .SetResourceBuilder(
                        ResourceBuilder.CreateDefault()
                            .AddService(
                                serviceName: "dotnet-ecommerce",
                                serviceVersion: "1.0.0"
                            )
                    )

                    // ROOT HTTP SPAN
                    .AddAspNetCoreInstrumentation(opt =>
                    {
                        opt.RecordException = true;
                    })

                    // HTTP CLIENT
                    .AddHttpClientInstrumentation()

                    // EXPORTER → ALLOY / TEMPO
                    .AddOtlpExporter(opt =>
                    {
                        opt.Endpoint = new Uri(
                            configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]
                            ?? "http://alloy.monitoring.svc.cluster.local:4318"
                        );
                    });
            });

        Console.WriteLine("✅ Tracing initialized");
    }
}
