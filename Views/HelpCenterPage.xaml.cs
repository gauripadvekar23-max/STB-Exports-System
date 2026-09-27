using Microsoft.Maui.ApplicationModel;

namespace STB_Exports.Views;

public partial class HelpCenterPage : ContentPage
{
    // =====================================================
    // SUPPORT EMAIL
    // =====================================================

    private const string AdminEmail =
        "samarthaex1015@gmail.com";


    // =====================================================
    // CONSTRUCTOR
    // =====================================================

    public HelpCenterPage()
    {
        InitializeComponent();
    }


    // =====================================================
    // BACK BUTTON
    // =====================================================

    private async void BackButton_Clicked(
        object sender,
        EventArgs e)
    {
        if (Navigation.NavigationStack.Count > 1)
        {
            await Navigation.PopAsync();
        }
        else
        {
            await Shell.Current.GoToAsync("..");
        }
    }


    // =====================================================
    // CONTACT SUPPORT
    // =====================================================

    private async void ContactSupport_Clicked(
        object sender,
        EventArgs e)
    {
        await ShowSupportEmailAsync("Contact Support");
    }


    // =====================================================
    // ORDERS
    // =====================================================

    private async void Orders_Clicked(
        object sender,
        TappedEventArgs e)
    {
        await ShowSupportEmailAsync("Orders Support");
    }


    // =====================================================
    // PAYMENT
    // =====================================================

    private async void Payment_Clicked(
        object sender,
        TappedEventArgs e)
    {
        await ShowSupportEmailAsync("Payment Support");
    }


    // =====================================================
    // RETURNS & REFUNDS
    // =====================================================

    private async void ReturnsRefunds_Clicked(
        object sender,
        TappedEventArgs e)
    {
        await ShowSupportEmailAsync("Returns & Refunds");
    }


    // =====================================================
    // SHOW SUPPORT EMAIL
    // =====================================================

    private async Task ShowSupportEmailAsync(string title)
    {
        bool openEmail = await DisplayAlert(
            title,
            $"For assistance, please contact our support team at:\n\n{AdminEmail}",
            "Send Email",
            "Cancel");

        if (!openEmail)
            return;


        // =================================================
        // OPEN EMAIL APP
        // =================================================

        try
        {
            Uri emailUri = new Uri(
                $"mailto:{AdminEmail}");

            bool opened =
                await Launcher.Default.OpenAsync(emailUri);

            if (!opened)
            {
                await DisplayAlert(
                    "Email",
                    $"Please email us at:\n\n{AdminEmail}",
                    "OK");
            }
        }
        catch
        {
            await DisplayAlert(
                "Email",
                $"Please email us at:\n\n{AdminEmail}",
                "OK");
        }
    }
}