using Microsoft.Maui.Storage;
using STB_Exports.Data;
using STB_Exports.Models;

namespace STB_Exports.Views;

public partial class PaymentPage : ContentPage
{
    private readonly DatabaseService _databaseService;

    private readonly HttpClient _httpClient;

    private readonly List<CartItem> _cartItems;

    private readonly ApiAddress _selectedAddress;

    private readonly decimal _totalAmount;

    private bool _isPlacingOrder;


    // =====================================================
    // CONSTRUCTOR
    // =====================================================

    public PaymentPage(
        List<CartItem> cartItems,
        ApiAddress selectedAddress,
        decimal totalAmount)
    {
        InitializeComponent();

        _databaseService = new DatabaseService();

        // Allow localhost HTTPS certificate for local API
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                (message, certificate, chain, errors) => true
        };

        _httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://localhost:7197")
        };

        _cartItems =
            cartItems ?? new List<CartItem>();

        _selectedAddress =
            selectedAddress;

        _totalAmount =
            totalAmount;


        // =================================================
        // DISPLAY TOTAL
        // =================================================

        PaymentTotalLabel.Text =
            $"₹{_totalAmount:0.00}";


        // =================================================
        // COD ONLY
        // =================================================

        CodRadioButton.IsChecked = true;

        UpdatePaymentButtonText();
    }


    // =====================================================
    // PAYMENT METHOD
    // =====================================================

    private void PaymentMethod_CheckedChanged(
        object sender,
        CheckedChangedEventArgs e)
    {
        if (!e.Value)
            return;

        UpdatePaymentButtonText();
    }


    // =====================================================
    // UPDATE BUTTON TEXT
    // =====================================================

    private void UpdatePaymentButtonText()
    {
        if (PayButton == null)
            return;

        if (CodRadioButton != null &&
            CodRadioButton.IsChecked)
        {
            PayButton.Text = "Place Order";
        }
        else
        {
            PayButton.Text = "Pay Now";
        }
    }


    // =====================================================
    // PAYMENT BUTTON
    // =====================================================

    private async void PayButton_Clicked(
        object sender,
        EventArgs e)
    {
        if (_isPlacingOrder)
            return;

        try
        {
            _isPlacingOrder = true;

            PayButton.IsEnabled = false;


            // =================================================
            // COD ONLY
            // =================================================

            if (!CodRadioButton.IsChecked)
            {
                await DisplayAlert(
                    "Payment Method Required",
                    "Please select Cash on Delivery.",
                    "OK");

                return;
            }


            const string paymentMethod = "COD";


            // =================================================
            // CONFIRM ORDER
            // =================================================

            bool confirm =
                await DisplayAlert(
                    "Confirm Order",
                    $"Place order with Cash on Delivery?\n\n" +
                    $"Amount: ₹{_totalAmount:0.00}",
                    "Place Order",
                    "Cancel");


            if (!confirm)
                return;


            // =================================================
            // PLACE ORDER
            // =================================================

            await PlaceOrderAsync(paymentMethod);
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Payment Error",
                "Something went wrong while processing your order.\n\n" +
                ex.Message,
                "OK");
        }
        finally
        {
            _isPlacingOrder = false;

            if (PayButton != null)
                PayButton.IsEnabled = true;

            UpdatePaymentButtonText();
        }
    }


    // =====================================================
    // PLACE ORDER
    // =====================================================

    private async Task PlaceOrderAsync(
        string paymentMethod)
    {
        try
        {
            // =================================================
            // LOCAL CUSTOMER ID
            // =================================================

            int customerId =
                Preferences.Get(
                    "CustomerId",
                    0);


            if (customerId <= 0)
            {
                await DisplayAlert(
                    "Customer Error",
                    "Customer information could not be found. Please login again.",
                    "OK");

                return;
            }


            // =================================================
            // API CUSTOMER ID
            // =================================================

            string apiCustomerId =
                Preferences.Get(
                    "ApiCustomerId",
                    string.Empty)
                .Trim();


            if (string.IsNullOrWhiteSpace(apiCustomerId))
            {
                await DisplayAlert(
                    "Login Required",
                    "Your API login session could not be found. Please login again.",
                    "OK");

                return;
            }


            // =================================================
            // CHECK CART
            // =================================================

            if (_cartItems == null ||
                _cartItems.Count == 0)
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

            if (_selectedAddress == null ||
                string.IsNullOrWhiteSpace(
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
                    "Address Required",
                    "Please select a complete delivery address before placing the order.",
                    "OK");

                return;
            }


            // =================================================
            // PAYMENT STATUS
            // =================================================

            string paymentStatus =
                paymentMethod == "COD"
                    ? "Pending"
                    : "Paid";


            // =================================================
            // RECALCULATE TOTAL
            // =================================================

            decimal calculatedTotal = 0m;

            foreach (CartItem item in _cartItems)
            {
                calculatedTotal += item.TotalPrice;
            }


            // =================================================
            // CREATE LOCAL ORDER
            // =================================================

            Order order =
                new Order
                {
                    CustomerId =
                        customerId,

                    OrderStatus =
                        "Pending",

                    PaymentMethod =
                        paymentMethod,

                    PaymentStatus =
                        paymentStatus,

                    FullName =
                        _selectedAddress.FullName,

                    PhoneNumber =
                        _selectedAddress.Phone,

                    Address =
                        _selectedAddress.AddressLine,

                    City =
                        _selectedAddress.City,

                    State =
                        _selectedAddress.State,

                    Pincode =
                        _selectedAddress.Pincode,

                    TotalAmount =
                        calculatedTotal
                };


            // =================================================
            // SAVE LOCAL ORDER
            // =================================================

            int orderId =
                await _databaseService.CreateOrderAsync(
                    order,
                    _cartItems);


            // =================================================
            // CHECK RESULT
            // =================================================

            if (orderId <= 0)
            {
                await DisplayAlert(
                    "Order Failed",
                    "We could not place your order. Please try again.",
                    "OK");

                return;
            }


            // =================================================
            // CLEAR API CART
            // =================================================

            bool apiCartCleared =
                await ClearApiCartAsync(
                    apiCustomerId);


            // =================================================
            // CLEAR SELECTED ADDRESS PREFERENCES
            // =================================================

            Preferences.Remove(
                "SelectedAddressId");

            Preferences.Remove(
                "SelectedAddressFullName");

            Preferences.Remove(
                "SelectedAddressPhone");

            Preferences.Remove(
                "SelectedAddressLine");

            Preferences.Remove(
                "SelectedAddressCity");

            Preferences.Remove(
                "SelectedAddressState");

            Preferences.Remove(
                "SelectedAddressPincode");


            // =================================================
            // SUCCESS MESSAGE
            // =================================================

            string message =
                $"Your order has been placed successfully!\n\n" +
                $"Order Number: {order.OrderNumber}\n" +
                $"Payment Method: {paymentMethod}\n" +
                $"Payment Status: {paymentStatus}\n" +
                $"Amount: ₹{order.TotalAmount:0.00}";


            // If API cart could not be cleared,
            // tell the user that the order itself is still successful.
            if (!apiCartCleared)
            {
                message +=
                    "\n\nNote: Your order was successfully placed, " +
                    "but the online cart could not be synchronized.";
            }


            await DisplayAlert(
                "Order Placed Successfully",
                message,
                "View Orders");


            // =================================================
            // NAVIGATE TO ORDERS
            // =================================================

            await NavigateToOrdersAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Order Error",
                "Unable to place your order.\n\n" +
                ex.Message,
                "OK");
        }
    }


    // =====================================================
    // CLEAR API CART
    // =====================================================

    private async Task<bool> ClearApiCartAsync(
        string apiCustomerId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(apiCustomerId))
                return false;

            using HttpResponseMessage response =
                await _httpClient.DeleteAsync(
                    $"/api/CartItems/customer/{apiCustomerId}");

            return response.IsSuccessStatusCode;
        }
        catch
        {
            // The local order has already been successfully
            // created, so don't fail the order because of
            // an API cart synchronization problem.
            return false;
        }
    }


    // =====================================================
    // NAVIGATE TO ORDERS
    // =====================================================

    private async Task NavigateToOrdersAsync()
    {
        try
        {
            // PaymentPage is on top of CheckoutPage.
            if (Navigation.NavigationStack.Count > 0)
            {
                await Navigation.PopAsync();
            }

            await Task.Delay(100);

            await Navigation.PushAsync(
                new OrdersPage());
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Navigation Error",
                "Order was placed successfully, but Orders page could not be opened.\n\n" +
                ex.Message,
                "OK");
        }
    }
}