/**
 * Observability/Metrics.cs
 * 
 * Equivalent to: observability metrics in Go
 * 
 * Prometheus metrics setup.
 * Exposes /metrics endpoint.
 */

using Prometheus;

namespace EcommerceApp.Observability;

public static class Metrics
{
    // Prometheus metrics (1:1 dengan Go app)
    public static readonly Counter HttpRequestsTotal = Prometheus.Metrics
        .CreateCounter(
            "http_requests_total",
            "Total HTTP requests",
            new CounterConfiguration
            {
                LabelNames = new[] { "method", "endpoint", "status" }
            });

    public static readonly Histogram HttpRequestDuration = Prometheus.Metrics
        .CreateHistogram(
            "http_request_duration_seconds",
            "HTTP request duration",
            new HistogramConfiguration
            {
                LabelNames = new[] { "method", "endpoint" }
            });

    public static readonly Counter OrdersCreatedTotal = Prometheus.Metrics
        .CreateCounter(
            "orders_created_total",
            "Total orders created");

    /// <summary>
    /// Initialize Prometheus metrics.
    /// Equivalent to metrics setup in Go.
    /// </summary>
    public static void InitMetrics(this IApplicationBuilder app)
    {
        // Expose /metrics endpoint
        app.UseMetricServer();
        Console.WriteLine("✅ Metrics initialized, endpoint: /metrics");
    }
}
