using Microsoft.Maui.Storage;
using STB_Exports.Data;
using STB_Exports.Models;

namespace STB_Exports.Views;

public partial class NotificationsPage : ContentPage
{
    private readonly DatabaseService _databaseService;

    private int _customerId;

    public NotificationsPage()
    {
        InitializeComponent();

        _databaseService =
            new DatabaseService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await LoadNotificationsAsync();
    }


    private async Task LoadNotificationsAsync()
    {
        try
        {
            _customerId =
                Preferences.Get(
                    "CustomerId",
                    0);

            if (_customerId <= 0)
            {
                NotificationsCollectionView.ItemsSource =
                    null;

                return;
            }


            // =============================================
            // LOAD NOTIFICATIONS
            // =============================================

            List<OrderNotification> notifications =
                await _databaseService
                    .GetCustomerNotificationsAsync(
                        _customerId);

            NotificationsCollectionView.ItemsSource =
                notifications;


            // =============================================
            // MARK ALL AS SEEN
            // =============================================

            foreach (
                OrderNotification notification
                in notifications)
            {
                if (!notification.IsRead)
                {
                    await _databaseService
                        .MarkNotificationAsReadAsync(
                            notification.NotificationId,
                            _customerId);
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Notifications Error",
                ex.ToString(),
                "OK");
        }
    }
}