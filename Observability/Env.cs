/**
 * Observability/Env.cs
 *
 * Equivalent to: observability/env.go
 *
 * Environment variable helper + observability config.
 */

namespace EcommerceApp.Observability;

public static class Env
{
    /// <summary>
    /// Get environment variable or return default value.
    /// </summary>
    public static string GetEnv(string key, string defaultValue = "")
    {
        return Environment.GetEnvironmentVariable(key) ?? defaultValue;
    }

    // ============================
    // Service identity
    // ============================

    public static string ServiceName =>
        GetEnv("SERVICE_NAME", "ecommerce-app");

    public static string ServiceVersion =>
        GetEnv("SERVICE_VERSION", "1.0.0");

    // ============================
    // Alloy OTLP endpoints
    // ============================

    /// <summary>
    /// OTLP gRPC endpoint (Tracing → Tempo)
    /// Example: http://20.xx.xx.xx:4317
    /// </summary>
    public static string AlloyOtlpGrpcEndpoint =>
        GetEnv("ALLOY_OTLP_GRPC_ENDPOINT", "http://20.98.125.199:4317");

    /// <summary>
    /// OTLP HTTP base endpoint (Metrics & Logs)
    /// Example: http://20.xx.xx.xx:4318
    /// </summary>
    public static string AlloyOtlpHttpEndpoint =>
        GetEnv("ALLOY_OTLP_HTTP_ENDPOINT", "http://20.98.125.199:4318");
}
