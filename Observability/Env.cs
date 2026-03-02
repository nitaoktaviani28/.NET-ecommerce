namespace EcommerceApp.Observability;

public static class Env
{
    /// <summary>
    /// Get environment variable or return default value.
    /// </summary>
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

    public static string AppEnvironment =>
        GetEnv("ENVIRONMENT", "vm");

    // ============================
    // Tracing (OTLP → Alloy → Tempo)
    // ============================

    /// <summary>
    /// OTLP gRPC endpoint for tracing.
    /// </summary>
    public static string AlloyOtlpGrpcEndpoint =>
        GetEnv("ALLOY_OTLP_GRPC_ENDPOINT", "http://20.98.125.199:4317");

    /// <summary>
    /// OTLP HTTP endpoint (optional).
    /// </summary>
    public static string AlloyOtlpHttpEndpoint =>
        GetEnv("ALLOY_OTLP_HTTP_ENDPOINT", "http://20.98.125.199:4318");

    // ============================
    // Logging (Direct → Loki)
    // ============================

    /// <summary>
    /// Loki HTTP push endpoint.
    /// Direct to Loki Gateway (NOT Alloy).
    /// </summary>
    public static string LokiEndpoint =>
        GetEnv("LOKI_ENDPOINT", "http://20.72.253.146/loki/api/v1/push");
}
