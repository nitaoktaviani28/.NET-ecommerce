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
                    .SetResourceBuilder(
                        ResourceBuilder.CreateDefault()
                            .AddService("dotnet-ecommerce")
                    )
                    .AddAspNetCoreInstrumentation()
                    .AddOtlpExporter(o =>
                    {
                        o.Endpoint = new Uri("http://alloy.monitoring.svc.cluster.local:4318");
                    });
            });
    }
}
