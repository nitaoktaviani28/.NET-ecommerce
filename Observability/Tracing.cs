using System.Diagnostics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Exporter;

namespace EcommerceApp.Observability;

public static class Tracing
{
    // 🔥 Global ActivitySource
    public static readonly ActivitySource ActivitySource =
        new(Env.ServiceName);

    public static IServiceCollection AddTracing(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing
                    .SetResourceBuilder(
                        ResourceBuilder.CreateDefault()
                            .AddService(
                                Env.ServiceName,
                                serviceVersion: Env.ServiceVersion))

                    // App spans
                    .AddSource(Env.ServiceName)

                    // DB spans (Npgsql)
                    .AddSource("Npgsql")

                    // Incoming HTTP
                    .AddAspNetCoreInstrumentation(o =>
                    {
                        o.RecordException = true;
                    })

                    // Outgoing HTTP
                    .AddHttpClientInstrumentation()

                    // 🔥 EXPORT KE ALLOY (BUKAN LANGSUNG TEMPO)
                    .AddOtlpExporter(o =>
                    {
                        o.Endpoint = new Uri(Env.AlloyOtlpGrpcEndpoint);
                        o.Protocol = OtlpExportProtocol.Grpc;
                    })

                    .SetSampler(new AlwaysOnSampler());
            });

        Console.WriteLine("✅ Tracing → Alloy → Tempo");
        return services;
    }
}
