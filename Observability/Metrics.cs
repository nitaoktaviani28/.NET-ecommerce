using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Exporter;

namespace EcommerceApp.Observability;

public static class Metrics
{
    public static IServiceCollection AddOtelMetrics(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .WithMetrics(builder =>
            {
                builder
                    .SetResourceBuilder(
                        ResourceBuilder.CreateDefault()
                            .AddService(
                                serviceName: Env.ServiceName,
                                serviceVersion: Env.ServiceVersion))

                    // 🔥 Instrumentations
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()

                    // 🔥 Export via OTLP gRPC (RECOMMENDED)
                    .AddOtlpExporter(o =>
                    {
                        o.Endpoint = new Uri(Env.AlloyOtlpGrpcEndpoint);
                        o.Protocol = OtlpExportProtocol.Grpc;
                    });
            });

        Console.WriteLine("✅ Metrics → Alloy → Mimir (via OTLP gRPC)");
        return services;
    }
}
