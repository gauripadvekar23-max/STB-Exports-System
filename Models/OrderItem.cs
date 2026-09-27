using SQLite;

namespace STB_Exports.Models;

public class OrderItem
{
    [PrimaryKey]
    [AutoIncrement]
    public int OrderItemId { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public string ProductName { get; set; } = "";

    public string ProductImage { get; set; } = "";

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public string QuantityUnit { get; set; } = "Weight";


    // =====================================================
    // TOTAL
    // =====================================================
    [Ignore]
    public decimal TotalPrice
    {
        get
        {
            string unit =
                (QuantityUnit ?? "")
                .Trim()
                .ToLowerInvariant();

            bool isWeight =
                unit == "weight" ||
                unit == "kg" ||
                unit == "kilogram" ||
                unit == "kilograms" ||
                unit == "gram" ||
                unit == "grams" ||
                unit == "g";

            if (isWeight)
            {
                // Price = ₹ per KG
                // Quantity = grams

                return Price * Quantity / 1000m;
            }

            return Price * Quantity;
        }
    }

    // =====================================================
    // DISPLAY QUANTITY
    // =====================================================

    [Ignore]
    public string DisplayQuantity
    {
        get
        {
            if (string.Equals(
                QuantityUnit,
                "Weight",
                StringComparison.OrdinalIgnoreCase))
            {
                if (Quantity < 1000)
                {
                    return $"{Quantity} g";
                }

                decimal kg =
                    Quantity / 1000m;

                return $"{kg:0.##} kg";
            }

            return Quantity == 1
                ? "1 piece"
                : $"{Quantity} pieces";
        }
    }
}