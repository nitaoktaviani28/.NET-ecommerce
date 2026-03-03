namespace EcommerceApp.Observability;

public static class Env
{
    public static string GetEnv(string key, string defaultValue = "")
    {
        return System.Environment.GetEnvironmentVariable(key) ?? defaultValue;
    }

    // ============================
    // Service identity
    // ============================

    public static string ServiceName =>
        GetEnv("SERVICE_NAME", "ecommerce-app");

    public static string ServiceVersion =>
        GetEnv("SERVICE_VERSION", "1.0.0");

    public static string RuntimeEnvironment =>
        GetEnv("ENVIRONMENT", "vm");

    // ============================
    // Tracing (OTLP → Alloy → Tempo)
    // ============================

    public static string AlloyOtlpGrpcEndpoint =>
        GetEnv("ALLOY_OTLP_GRPC_ENDPOINT", "http://20.115.192.181:4317");

    public static string AlloyOtlpHttpEndpoint =>
        GetEnv("ALLOY_OTLP_HTTP_ENDPOINT", "http://20.115.192.181:4318");

    // ============================
    // Logging (Direct → Loki)
    // ============================

    public static string LokiEndpoint =>
        GetEnv(
            "LOKI_ENDPOINT",
            "http://4.149.158.209/loki/api/v1/push"
        );
}
