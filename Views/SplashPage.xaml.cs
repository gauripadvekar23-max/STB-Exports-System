using System.Threading.Tasks;

namespace STB_Exports.Views;

public partial class SplashPage : ContentPage
{
    private bool _hasNavigated = false;

    public SplashPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_hasNavigated)
            return;

        _hasNavigated = true;

        await Task.Delay(2500);

        await Shell.Current.GoToAsync("//CustomerLoginPage");
    }
}