using Microsoft.Maui.Storage;
using STB_Exports.Models;
using System.Collections.ObjectModel;
using System.Net.Http.Json;

namespace STB_Exports.Views;

public partial class CartPage : ContentPage
{
    private readonly HttpClient _httpClient;

    private readonly ObservableCollection<ApiCartDisplayItem>
        _cartItems = new();

    // =====================================================
    // CONSTRUCTOR
    // =====================================================

    public CartPage()
    {
        InitializeComponent();

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7197")
        };

        CartCollectionView.ItemsSource = _cartItems;
    }

    // =====================================================
    // PAGE APPEARING
    // =====================================================

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await LoadCartAsync();
    }

    // =====================================================
    // LOAD CART FROM API
    // =====================================================

    private async Task LoadCartAsync()
    {
        try
        {
            string customerId =
                Preferences.Get(
                    "ApiCustomerId",
                    string.Empty)
                .Trim();

            _cartItems.Clear();

            TotalAmountLabel.Text = "₹0.00";

            if (string.IsNullOrWhiteSpace(customerId))
                return;

            using HttpResponseMessage response =
                await _httpClient.GetAsync(
                    $"/api/CartItems/customer/{customerId}");

            if (!response.IsSuccessStatusCode)
            {
                string error =
                    await response.Content.ReadAsStringAsync();

                await DisplayAlert(
                    "Cart Error",
                    string.IsNullOrWhiteSpace(error)
                        ? $"Unable to load cart. Status: {(int)response.StatusCode}"
                        : error,
                    "OK");

                return;
            }

            List<ApiCartItem>? apiItems =
                await response.Content
                    .ReadFromJsonAsync<List<ApiCartItem>>();

            if (apiItems == null)
                return;

            foreach (ApiCartItem item in apiItems)
            {
                _cartItems.Add(
                    new ApiCartDisplayItem
                    {
                        CartItemId = item.CartItemId,
                        CustomerId = item.CustomerId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        QuantityUnit = item.QuantityUnit,
                        Price = (decimal)item.Price,

                        ProductName =
                            item.Product?.Name
                            ?? "Product",

                        ProductImage =
                            item.Product?.Image
                            ?? string.Empty
                    });
            }

            UpdateTotal();
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
    // UPDATE TOTAL
    // =====================================================

    private void UpdateTotal()
    {
        decimal total = 0m;

        foreach (ApiCartDisplayItem item in _cartItems)
        {
            total += item.TotalPrice;
        }

        TotalAmountLabel.Text =
            $"₹{total:0.00}";
    }

    // =====================================================
    // INCREASE QUANTITY
    // =====================================================

    private async void IncreaseQuantity_Clicked(
        object sender,
        EventArgs e)
    {
        if (sender is not Button button ||
            button.CommandParameter is not ApiCartDisplayItem item)
        {
            return;
        }

        try
        {
            double newQuantity;

            if (IsWeightProduct(item.QuantityUnit))
            {
                newQuantity =
                    item.Quantity + 250;
            }
            else
            {
                newQuantity =
                    item.Quantity + 1;
            }

            await UpdateCartQuantityAsync(
                item,
                newQuantity);
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
    // DECREASE QUANTITY
    // =====================================================

    private async void DecreaseQuantity_Clicked(
        object sender,
        EventArgs e)
    {
        if (sender is not Button button ||
            button.CommandParameter is not ApiCartDisplayItem item)
        {
            return;
        }

        try
        {
            if (IsWeightProduct(item.QuantityUnit))
            {
                if (item.Quantity > 250)
                {
                    await UpdateCartQuantityAsync(
                        item,
                        item.Quantity - 250);
                }
                else
                {
                    await DeleteCartItemAsync(item);
                }
            }
            else
            {
                if (item.Quantity > 1)
                {
                    await UpdateCartQuantityAsync(
                        item,
                        item.Quantity - 1);
                }
                else
                {
                    await DeleteCartItemAsync(item);
                }
            }
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
    // UPDATE CART QUANTITY API
    // =====================================================

    private async Task UpdateCartQuantityAsync(
        ApiCartDisplayItem item,
        double quantity)
    {
        var updateData = new
        {
            cartItemId = item.CartItemId,
            customerId = item.CustomerId,
            productId = item.ProductId,
            quantity = quantity,
            quantityUnit = item.QuantityUnit,
            price = (double)item.Price
        };

        using HttpResponseMessage response =
            await _httpClient.PutAsJsonAsync(
                $"/api/CartItems/{item.CartItemId}",
                updateData);

        if (!response.IsSuccessStatusCode)
        {
            string error =
                await response.Content.ReadAsStringAsync();

            await DisplayAlert(
                "Cart Error",
                string.IsNullOrWhiteSpace(error)
                    ? $"Unable to update cart. Status: {(int)response.StatusCode}"
                    : error,
                "OK");

            return;
        }

        await LoadCartAsync();
    }

    // =====================================================
    // REMOVE ITEM
    // =====================================================

    private async void RemoveItem_Clicked(
        object sender,
        EventArgs e)
    {
        if (sender is not Button button ||
            button.CommandParameter is not ApiCartDisplayItem item)
        {
            return;
        }

        bool confirm =
            await DisplayAlert(
                "Remove Item",
                $"Remove {item.ProductName} from your cart?",
                "Remove",
                "Cancel");

        if (!confirm)
            return;

        await DeleteCartItemAsync(item);
    }

    // =====================================================
    // DELETE CART ITEM API
    // =====================================================

    private async Task DeleteCartItemAsync(
        ApiCartDisplayItem item)
    {
        try
        {
            using HttpResponseMessage response =
                await _httpClient.DeleteAsync(
                    $"/api/CartItems/{item.CartItemId}");

            if (!response.IsSuccessStatusCode)
            {
                string error =
                    await response.Content.ReadAsStringAsync();

                await DisplayAlert(
                    "Cart Error",
                    string.IsNullOrWhiteSpace(error)
                        ? "Unable to remove item."
                        : error,
                    "OK");

                return;
            }

            await LoadCartAsync();
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
                "Cart Error",
                ex.Message,
                "OK");
        }
    }

    // =====================================================
    // CLEAR CART
    // =====================================================

    private async void ClearCartButton_Clicked(
        object sender,
        EventArgs e)
    {
        string customerId =
            Preferences.Get(
                "ApiCustomerId",
                string.Empty)
            .Trim();

        if (string.IsNullOrWhiteSpace(customerId))
            return;

        if (_cartItems.Count == 0)
            return;

        bool confirm =
            await DisplayAlert(
                "Clear Cart",
                "Are you sure you want to remove all products?",
                "Clear",
                "Cancel");

        if (!confirm)
            return;

        try
        {
            foreach (ApiCartDisplayItem item in
                     _cartItems.ToList())
            {
                using HttpResponseMessage response =
                    await _httpClient.DeleteAsync(
                        $"/api/CartItems/{item.CartItemId}");

                if (!response.IsSuccessStatusCode)
                    break;
            }

            await LoadCartAsync();
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
    // CHECKOUT
    // =====================================================

    private async void CheckoutButton_Clicked(
        object sender,
        EventArgs e)
    {
        if (_cartItems.Count == 0)
        {
            await DisplayAlert(
                "Cart Empty",
                "Please add products to your cart first.",
                "OK");

            return;
        }

        await Navigation.PushAsync(
            new CheckoutPage());
    }

    // =====================================================
    // PRODUCT TYPE
    // =====================================================

    private bool IsWeightProduct(
        string? unit)
    {
        string value =
            (unit ?? "")
            .Trim()
            .ToLowerInvariant();

        return value == "weight" ||
               value == "kg" ||
               value == "kilogram" ||
               value == "kilograms" ||
               value == "gram" ||
               value == "grams" ||
               value == "g";
    }
}


// =============================================================
// CART DISPLAY MODEL
// =============================================================

public class ApiCartDisplayItem
{
    public string CartItemId { get; set; } =
        string.Empty;

    public string CustomerId { get; set; } =
        string.Empty;

    public string ProductId { get; set; } =
        string.Empty;

    public string ProductName { get; set; } =
        string.Empty;

    public string ProductImage { get; set; } =
        string.Empty;

    public double Quantity { get; set; }

    public string QuantityUnit { get; set; } =
        "Weight";

    public decimal Price { get; set; }

    // =====================================================
    // TOTAL PRICE
    // =====================================================

    public decimal TotalPrice
    {
        get
        {
            if (IsWeight())
            {
                return Price *
                       (decimal)Quantity /
                       1000m;
            }

            return Price *
                   (decimal)Quantity;
        }
    }

    // =====================================================
    // DISPLAY QUANTITY
    // =====================================================

    public string DisplayQuantity
    {
        get
        {
            if (IsWeight())
            {
                if (Quantity < 1000)
                    return $"{Quantity:0} g";

                decimal kg =
                    (decimal)Quantity / 1000m;

                return $"{kg:0.##} kg";
            }

            return Quantity == 1
                ? "1 piece"
                : $"{Quantity:0} pieces";
        }
    }

    // =====================================================
    // DISPLAY PRICE
    // =====================================================

    public string DisplayPrice
    {
        get
        {
            return IsWeight()
                ? $"₹{Price:0.00}/kg"
                : $"₹{Price:0.00}/piece";
        }
    }

    private bool IsWeight()
    {
        string unit =
            (QuantityUnit ?? "")
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
}