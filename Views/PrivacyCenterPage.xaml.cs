namespace STB_Exports.Views;

public partial class PrivacyCenterPage : ContentPage
{
    public PrivacyCenterPage()
    {
        InitializeComponent();
    }

    

    // =====================================================
    // PRIVACY POLICY
    // =====================================================

    private async void PrivacyPolicy_Clicked(
        object sender,
        EventArgs e)
    {
        await DisplayAlert(
            "Privacy Policy",
            "STB Exports respects your privacy.\n\n" +
            "We collect only the information required to provide our services, " +
            "process orders, manage deliveries and provide customer support.\n\n" +
            "Your personal information will not be unnecessarily shared with third parties.",
            "OK");
    }

    // =====================================================
    // REQUEST MY DATA
    // =====================================================

    private async void RequestData_Clicked(
        object sender,
        EventArgs e)
    {
        await DisplayAlert(
            "Request My Data",
            "You can request the personal information associated with your STB Exports account.\n\n" +
            "Please contact our support team at:\n\n" +
            "samarthaex1015@gmail.com",
            "OK");
    }

    // =====================================================
    // CONSENT
    // =====================================================

    private async void Consent_Clicked(
        object sender,
        EventArgs e)
    {
        await DisplayAlert(
            "Consent Management",
            "You can manage your notification and communication preferences from the Settings page.",
            "OK");
    }

    // =====================================================
    // GRIEVANCE
    // =====================================================

    private async void Grievance_Clicked(
        object sender,
        EventArgs e)
    {
        await DisplayAlert(
            "Grievance Redressal",
            "For complaints or grievances, please contact:\n\n" +
            "samarthaex1015@gmail.com",
            "OK");
    }

    // =====================================================
    // DELETE ACCOUNT
    // =====================================================

    private async void DeleteAccount_Clicked(
        object sender,
        EventArgs e)
    {
        bool delete = await DisplayAlert(
            "Delete Account",
            "Are you sure you want to permanently delete your account?\n\n" +
            "This action cannot be undone.",
            "Delete",
            "Cancel");

        if (!delete)
            return;

        Preferences.Remove("CustomerName");
        Preferences.Remove("CustomerEmail");
        Preferences.Remove("CustomerPhone");
        Preferences.Remove("AccountDeactivated");

        await DisplayAlert(
            "Account Deleted",
            "Your account has been deleted successfully.",
            "OK");

        await Navigation.PopToRootAsync();
    }

    // =====================================================
    // DEACTIVATE ACCOUNT
    // =====================================================

    private async void DeactivateAccount_Clicked(
        object sender,
        EventArgs e)
    {
        bool deactivate = await DisplayAlert(
            "Deactivate Account",
            "Do you want to deactivate your account?\n\n" +
            "You can contact support if you want to reactivate it later.",
            "Deactivate",
            "Cancel");

        if (!deactivate)
            return;

        Preferences.Set(
            "AccountDeactivated",
            true);

        await DisplayAlert(
            "Account Deactivated",
            "Your account has been deactivated.",
            "OK");

        await Navigation.PopToRootAsync();
    }
}