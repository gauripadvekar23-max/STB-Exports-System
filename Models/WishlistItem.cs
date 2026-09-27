namespace STB_Exports.Models;

public class WishlistItem
{
    public int WishlistItemId { get; set; }

    public int CustomerId { get; set; }

    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string ProductImage { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string Category { get; set; } = string.Empty;
}