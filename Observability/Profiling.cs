using Pyroscope;

namespace EcommerceApp.Observability;

public static class Profiling
{
    public static void InitProfiling()
    {
        PyroscopeAgent.Start(new PyroscopeAgentOptions
        {
            ApplicationName = "ecommerce-app",
            ServerAddress = "http://172.193.209.242:4040",
            Environment = "vm"
        });
    }
}
