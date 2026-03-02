using EcommerceApp.Repository;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceApp.Controllers;

public class HomeController : Controller
{
    private readonly ProductRepository _productRepository;
    private readonly ILogger<HomeController> _logger;

    public HomeController(
        ProductRepository productRepository,
        ILogger<HomeController> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    /// <summary>
    /// GET /
    /// Menampilkan list produk.
    /// Routing DIATUR oleh MapControllerRoute (Program.cs)
    /// </summary>
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
            return View("Error");
        }
    }
}
