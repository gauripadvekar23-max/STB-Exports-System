namespace STB_Exports.Models;

public class Product
{
    public int ProductId { get; set; }

    public string ApiProductId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Image { get; set; } = string.Empty;

    public string BenefitsImage { get; set; } = string.Empty;

    public List<string> Images { get; set; } = new();

    // Product video uploaded from Admin/API
    public string Video { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string QuantityUnit { get; set; } = "Weight";

    public int Quantity { get; set; } = 250;

    public string Description { get; set; } = string.Empty;

    public string PriceDisplay
    {
        get
        {
            if (QuantityUnit == "Piece")
                return $"₹{Price:0.00} / piece";

            return $"₹{Price:0.00} / kg";
        }
    }
}