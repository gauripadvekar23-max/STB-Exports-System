using SQLite;

namespace STB_Exports.Models;

public class CartItem
{
    [PrimaryKey]
    [AutoIncrement]
    public int CartItemId { get; set; }

    public int CustomerId { get; set; }

    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string ProductImage { get; set; } = string.Empty;


    // =====================================================
    // PRICE
    // =====================================================
    // Weight products:
    // Price = price per 1 KG
    //
    // Piece products:
    // Price = price per 1 PIECE
    // =====================================================

    public decimal Price { get; set; }


    // =====================================================
    // QUANTITY
    // =====================================================
    //
    // Weight:
    // 250  = 250 g
    // 500  = 500 g
    // 750  = 750 g
    // 1000 = 1 kg
    // 1250 = 1.25 kg
    //
    // Piece:
    // 1 = 1 piece
    // 2 = 2 pieces
    // =====================================================

    public int Quantity { get; set; } = 250;


    // =====================================================
    // PRODUCT TYPE
    // =====================================================
    //
    // "Weight"
    // "Piece"
    // =====================================================

    public string QuantityUnit { get; set; } = "Weight";


    // =====================================================
    // TOTAL PRICE
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

            // Price = ₹ per piece
            // Quantity = number of pieces

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
                // Less than 1 KG → grams

                if (Quantity < 1000)
                {
                    return $"{Quantity} g";
                }


                // 1 KG or more → KG

                decimal kg =
                    Quantity / 1000m;

                return $"{kg:0.##} kg";
            }


            // =================================================
            // PIECE
            // =================================================

            return Quantity == 1
                ? "1 piece"
                : $"{Quantity} pieces";
        }
    }


    // =====================================================
    // DISPLAY PRICE
    // =====================================================
    [Ignore]
    public string DisplayPrice
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
                return $"₹{Price:0.00}/kg";
            }

            return $"₹{Price:0.00}/piece";
        }
    }
}
    