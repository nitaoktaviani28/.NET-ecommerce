using Serilog;
using Serilog.Events;
using Serilog.Sinks.Grafana.Loki;

namespace EcommerceApp.Observability;

public static class Logging
{
    public static void Init()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)

            .Enrich.FromLogContext()
            .Enrich.WithProperty("service", Env.ServiceName)
            .Enrich.WithProperty("version", Env.ServiceVersion)
            .Enrich.WithProperty("env", Env.RuntimeEnvironment)

            .WriteTo.Console()
            .WriteTo.GrafanaLoki(
                uri: Env.LokiEndpoint,
                labels: new[]
                {
                    new LokiLabel { Key = "service", Value = Env.ServiceName },
                    new LokiLabel { Key = "env", Value = Env.RuntimeEnvironment }
                })

            .CreateLogger();
    }
}
