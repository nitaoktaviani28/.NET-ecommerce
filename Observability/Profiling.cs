using Grafana.Pyroscope;

namespace EcommerceApp.Observability;

public static class Profiling
{
    public static void InitProfiling()
    {
        PyroscopeProfiler.Start(new PyroscopeProfilerOptions
        {
            ApplicationName = "ecommerce-app",
            ServerAddress = "http://172.193.209.242:4040",
            Environment = "vm"
        });
    }
}
