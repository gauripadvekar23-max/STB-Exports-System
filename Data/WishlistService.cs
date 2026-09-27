using STB_Exports.Models;

namespace STB_Exports.Data;

public static class WishlistService
{
    // =====================================================
    // CHECK WISHLIST
    // =====================================================

    public static async Task<bool> IsInWishlistAsync(
        int customerId,
        int productId)
    {
        if (customerId <= 0 || productId <= 0)
            return false;

        DatabaseService databaseService =
            new DatabaseService();

        return await databaseService
            .IsProductInWishlistAsync(
                customerId,
                productId);
    }


    // =====================================================
    // ADD TO WISHLIST
    // =====================================================

    public static async Task<bool> AddAsync(
        int customerId,
        Product product)
    {
        if (customerId <= 0 || product == null)
            return false;

        DatabaseService databaseService =
            new DatabaseService();

        WishlistItem item =
            new WishlistItem
            {
                CustomerId = customerId,
                ProductId = product.ProductId,
                ProductName = product.Name,
                ProductImage = product.Image,
                Price = product.Price,
                Category = product.Category
            };

        int result =
            await databaseService
                .AddToWishlistAsync(item);

        return result > 0;
    }


    // =====================================================
    // REMOVE FROM WISHLIST
    // =====================================================

    public static async Task<bool> RemoveAsync(
        int customerId,
        int productId)
    {
        if (customerId <= 0 || productId <= 0)
            return false;

        DatabaseService databaseService =
            new DatabaseService();

        int result =
            await databaseService
                .RemoveFromWishlistAsync(
                    customerId,
                    productId);

        return result > 0;
    }


    // =====================================================
    // TOGGLE WISHLIST
    // =====================================================

    public static async Task<bool> ToggleAsync(
        int customerId,
        Product product)
    {
        if (customerId <= 0 || product == null)
            return false;

        bool exists =
            await IsInWishlistAsync(
                customerId,
                product.ProductId);

        if (exists)
        {
            await RemoveAsync(
                customerId,
                product.ProductId);

            return false;
        }

        await AddAsync(
            customerId,
            product);

        return true;
    }
}