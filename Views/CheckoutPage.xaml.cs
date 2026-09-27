using Microsoft.Maui.Storage;
using STB_Exports.Data;
using STB_Exports.Models;
using System.Net.Http.Json;

namespace STB_Exports.Views;

public partial class CheckoutPage : ContentPage
{
    private readonly DatabaseService _databaseService;
    private readonly HttpClient _httpClient;

    private readonly List<CartItem> _cartItems = new();

    private ApiAddress? _selectedAddress;


    // =====================================================
    // CONSTRUCTOR
    // =====================================================

    public CheckoutPage()
    {
        InitializeComponent();

        _databaseService =
            new DatabaseService();

        var handler =
            new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    (message, certificate, chain, errors) => true
            };

        _httpClient =
            new HttpClient(handler)
            {
                BaseAddress =
                    new Uri("https://localhost:7197")
            };
    }


    // =====================================================
    // PAGE APPEARING
    // =====================================================

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await LoadCheckoutAsync();
    }


    // =====================================================
    // LOAD CHECKOUT
    // =====================================================

    private async Task LoadCheckoutAsync()
    {
        try
        {
            string apiCustomerId =
                Preferences.Get(
                    "ApiCustomerId",
                    string.Empty)
                .Trim();


            // =================================================
            // CHECK LOGIN
            // =================================================

            if (string.IsNullOrWhiteSpace(apiCustomerId))
            {
                await DisplayAlert(
                    "Login Required",
                    "Please login before placing an order.",
                    "OK");

                await Navigation.PopAsync();

                return;
            }


            // =================================================
            // LOAD CART FROM API
            // =================================================

            using HttpResponseMessage response =
                await _httpClient.GetAsync(
                    $"/api/CartItems/customer/{apiCustomerId}");


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


            List<ApiCartItem>? apiCartItems =
                await response.Content
                    .ReadFromJsonAsync<List<ApiCartItem>>();


            if (apiCartItems == null ||
                apiCartItems.Count == 0)
            {
                await DisplayAlert(
                    "Cart Empty",
                    "Your cart is empty.",
                    "OK");

                await Navigation.PopAsync();

                return;
            }


            // =================================================
            // CONVERT API CART TO LOCAL MODEL
            // =================================================

            _cartItems.Clear();

            foreach (ApiCartItem apiItem in apiCartItems)
            {
                int localProductId = 0;

                int.TryParse(
                    apiItem.ProductId,
                    out localProductId);


                CartItem cartItem =
                    new CartItem
                    {
                        CartItemId = 0,

                        ProductId =
                            localProductId,

                        ProductName =
                            apiItem.Product?.Name
                            ?? "Product",

                        ProductImage =
                            apiItem.Product?.Image
                            ?? string.Empty,

                        Price =
                            (decimal)apiItem.Price,

                        Quantity =
                            (int)apiItem.Quantity,

                        QuantityUnit =
                            string.IsNullOrWhiteSpace(
                                apiItem.QuantityUnit)
                                ? "Weight"
                                : apiItem.QuantityUnit
                    };


                _cartItems.Add(cartItem);
            }


            // =================================================
            // DISPLAY ITEMS
            // =================================================

            CheckoutItemsCollection.ItemsSource = null;

            CheckoutItemsCollection.ItemsSource =
                _cartItems;


            // =================================================
            // LOAD ADDRESS
            // =================================================

            await LoadSelectedAddressAsync();


            // =================================================
            // UPDATE TOTAL
            // =================================================

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
                "Checkout Error",
                ex.Message,
                "OK");
        }
    }


    // =====================================================
    // LOAD SELECTED ADDRESS
    // =====================================================

    private async Task LoadSelectedAddressAsync()
    {
        try
        {
            string customerId =
                Preferences.Get(
                    "ApiCustomerId",
                    string.Empty)
                .Trim();


            if (string.IsNullOrWhiteSpace(customerId))
            {
                ClearAddressDisplay();
                return;
            }


            // =================================================
            // GET SELECTED ADDRESS ID
            // =================================================

            string selectedAddressId =
                Preferences.Get(
                    "SelectedAddressId",
                    string.Empty)
                .Trim();


            // =================================================
            // LOAD SELECTED ADDRESS
            // =================================================

            if (!string.IsNullOrWhiteSpace(
                selectedAddressId))
            {
                using HttpResponseMessage response =
                    await _httpClient.GetAsync(
                        $"/api/Addresses/{selectedAddressId}");


                if (response.IsSuccessStatusCode)
                {
                    ApiAddress? address =
                        await response.Content
                            .ReadFromJsonAsync<ApiAddress>();


                    if (address != null &&
                        address.CustomerId == customerId)
                    {
                        _selectedAddress = address;

                        DisplayAddress(address);

                        SaveSelectedAddressPreferences(
                            address);

                        return;
                    }
                }
            }


            // =================================================
            // LOAD CUSTOMER ADDRESSES
            // =================================================

            using HttpResponseMessage addressesResponse =
                await _httpClient.GetAsync(
                    $"/api/Addresses/customer/{customerId}");


            if (!addressesResponse.IsSuccessStatusCode)
            {
                ClearAddressDisplay();
                return;
            }


            List<ApiAddress>? addresses =
                await addressesResponse.Content
                    .ReadFromJsonAsync<List<ApiAddress>>();


            ApiAddress? firstAddress =
                addresses?.FirstOrDefault();


            if (firstAddress != null)
            {
                _selectedAddress =
                    firstAddress;


                SaveSelectedAddressPreferences(
                    firstAddress);


                DisplayAddress(
                    firstAddress);
            }
            else
            {
                _selectedAddress = null;

                ClearAddressDisplay();
            }
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
                "Address Error",
                "Unable to load delivery address.\n\n" +
                ex.Message,
                "OK");
        }
    }


    // =====================================================
    // DISPLAY ADDRESS
    // =====================================================

    private void DisplayAddress(
        ApiAddress address)
    {
        CustomerNameLabel.Text =
            address.FullName;

        CustomerPhoneLabel.Text =
            address.Phone;

        CustomerAddressLabel.Text =
            address.AddressLine;

        CustomerCityStateLabel.Text =
            $"{address.City}, " +
            $"{address.State} - " +
            $"{address.Pincode}";
    }


    // =====================================================
    // CLEAR ADDRESS
    // =====================================================

    private void ClearAddressDisplay()
    {
        CustomerNameLabel.Text =
            "Address not added";

        CustomerPhoneLabel.Text =
            string.Empty;

        CustomerAddressLabel.Text =
            "Please add your delivery address.";

        CustomerCityStateLabel.Text =
            string.Empty;

        _selectedAddress = null;
    }


    // =====================================================
    // SAVE SELECTED ADDRESS
    // =====================================================

    private void SaveSelectedAddressPreferences(
        ApiAddress address)
    {
        Preferences.Set(
            "SelectedAddressId",
            address.AddressId);

        Preferences.Set(
            "SelectedAddressFullName",
            address.FullName);

        Preferences.Set(
            "SelectedAddressPhone",
            address.Phone);

        Preferences.Set(
            "SelectedAddressLine",
            address.AddressLine);

        Preferences.Set(
            "SelectedAddressCity",
            address.City);

        Preferences.Set(
            "SelectedAddressState",
            address.State);

        Preferences.Set(
            "SelectedAddressPincode",
            address.Pincode);
    }


    // =====================================================
    // UPDATE TOTAL
    // =====================================================

    private void UpdateTotal()
    {
        decimal itemsTotal = 0m;


        foreach (CartItem item in _cartItems)
        {
            itemsTotal += item.TotalPrice;
        }


        decimal deliveryCharges = 0m;

        decimal finalTotal =
            itemsTotal +
            deliveryCharges;


        ItemsTotalLabel.Text =
            $"₹{itemsTotal:0.00}";


        DeliveryChargesLabel.Text =
            deliveryCharges == 0
                ? "FREE"
                : $"₹{deliveryCharges:0.00}";


        TotalAmountLabel.Text =
            $"₹{finalTotal:0.00}";
    }


    // =====================================================
    // CHANGE ADDRESS
    // =====================================================

    private async void ChangeAddressButton_Clicked(
        object sender,
        EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(
                new AccountInformationPage(true));
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Address Error",
                "Unable to open saved addresses.\n\n" +
                ex.Message,
                "OK");
        }
    }


    // =====================================================
    // CONTINUE TO PAYMENT
    // =====================================================

    private async void ContinueToPaymentButton_Clicked(
        object sender,
        EventArgs e)
    {
        try
        {
            // =================================================
            // CHECK CART
            // =================================================

            if (_cartItems.Count == 0)
            {
                await DisplayAlert(
                    "Cart Empty",
                    "Your cart is empty.",
                    "OK");

                return;
            }


            // =================================================
            // CHECK ADDRESS
            // =================================================

            if (_selectedAddress == null)
            {
                await DisplayAlert(
                    "Delivery Address Required",
                    "Please select or add a delivery address before continuing.",
                    "OK");

                return;
            }


            // =================================================
            // VALIDATE ADDRESS
            // =================================================

            if (string.IsNullOrWhiteSpace(
                    _selectedAddress.FullName) ||

                string.IsNullOrWhiteSpace(
                    _selectedAddress.Phone) ||

                string.IsNullOrWhiteSpace(
                    _selectedAddress.AddressLine) ||

                string.IsNullOrWhiteSpace(
                    _selectedAddress.City) ||

                string.IsNullOrWhiteSpace(
                    _selectedAddress.State) ||

                string.IsNullOrWhiteSpace(
                    _selectedAddress.Pincode))
            {
                await DisplayAlert(
                    "Incomplete Address",
                    "Please select a complete delivery address.",
                    "OK");

                return;
            }


            // =================================================
            // CALCULATE TOTAL
            // =================================================

            decimal total = 0m;

            foreach (CartItem item in _cartItems)
            {
                total += item.TotalPrice;
            }


            // =================================================
            // GO TO PAYMENT
            // =================================================

            await Navigation.PushAsync(
                new PaymentPage(
                    _cartItems,
                    _selectedAddress,
                    total));
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Payment Error",
                "Unable to continue to payment.\n\n" +
                ex.Message,
                "OK");
        }
    }
}