using Serilog;
using Serilog.Events;
using Serilog.Sinks.Grafana.Loki;

namespace EcommerceApp.Observability;

public static class Logging
{
    /// <summary>
    /// Initialize application logging.
    /// Logs are sent to:
    /// - Console
    /// - Loki (via HTTP push, without agent)
    /// </summary>
    public static void Init()
    {
        Log.Logger = new LoggerConfiguration()
            // =========================
            // LOG LEVEL
            // =========================
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)

            // =========================
            // ENRICHMENT (GLOBAL LABELS)
            // =========================
            .Enrich.FromLogContext()
            .Enrich.WithProperty("service", Env.ServiceName)
            .Enrich.WithProperty("version", Env.ServiceVersion)

            // =========================
            // SINKS
            // =========================
            .WriteTo.Console()
            .WriteTo.GrafanaLoki(
                uri: Env.LokiEndpoint,
                labels: new[]
                {
                    new LokiLabel
                    {
                        Key = "service",
                        Value = Env.ServiceName
                    },
                    new LokiLabel
                    {
                        Key = "env",
                        Value = Env.GetEnv("ENVIRONMENT", "vm")
                    }
                })

            .CreateLogger();
    }
}
