/**
 * Observability/Env.cs
 *
 * Equivalent to: observability/env.go
 *
 * Centralized environment variable helper
 * for ALL observability configuration.
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

    public static string Environment =>
        GetEnv("ENVIRONMENT", "vm");

    // ============================
    // Tracing (OTLP → Alloy → Tempo)
    // ============================

    /// <summary>
    /// OTLP gRPC endpoint for tracing.
    /// Example: http://20.xx.xx.xx:4317
    /// </summary>
    public static string AlloyOtlpGrpcEndpoint =>
        GetEnv("ALLOY_OTLP_GRPC_ENDPOINT", "http://20.98.125.199:4317");

    /// <summary>
    /// OTLP HTTP endpoint (base).
    /// Used only if needed (metrics / logs via OTLP).
    /// Example: http://20.xx.xx.xx:4318
    /// </summary>
    public static string AlloyOtlpHttpEndpoint =>
        GetEnv("ALLOY_OTLP_HTTP_ENDPOINT", "http://20.98.125.199:4318");

    // ============================
    // Logging (Direct → Loki)
    // ============================

    /// <summary>
    /// Loki HTTP push endpoint.
    /// MUST point directly to Loki Gateway,
    /// NOT Alloy.
    ///
    /// Example:
    /// http://20.xx.xx.xx/loki/api/v1/push
    /// </summary>
    public static string LokiEndpoint =>
        GetEnv(
            "LOKI_ENDPOINT",
            "http://20.72.253.146/loki/api/v1/push"
        );
}
