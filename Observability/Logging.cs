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
    /// - Loki (direct HTTP push, no agent)
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
            .Enrich.WithProperty("service_name", Env.ServiceName)
            .Enrich.WithProperty("service_version", Env.ServiceVersion)
            .Enrich.WithProperty("env", Env.Environment)

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
                        Key = "service_name",
                        Value = Env.ServiceName
                    },
                    new LokiLabel
                    {
                        Key = "env",
                        Value = Env.Environment
                    }
                })

            .CreateLogger();
    }
}
