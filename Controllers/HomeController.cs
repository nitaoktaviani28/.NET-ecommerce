/**
 * Controllers/HomeController.cs
 * 
 * Equivalent to: handlers/product.go
 * 
 * Home controller untuk product listing.
 * Clean business logic tanpa observability code.
 */

using EcommerceApp.Repository;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceApp.Controllers;

public class HomeController : Controller
{
    private readonly ProductRepository _productRepository;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ProductRepository productRepository, ILogger<HomeController> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    /// <summary>
    /// GET /
    /// Menampilkan list produk.
    /// Equivalent to Home() in Go handlers/product.go.
    /// 
    /// Controller ini TIDAK mengandung kode tracing.
    /// - HTTP request di-trace otomatis oleh OpenTelemetry
    /// - SQL queries di-trace otomatis oleh Npgsql instrumentation
    /// </summary>
    [HttpGet("/")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var products = await _productRepository.GetAllAsync();
            return View("Index", products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Home controller error");
            return StatusCode(500, "Internal Server Error");
        }
    }
}
