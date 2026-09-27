using Microsoft.Maui.Storage;
using STB_Exports.Data;
using STB_Exports.Models;

namespace STB_Exports.Views;

public partial class OrdersPage : ContentPage
{
    private readonly DatabaseService _databaseService;


    // =====================================================
    // CONSTRUCTOR
    // =====================================================

    public OrdersPage()
    {
        InitializeComponent();

        _databaseService = new DatabaseService();
    }


    // =====================================================
    // PAGE APPEARING
    // =====================================================

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await LoadOrdersAsync();
    }


    // =====================================================
    // LOAD ORDERS
    // =====================================================

    private async Task LoadOrdersAsync()
    {
        try
        {
            int customerId = Preferences.Get(
                "CustomerId",
                0);


            // =================================================
            // CUSTOMER NOT LOGGED IN
            // =================================================

            if (customerId <= 0)
            {
                OrdersCollectionView.ItemsSource = null;
                OrdersCollectionView.IsVisible = false;

                EmptyOrdersLayout.IsVisible = true;

                return;
            }


            // =================================================
            // GET ORDERS
            // =================================================

            List<Order> orders =
                await _databaseService.GetCustomerOrdersAsync(
                    customerId);


            // =================================================
            // NO ORDERS
            // =================================================

            if (orders == null || orders.Count == 0)
            {
                OrdersCollectionView.ItemsSource = null;
                OrdersCollectionView.IsVisible = false;

                EmptyOrdersLayout.IsVisible = true;

                return;
            }


            // =================================================
            // SHOW ORDERS
            // =================================================

            OrdersCollectionView.ItemsSource = orders;

            OrdersCollectionView.IsVisible = true;
            EmptyOrdersLayout.IsVisible = false;
        }
        catch (Exception ex)
        {
            OrdersCollectionView.ItemsSource = null;
            OrdersCollectionView.IsVisible = false;

            EmptyOrdersLayout.IsVisible = true;

            await DisplayAlert(
                "Orders Error",
                ex.Message,
                "OK");
        }
    }


    // =====================================================
    // VIEW ORDER DETAILS
    // =====================================================

    private async void ViewOrderDetails_Clicked(
        object sender,
        EventArgs e)
    {
        try
        {
            if (sender is not Button button)
                return;


            if (button.CommandParameter is not Order order)
                return;


            await DisplayAlert(
                "Order Details",

                $"Order Number: {order.OrderNumber}\n\n" +

                $"Order Date: " +
                $"{order.OrderDate:dd MMM yyyy}\n" +

                $"Expected Delivery: " +
                $"{order.ExpectedDeliveryDate:dd MMM yyyy}\n\n" +

                $"Status: " +
                $"{order.OrderStatus}\n" +

                $"Payment: " +
                $"{order.PaymentMethod}\n" +

                $"Payment Status: " +
                $"{order.PaymentStatus}\n\n" +

                $"Total: " +
                $"₹{order.TotalAmount:0.00}",

                "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Error",
                ex.Message,
                "OK");
        }
    }
}