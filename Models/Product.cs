/**
 * Models/Product.cs
 * 
 * Equivalent to: Product struct in Go
 */

namespace EcommerceApp.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
