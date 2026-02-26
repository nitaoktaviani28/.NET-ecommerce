/**
 * Controllers/CheckoutController.cs
 * 
 * Equivalent to: handlers/order.go
 * 
 * Checkout controller untuk order processing.
 * Clean business logic tanpa observability code.
 */

using EcommerceApp.Repository;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceApp.Controllers;

public class CheckoutController : Controller
{
    private readonly ProductRepository _productRepository;
    private readonly OrderRepository _orderRepository;
    private readonly ILogger<CheckoutController> _logger;

    public CheckoutController(
        ProductRepository productRepository,
        OrderRepository orderRepository,
        ILogger<CheckoutController> logger)
    {
        _productRepository = productRepository;
        _orderRepository = orderRepository;
        _logger = logger;
    }

    /// <summary>
    /// POST /checkout
    /// Process order dan redirect ke success page.
    /// Equivalent to Checkout() in Go handlers/order.go.
    /// 
    /// Controller ini TIDAK mengandung kode tracing.
    /// - HTTP request di-trace otomatis oleh OpenTelemetry
    /// - SQL queries di-trace otomatis oleh Npgsql instrumentation
    /// </summary>
    [HttpPost("/checkout")]
    public async Task<IActionResult> Checkout(int product_id, int quantity)
    {
        try
        {
            // Simulate CPU work untuk profiling
            SimulateCpuWork();

            // Get product
            var product = await _productRepository.GetByIdAsync(product_id);
            if (product == null)
            {
                return NotFound("Product not found");
            }

            // Calculate total
            var total = product.Price * quantity;

            // Create order
            var orderId = await _orderRepository.CreateAsync(product_id, quantity, total);

            _logger.LogInformation(
                "Order created: id={OrderId}, product={ProductName}, total={Total}",
                orderId, product.Name, total);

            // Redirect to success page
            return Redirect($"/success?order_id={orderId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Checkout controller error");
            return StatusCode(500, "Internal Server Error");
        }
    }

    /// <summary>
    /// GET /success
    /// Menampilkan order confirmation.
    /// Equivalent to Success() in Go handlers/order.go.
    /// </summary>
    [HttpGet("/success")]
    public async Task<IActionResult> Success(int order_id)
    {
        try
        {
            // Get order
            var order = await _orderRepository.GetByIdAsync(order_id);
            if (order == null)
            {
                return NotFound("Order not found");
            }

            // Get product
            var product = await _productRepository.GetByIdAsync(order.ProductId);

            return View("Success", new { Order = order, Product = product });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Success controller error");
            return StatusCode(500, "Internal Server Error");
        }
    }

    /// <summary>
    /// Simulate CPU-intensive work untuk profiling visibility.
    /// Equivalent to simulateCpuWork() in Go.
    /// 
    /// Fungsi ini akan terlihat di Pyroscope flamegraph.
    /// </summary>
    private void SimulateCpuWork()
    {
        long result = 0;
        for (int i = 0; i < 2000000; i++)
        {
            result += (long)i * i * i;
        }
    }
}
