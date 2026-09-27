namespace STB_Exports.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
    }

    // =====================================================
    // BACK
    // =====================================================

    private async void BackButton_Clicked(
        object sender,
        EventArgs e)
    {
        if (Navigation.NavigationStack.Count > 1)
        {
            await Navigation.PopAsync();
        }
    }


    // =====================================================
    // ACCOUNT INFORMATION
    // =====================================================

    private async void AccountInformation_Clicked(
        object sender,
        EventArgs e)
    {
        await Navigation.PushAsync(
            new AccountInformationPage());
    }


    // =====================================================
    // PRIVACY POLICY
    // =====================================================

    private async void PrivacyPolicy_Clicked(
        object sender,
        EventArgs e)
    {
        await Navigation.PushAsync(
            new PrivacyCenterPage());
    }


    // =====================================================
    // TERMS & CONDITIONS
    // =====================================================

    private async void Terms_Clicked(
        object sender,
        EventArgs e)
    {
        await DisplayAlert(
            "Terms & Conditions",
            "By using STB Exports, you agree to follow our terms of service, ordering rules, payment policies and delivery conditions.",
            "OK");
    }


    // =====================================================
    // ABOUT STB EXPORTS
    // =====================================================

    private async void About_Clicked(
        object sender,
        EventArgs e)
    {
        await DisplayAlert(
            "About STB Exports",
            "Samartha Trade Bridge\n\n" +
            "Import & Export Management System\n\n" +
            "We provide quality products for customers and manage export-related services through STB Exports.",
            "OK");
    }


    // =====================================================
    // LOGOUT
    // =====================================================

    private async void LogoutButton_Clicked(
        object sender,
        EventArgs e)
    {
        bool logout = await DisplayAlert(
            "Logout",
            "Are you sure you want to logout?",
            "Yes",
            "Cancel");

        if (!logout)
            return;

        await Navigation.PopToRootAsync();
    }
}