using Microsoft.Maui.Storage;
using STB_Exports.Models;
using System.Net;
using System.Net.Http.Json;

namespace STB_Exports.Views;

public partial class ProductsDetailsPage : ContentPage
{
    private readonly Product _product;
    private readonly HttpClient _httpClient;

    // Weight is stored in grams.
    // 250 = 250g
    // 500 = 500g
    // 750 = 750g
    // 1000 = 1kg
    private int _quantity;

    // =====================================================
    // CONSTRUCTOR
    // =====================================================

    public ProductsDetailsPage(Product product)
    {
        InitializeComponent();

        _product = product;

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7197")
        };

        _quantity = IsWeightProduct()
            ? 250
            : 1;

        LoadProduct();
    }

    // =====================================================
    // PAGE APPEARING
    // =====================================================

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await UpdateWishlistButtonAsync();
        await LoadReviewsAsync();
        await UpdateReviewButtonAsync();
    }

    // =====================================================
    // PRODUCT UNIT
    // =====================================================

    private bool IsWeightProduct()
    {
        string unit =
            (_product.QuantityUnit ?? "")
            .Trim()
            .ToLowerInvariant();

        return unit == "weight" ||
               unit == "kg" ||
               unit == "kilogram" ||
               unit == "kilograms" ||
               unit == "gram" ||
               unit == "grams" ||
               unit == "g";
    }

    // =====================================================
    // LOAD PRODUCT
    // =====================================================

    private void LoadProduct()
    {
        ProductNameLabel.Text =
            _product.Name;

        ProductCategoryLabel.Text =
            _product.Category;

        ProductDescriptionLabel.Text =
            _product.Description;

        // =================================================
        // PRICE
        // =================================================

        ProductPriceLabel.Text =
            _product.Price.ToString("0.00");

        // =================================================
        // PRICE UNIT
        // =================================================

        ProductPriceUnitLabel.Text =
            IsWeightProduct()
                ? "kg"
                : "piece";

        // =================================================
        // QUANTITY
        // =================================================

        UpdateQuantityDisplay();

        // =================================================
        // TOTAL
        // =================================================

        UpdateTotal();

        // =================================================
        // BENEFITS
        // =================================================

        if (!string.IsNullOrWhiteSpace(
            _product.BenefitsImage))
        {
            BenefitsImageView.Source =
                _product.BenefitsImage;

            BenefitsFrame.IsVisible =
                true;
        }
        else
        {
            BenefitsFrame.IsVisible =
                false;
        }

        // =================================================
        // PRODUCT IMAGES
        // =================================================

        if (_product.Images != null &&
            _product.Images.Count > 0)
        {
            ProductImagesCarousel.ItemsSource =
                _product.Images;
        }
        else if (!string.IsNullOrWhiteSpace(
            _product.Image))
        {
            ProductImagesCarousel.ItemsSource =
                new List<string>
                {
                    _product.Image
                };
        }
    }

    // =====================================================
    // QUANTITY DISPLAY
    // =====================================================

    private void UpdateQuantityDisplay()
    {
        if (IsWeightProduct())
        {
            QuantityLabel.Text =
                FormatWeight(_quantity);
        }
        else
        {
            QuantityLabel.Text =
                _quantity == 1
                    ? "1 piece"
                    : $"{_quantity} pieces";
        }
    }

    // =====================================================
    // FORMAT WEIGHT
    // =====================================================

    private string FormatWeight(int grams)
    {
        if (grams < 1000)
        {
            return $"{grams} g";
        }

        decimal kg =
            grams / 1000m;

        return kg % 1 == 0
            ? $"{kg:0} kg"
            : $"{kg:0.##} kg";
    }

    // =====================================================
    // CALCULATE TOTAL
    // =====================================================

    private decimal CalculateTotal()
    {
        if (IsWeightProduct())
        {
            return
                _product.Price *
                _quantity /
                1000m;
        }

        return
            _product.Price *
            _quantity;
    }

    // =====================================================
    // UPDATE TOTAL
    // =====================================================

    private void UpdateTotal()
    {
        decimal total =
            CalculateTotal();

        ProductTotalLabel.Text =
            total.ToString("0.00");
    }

    // =====================================================
    // DECREASE QUANTITY
    // =====================================================

    private void DecreaseQuantityButton_Clicked(
        object sender,
        EventArgs e)
    {
        if (IsWeightProduct())
        {
            if (_quantity > 250)
            {
                _quantity -= 250;
            }
        }
        else
        {
            if (_quantity > 1)
            {
                _quantity--;
            }
        }

        UpdateQuantityDisplay();
        UpdateTotal();
    }

    // =====================================================
    // INCREASE QUANTITY
    // =====================================================

    private void IncreaseQuantityButton_Clicked(
        object sender,
        EventArgs e)
    {
        if (IsWeightProduct())
        {
            _quantity += 250;
        }
        else
        {
            _quantity++;
        }

        UpdateQuantityDisplay();
        UpdateTotal();
    }

    // =====================================================
    // WISHLIST BUTTON
    // =====================================================

    private async void WishlistButton_Clicked(
        object sender,
        EventArgs e)
    {
        try
        {
            string customerId =
                Preferences.Get(
                    "ApiCustomerId",
                    string.Empty);

            if (string.IsNullOrWhiteSpace(customerId))
            {
                await DisplayAlert(
                    "Login Required",
                    "Please login before adding products to your wishlist.",
                    "OK");

                return;
            }

            if (string.IsNullOrWhiteSpace(
                _product.ApiProductId))
            {
                await DisplayAlert(
                    "Wishlist Error",
                    "API Product ID is missing for this product.",
                    "OK");

                return;
            }

            // =================================================
            // CHECK CURRENT WISHLIST
            // =================================================

            using HttpResponseMessage getResponse =
                await _httpClient.GetAsync(
                    $"/api/Wishlist/customer/{customerId}");

            if (!getResponse.IsSuccessStatusCode)
            {
                string error =
                    await getResponse.Content.ReadAsStringAsync();

                await DisplayAlert(
                    "Wishlist Error",
                    $"Unable to check wishlist.\n\n{error}",
                    "OK");

                return;
            }

            List<ApiWishlistItem>? wishlist =
                await getResponse.Content
                    .ReadFromJsonAsync<List<ApiWishlistItem>>();

            bool isInWishlist =
                wishlist?.Any(w =>
                    string.Equals(
                        w.ProductId?.Trim(),
                        _product.ApiProductId?.Trim(),
                        StringComparison.OrdinalIgnoreCase)) == true;

            // =================================================
            // REMOVE FROM WISHLIST
            // =================================================

            if (isInWishlist)
            {
                using HttpResponseMessage deleteResponse =
                    await _httpClient.DeleteAsync(
                        $"/api/Wishlist/{customerId}/{_product.ApiProductId}");

                if (deleteResponse.IsSuccessStatusCode)
                {
                    await UpdateWishlistButtonAsync();

                    await DisplayAlert(
                        "Wishlist",
                        $"{_product.Name} has been removed from your wishlist.",
                        "OK");
                }
                else
                {
                    string error =
                        await deleteResponse.Content.ReadAsStringAsync();

                    await DisplayAlert(
                        "Wishlist Error",
                        error,
                        "OK");
                }

                return;
            }

            // =================================================
            // ADD TO WISHLIST
            // =================================================

            ApiWishlistItem wishlistItem =
                new ApiWishlistItem
                {
                    CustomerId =
                        customerId,

                    ProductId =
                        _product.ApiProductId,

                    AddedAt =
                        DateTime.UtcNow
                };

            using HttpResponseMessage postResponse =
                await _httpClient.PostAsJsonAsync(
                    "/api/Wishlist",
                    wishlistItem);

            if (postResponse.IsSuccessStatusCode)
            {
                await UpdateWishlistButtonAsync();

                await DisplayAlert(
                    "Wishlist",
                    $"{_product.Name} has been added to your wishlist.",
                    "OK");
            }
            else
            {
                string error =
                    await postResponse.Content.ReadAsStringAsync();

                if (postResponse.StatusCode ==
                    HttpStatusCode.Conflict)
                {
                    await UpdateWishlistButtonAsync();

                    await DisplayAlert(
                        "Wishlist",
                        $"{_product.Name} is already in your wishlist.",
                        "OK");

                    return;
                }

                await DisplayAlert(
                    "Wishlist Error",
                    error,
                    "OK");
            }
        }
        catch (HttpRequestException)
        {
            await DisplayAlert(
                "Connection Error",
                "Unable to connect to the API.\n\n" +
                "Please make sure STB_Exports_API is running.",
                "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Wishlist Error",
                ex.Message,
                "OK");
        }
    }

    // =====================================================
    // UPDATE WISHLIST BUTTON
    // =====================================================

    private async Task UpdateWishlistButtonAsync()
    {
        try
        {
            string customerId =
                Preferences.Get(
                    "ApiCustomerId",
                    string.Empty);

            if (string.IsNullOrWhiteSpace(customerId) ||
                string.IsNullOrWhiteSpace(
                    _product.ApiProductId))
            {
                WishlistButton.Text =
                    "♡  Add to Wishlist";

                WishlistButton.TextColor =
                    Colors.MidnightBlue;

                return;
            }

            using HttpResponseMessage response =
                await _httpClient.GetAsync(
                    $"/api/Wishlist/customer/{customerId}");

            if (!response.IsSuccessStatusCode)
            {
                WishlistButton.Text =
                    "♡  Add to Wishlist";

                WishlistButton.TextColor =
                    Colors.MidnightBlue;

                return;
            }

            List<ApiWishlistItem>? wishlist =
                await response.Content
                    .ReadFromJsonAsync<List<ApiWishlistItem>>();

            bool isInWishlist =
                wishlist?.Any(w =>
                    string.Equals(
                        w.ProductId?.Trim(),
                        _product.ApiProductId?.Trim(),
                        StringComparison.OrdinalIgnoreCase)) == true;

            if (isInWishlist)
            {
                WishlistButton.Text =
                    "♥  Added to Wishlist";

                WishlistButton.TextColor =
                    Colors.Goldenrod;
            }
            else
            {
                WishlistButton.Text =
                    "♡  Add to Wishlist";

                WishlistButton.TextColor =
                    Colors.MidnightBlue;
            }
        }
        catch
        {
            WishlistButton.Text =
                "♡  Add to Wishlist";

            WishlistButton.TextColor =
                Colors.MidnightBlue;
        }
    }

    // =====================================================
    // ADD TO CART
    // =====================================================

    private async void AddToCartButton_Clicked(
        object sender,
        EventArgs e)
    {
        try
        {
            string customerId =
                Preferences.Get(
                    "ApiCustomerId",
                    string.Empty);

            if (string.IsNullOrWhiteSpace(customerId))
            {
                await DisplayAlert(
                    "Login Required",
                    "Please login before adding products to your cart.",
                    "OK");

                return;
            }

            if (string.IsNullOrWhiteSpace(
                _product.ApiProductId))
            {
                await DisplayAlert(
                    "Cart Error",
                    "API Product ID is missing for this product.",
                    "OK");

                return;
            }

            decimal total =
                CalculateTotal();

            ApiCartItem cartItem =
                new ApiCartItem
                {
                    CustomerId =
                        customerId,

                    ProductId =
                        _product.ApiProductId,

                    Quantity =
                        _quantity,

                    QuantityUnit =
                        IsWeightProduct()
                            ? "Weight"
                            : "Piece",

                    Price =
                        (double)_product.Price
                };

            using HttpResponseMessage response =
                await _httpClient.PostAsJsonAsync(
                    "/api/CartItems",
                    cartItem);

            if (response.IsSuccessStatusCode)
            {
                string quantityText;

                if (IsWeightProduct())
                {
                    quantityText =
                        FormatWeight(_quantity);
                }
                else
                {
                    quantityText =
                        _quantity == 1
                            ? "1 piece"
                            : $"{_quantity} pieces";
                }

                await DisplayAlert(
                    "Added to Cart",
                    $"{_product.Name}\n\n" +
                    $"Quantity: {quantityText}\n" +
                    $"Price: ₹{_product.Price:0.00}/" +
                    $"{(IsWeightProduct()
                        ? "kg"
                        : "piece")}\n" +
                    $"Total: ₹{total:0.00}",
                    "OK");

                return;
            }

            string error =
                await response.Content.ReadAsStringAsync();

            await DisplayAlert(
                "Cart Error",
                string.IsNullOrWhiteSpace(error)
                    ? $"API returned status code {(int)response.StatusCode}."
                    : error,
                "OK");
        }
        catch (HttpRequestException)
        {
            await DisplayAlert(
                "Connection Error",
                "Unable to connect to the API.\n\n" +
                "Please make sure STB_Exports_API is running.",
                "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Cart Error",
                ex.Message,
                "OK");
        }
    }

    // =====================================================
    // WRITE REVIEW
    // =====================================================

    private async void WriteReviewButton_Clicked(
        object sender,
        EventArgs e)
    {
        await DisplayAlert(
            "Review",
            "Review functionality will be connected to the API next.",
            "OK");
    }

    // =====================================================
    // LOAD REVIEWS
    // =====================================================

    private async Task LoadReviewsAsync()
    {
        try
        {
            ProductReviewsCollection.ItemsSource =
                null;

            NoReviewsLabel.IsVisible =
                true;

            AverageRatingLabel.Text =
                "No rating yet";

            AverageRatingLabel2.Text =
                "No rating yet";

            ReviewCountLabel.Text =
                "(0 reviews)";

            ReviewCountLabel2.Text =
                "(0 reviews)";
        }
        catch
        {
            ProductReviewsCollection.ItemsSource =
                null;

            NoReviewsLabel.IsVisible =
                true;
        }

        await Task.CompletedTask;
    }

    // =====================================================
    // UPDATE REVIEW BUTTON
    // =====================================================

    private async Task UpdateReviewButtonAsync()
    {
        try
        {
            WriteReviewButton.IsVisible =
                false;
        }
        catch
        {
            WriteReviewButton.IsVisible =
                false;
        }

        await Task.CompletedTask;
    }
}