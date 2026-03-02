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
            .Enrich.WithProperty("env", Env.GetEnv("ENVIRONMENT", "vm"))

            .WriteTo.Console()

            // 🔥 FILE LOG (INI KUNCI)
            .WriteTo.File(
                path: "/var/log/ecommerce/app.log",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                shared: true
            )

            .CreateLogger();
    }
}
