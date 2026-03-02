using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

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
                                Env.ServiceName,
                                serviceVersion: Env.ServiceVersion))
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddOtlpExporter(o =>
                    {
                        o.Endpoint =
                            new Uri($"{Env.AlloyOtlpHttpEndpoint}/v1/metrics");
                    });
            });

        Console.WriteLine("✅ Metrics → Alloy → Mimir");
        return services;
    }
}
