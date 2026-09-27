using System;

namespace STB_Exports.Models;

// =============================================================
// API PRODUCT DTO
// =============================================================


public class ApiProduct
{
    public string ProductId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public double Price { get; set; }

    public string QuantityUnit { get; set; } = "Weight";

    public double Quantity { get; set; } = 250;

    public string Image { get; set; } = string.Empty;

    public string AdditionalImages { get; set; } = "[]";

    public string Video { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}
// =============================================================
// API WISHLIST DTO
// =============================================================

public class ApiWishlistItem
{
    public string WishlistItemId { get; set; } = string.Empty;

    public string CustomerId { get; set; } = string.Empty;

    public string ProductId { get; set; } = string.Empty;

    public DateTime AddedAt { get; set; }

    public ApiProduct? Product { get; set; }
}

// =============================================================
// API CART DTO
// =============================================================

public class ApiCartItem
{
    public string CartItemId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public double Quantity { get; set; }
    public string QuantityUnit { get; set; } = "Weight";
    public double Price { get; set; }

    public ApiProduct? Product { get; set; }
}

// =============================================================
// API CATEGORY DTO
// =============================================================

public class ApiCategory
{
    public string CategoryId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Image { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}