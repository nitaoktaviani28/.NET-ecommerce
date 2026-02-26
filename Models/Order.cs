/**
 * Models/Order.cs
 * 
 * Equivalent to: Order struct in Go
 */

namespace EcommerceApp.Models;

public class Order
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; }
}
