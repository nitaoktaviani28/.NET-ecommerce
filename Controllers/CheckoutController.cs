/**
 * Controllers/CheckoutController.cs
 *
 * Equivalent to: handlers/order.go
 */

using EcommerceApp.Repository;
using EcommerceApp.Observability;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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

    [HttpPost("/checkout")]
    public async Task<IActionResult> Checkout(int product_id, int quantity)
    {
        // 🔥 EQUIVALENT: checkout_handler (Go)
        using var activity =
            Tracing.ActivitySource.StartActivity(
                "checkout_handler",
                ActivityKind.Internal);

        try
        {
            SimulateCpuWork();

            var product = await _productRepository.GetByIdAsync(product_id);
            if (product == null)
                return NotFound("Product not found");

            var total = product.Price * quantity;

            var orderId =
                await _orderRepository.CreateAsync(product_id, quantity, total);

            _logger.LogInformation(
                "Order created: id={OrderId}, product={ProductName}, total={Total}",
                orderId, product.Name, total);

            return Redirect($"/success?order_id={orderId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Checkout handler error");
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }

    [HttpGet("/success")]
    public async Task<IActionResult> Success(int order_id)
    {
        // 🔥 Sama konsep seperti Go success handler
        using var activity =
            Tracing.ActivitySource.StartActivity(
                "success_handler",
                ActivityKind.Internal);

        try
        {
            var order = await _orderRepository.GetByIdAsync(order_id);
            if (order == null)
                return NotFound("Order not found");

            var product =
                await _productRepository.GetByIdAsync(order.ProductId);

            return View("Success", new { Order = order, Product = product });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Success handler error");
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }

    private void SimulateCpuWork()
    {
        long result = 0;
        for (int i = 0; i < 2_000_000; i++)
            result += (long)i * i * i;
    }
}
