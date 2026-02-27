using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Exporter;
 
namespace EcommerceApp.Observability;
 
public static class Tracing
{
    public static void InitTracing(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var serviceName =
            Environment.GetEnvironmentVariable("OTEL_SERVICE_NAME")
            ?? "dotnet-ecommerce";
 
        var otlpEndpoint =
            Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT")
            ?? "http://alloy.monitoring.svc.cluster.local:4318";
 
        services.AddOpenTelemetry()
            .ConfigureResource(resource =>
                resource.AddService(serviceName))
            .WithTracing(tracing =>
            {
                tracing
                    // 🔥 WAJIB: Jangan drop span
                    .SetSampler(new AlwaysOnSampler())
 
                    // 🔥 WAJIB: Auto trace HTTP request
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.RecordException = true;
                        options.EnrichWithHttpRequest = (activity, request) =>
                        {
                            activity.SetTag("http.request_content_length", request.ContentLength);
                        };
                    })
 
                    // 🔥 DEBUG: pastikan span benar-benar dibuat
                    .AddConsoleExporter()
 
                    // 🔥 Kirim ke Alloy → Tempo
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(otlpEndpoint);
                        options.Protocol = OtlpExportProtocol.HttpProtobuf;
 
                        // Timeout biar ga silent fail
                        options.TimeoutMilliseconds = 10000;
                    });
            });
    }
}
