using STB_Exports.Views;

namespace STB_Exports;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // =====================================================
        // CUSTOMER PORTAL - ROOT ROUTES
        // =====================================================

        Routing.RegisterRoute(
            nameof(CustomerLoginPage),
            typeof(CustomerLoginPage));

        Routing.RegisterRoute(
            nameof(CustomerRegistrationPage),
            typeof(CustomerRegistrationPage));

        // =====================================================
        // INTERNAL PAGES
        // =====================================================
        // Do NOT register these pages as Shell routes.
        // They are opened using Navigation.PushAsync().
        //
        // Examples:
        // Navigation.PushAsync(new CartPage());
        // Navigation.PushAsync(new CheckoutPage());
        // Navigation.PushAsync(new AccountInformationPage(true));
        // =====================================================
    }
}