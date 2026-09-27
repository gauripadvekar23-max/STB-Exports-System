namespace STB_Exports.Views;

public partial class CustomerBottomNavigation : ContentView
{
    public CustomerBottomNavigation()
    {
        InitializeComponent();
    }

    private async void Home_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//HomePage");
    }

    private async void Wishlist_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new WishlistPage());
    }

    private async void Categories_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(
            new CategoryProductsPage("Fruits"));
    }

    private async void Cart_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CartPage());
    }

    private async void Profile_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CustomerProfilePage());
    }
}