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
            .WithTracing(builder =>
            {
                builder
                    // =========================
                    // SERVICE METADATA
                    // =========================
                    .SetResourceBuilder(
                        ResourceBuilder.CreateDefault()
                            .AddService(
                                serviceName: configuration["OTEL_SERVICE_NAME"] ?? "dotnet-ecommerce",
                                serviceVersion: "1.0.0"
                            )
                    )

                    // =========================
                    // ASP.NET CORE (ROOT SPAN)
                    // =========================
                    .AddAspNetCoreInstrumentation(opt =>
                    {
                        opt.RecordException = true;
                        opt.Filter = ctx => ctx.Request.Path != "/metrics";
                    })

                    // =========================
                    // HTTP CLIENT
                    // =========================
                    .AddHttpClientInstrumentation()

                    // =========================
                    // POSTGRES (AUTO INSTRUMENTATION)
                    // =========================
                    .AddNpgsql()

                    // =========================
                    // OTLP EXPORTER → ALLOY → TEMPO
                    // =========================
                    .AddOtlpExporter(opt =>
                    {
                        opt.Endpoint = new Uri(
                            (configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]
                             ?? "http://alloy.monitoring.svc.cluster.local:4318")
                            + "/v1/traces"
                        );

                        opt.Protocol =
                            OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                    });
            });

        Console.WriteLine("✅ Tracing initialized");
    }
}
