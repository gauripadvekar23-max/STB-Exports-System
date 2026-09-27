using Microsoft.Maui.Storage;
using STB_Exports.Models;
using System.Collections.ObjectModel;
using System.Net.Http.Json;

namespace STB_Exports.Views;

public partial class WishlistPage : ContentPage
{
    private readonly HttpClient _httpClient;

    private readonly ObservableCollection<ApiWishlistDisplayItem>
        _wishlistItems = new();

    // =====================================================
    // CONSTRUCTOR
    // =====================================================

    public WishlistPage()
    {
        InitializeComponent();

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7197")
        };

        WishlistCollectionView.ItemsSource =
            _wishlistItems;
    }

    // =====================================================
    // PAGE APPEARING
    // =====================================================

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await LoadWishlistAsync();
    }

    // =====================================================
    // LOAD WISHLIST FROM API
    // =====================================================

    private async Task LoadWishlistAsync()
    {
        try
        {
            string customerId =
                Preferences.Get(
                    "ApiCustomerId",
                    string.Empty)
                .Trim();

            _wishlistItems.Clear();

            if (string.IsNullOrWhiteSpace(customerId))
                return;

            using HttpResponseMessage response =
                await _httpClient.GetAsync(
                    $"/api/Wishlist/customer/{customerId}");

            if (!response.IsSuccessStatusCode)
            {
                string error =
                    await response.Content.ReadAsStringAsync();

                await DisplayAlert(
                    "Wishlist Error",
                    string.IsNullOrWhiteSpace(error)
                        ? $"Unable to load wishlist. Status: {(int)response.StatusCode}"
                        : error,
                    "OK");

                return;
            }

            List<ApiWishlistItem>? items =
                await response.Content
                    .ReadFromJsonAsync<List<ApiWishlistItem>>();

            if (items == null)
                return;

            foreach (ApiWishlistItem item in items)
            {
                if (item.Product == null)
                    continue;

                _wishlistItems.Add(
                    new ApiWishlistDisplayItem
                    {
                        WishlistItemId =
                            item.WishlistItemId,

                        CustomerId =
                            item.CustomerId,

                        ProductId =
                            item.ProductId,

                        ProductName =
                            item.Product.Name,

                        ProductImage =
                            item.Product.Image,

                        Price =
                            (decimal)item.Product.Price,

                        Category =
                            item.Product.Category,

                        QuantityUnit =
                            item.Product.QuantityUnit
                    });
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
    // REMOVE FROM WISHLIST
    // =====================================================

    private async void RemoveWishlist_Clicked(
        object sender,
        EventArgs e)
    {
        if (sender is not Button button ||
            button.CommandParameter
                is not ApiWishlistDisplayItem item)
        {
            return;
        }

        bool confirm =
            await DisplayAlert(
                "Remove Wishlist",
                $"Remove {item.ProductName} from your wishlist?",
                "Remove",
                "Cancel");

        if (!confirm)
            return;

        try
        {
            using HttpResponseMessage response =
                await _httpClient.DeleteAsync(
                    $"/api/Wishlist/" +
                    $"{item.CustomerId}/{item.ProductId}");

            if (response.IsSuccessStatusCode)
            {
                _wishlistItems.Remove(item);

                await DisplayAlert(
                    "Wishlist",
                    $"{item.ProductName} has been removed from your wishlist.",
                    "OK");

                return;
            }

            string error =
                await response.Content.ReadAsStringAsync();

            await DisplayAlert(
                "Wishlist Error",
                string.IsNullOrWhiteSpace(error)
                    ? "Unable to remove product."
                    : error,
                "OK");
        }
        catch (HttpRequestException)
        {
            await DisplayAlert(
                "Connection Error",
                "Unable to connect to the API.",
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
    // VIEW PRODUCT DETAILS
    // =====================================================

    private async void Product_Clicked(
        object sender,
        EventArgs e)
    {
        if (sender is not Button button ||
            button.CommandParameter
                is not ApiWishlistDisplayItem item)
        {
            return;
        }

        try
        {
            // =============================================
            // GET PRODUCT DIRECTLY FROM API
            // =============================================

            ApiProduct? apiProduct =
                await _httpClient.GetFromJsonAsync<ApiProduct>(
                    $"/api/Products/{item.ProductId}");

            if (apiProduct == null)
            {
                await DisplayAlert(
                    "Product",
                    "Product details could not be found.",
                    "OK");

                return;
            }

            // =============================================
            // CREATE CUSTOMER PRODUCT
            // =============================================

            Product product =
                new Product
                {
                    ProductId =
                        GetLocalProductId(
                            apiProduct.ProductId),

                    ApiProductId =
                        apiProduct.ProductId,

                    Name =
                        apiProduct.Name,

                    Category =
                        apiProduct.Category,

                    Price =
                        (decimal)apiProduct.Price,

                    QuantityUnit =
                        apiProduct.QuantityUnit,

                    Quantity =
                        (int)apiProduct.Quantity,

                    Image =
                        apiProduct.Image,

                    Description =
                        apiProduct.Description,

                    Images =
                        new List<string>()
                };

            await Navigation.PushAsync(
                new ProductsDetailsPage(product));
        }
        catch (HttpRequestException)
        {
            await DisplayAlert(
                "Connection Error",
                "Unable to connect to the API.",
                "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Product Error",
                ex.Message,
                "OK");
        }
    }

    // =====================================================
    // LOCAL INTEGER ID
    // =====================================================

    private int GetLocalProductId(
        string apiProductId)
    {
        if (int.TryParse(
            apiProductId,
            out int numericId))
        {
            return numericId;
        }

        unchecked
        {
            int hash = 17;

            foreach (char c in apiProductId)
            {
                hash =
                    hash * 31 + c;
            }

            return Math.Abs(hash);
        }
    }
}


// =============================================================
// WISHLIST DISPLAY MODEL
// =============================================================

public class ApiWishlistDisplayItem
{
    public string WishlistItemId { get; set; } =
        string.Empty;

    public string CustomerId { get; set; } =
        string.Empty;

    public string ProductId { get; set; } =
        string.Empty;

    public string ProductName { get; set; } =
        string.Empty;

    public string ProductImage { get; set; } =
        string.Empty;

    public decimal Price { get; set; }

    public string Category { get; set; } =
        string.Empty;

    public string QuantityUnit { get; set; } =
        "Weight";
}