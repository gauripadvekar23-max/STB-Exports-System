using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using STB_Exports.Data;
using STB_Exports.Models;
using System.Collections;

namespace STB_Exports.Views;

public partial class HomePage : ContentPage
{
    private IDispatcherTimer? _bannerTimer;

    private readonly DatabaseService _databaseService;


    // =====================================================
    // CONSTRUCTOR
    // =====================================================

    public HomePage()
    {
        InitializeComponent();

        // =====================================================
        // DATABASE
        // =====================================================

        _databaseService = new DatabaseService();


        // =====================================================
        // BANNERS
        // =====================================================

        BannerCarousel.ItemsSource = new List<string>
        {
            "banner1.png",
            "banner2.png",
            "banner3.png"
        };


        // =====================================================
        // FEATURED PRODUCTS
        // =====================================================

        var featuredProducts =
            ProductCatalogue.Products
                .Where(p =>
                    p.Name == "Premium Export Chillies" ||
                    p.Name == "Fresh Export Tomatoes" ||
                    p.Name == "Premium Cashews" ||
                    p.Name == "Export Quality Spices")
                .Select(p => new FeaturedProduct
                {
                    Product = p
                })
                .ToList();

        FeaturedProductsCollection.ItemsSource =
            featuredProducts;


        // =====================================================
        // AUTO BANNER
        // =====================================================

        _bannerTimer =
            Dispatcher.CreateTimer();

        _bannerTimer.Interval =
            TimeSpan.FromSeconds(3);

        _bannerTimer.Tick +=
            BannerTimer_Tick;

        _bannerTimer.Start();
    }


    // =====================================================
    // FEATURED PRODUCT MODEL
    // =====================================================

    public class FeaturedProduct
    {
        public Product Product { get; set; } =
            new Product();

        public string Name =>
            Product.Name;

        public string Image =>
            Product.Image;

        public string WishlistIcon { get; set; } =
            "♡";

        public Color WishlistColor { get; set; } =
            Colors.Gray;
    }


    // =====================================================
    // PAGE APPEARING
    // =====================================================

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _bannerTimer?.Start();

        await LoadWishlistStatusAsync();
    }


    // =====================================================
    // BANNER TIMER
    // =====================================================

    private void BannerTimer_Tick(
        object? sender,
        EventArgs e)
    {
        if (BannerCarousel.ItemsSource is IList items &&
            items.Count > 0)
        {
            int nextIndex =
                BannerCarousel.Position + 1;

            if (nextIndex >= items.Count)
                nextIndex = 0;

            BannerCarousel.Position =
                nextIndex;
        }
    }


    // =====================================================
    // SEARCH - ENTER
    // =====================================================

    private async void SearchEntry_Completed(
        object sender,
        EventArgs e)
    {
        await PerformSearch();
    }


    // =====================================================
    // SEARCH - BUTTON
    // =====================================================

    private async void SearchButton_Clicked(
        object sender,
        EventArgs e)
    {
        await PerformSearch();
    }


    // =====================================================
    // SEARCH
    // =====================================================

    private async Task PerformSearch()
    {
        string searchText =
            SearchEntry.Text?.Trim() ??
            string.Empty;

        if (string.IsNullOrWhiteSpace(searchText))
        {
            await DisplayAlert(
                "Search",
                "Please enter a product name.",
                "OK");

            return;
        }

        await Navigation.PushAsync(
            new SearchResultsPage(searchText));
    }


    // =====================================================
    // NOTIFICATION
    // =====================================================

    private async void NotificationButton_Clicked(
        object sender,
        EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(
                new NotificationsPage());
        }
        catch
        {
            await DisplayAlert(
                "Notifications",
                "Notifications page is not available.",
                "OK");
        }
    }


    // =====================================================
    // PROFILE
    // =====================================================

    private async void ProfileButton_Clicked(
        object sender,
        EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(
                new CustomerProfilePage());
        }
        catch
        {
            await DisplayAlert(
                "Account",
                "Customer profile page is not available yet.",
                "OK");
        }
    }


    // =====================================================
    // CART
    // =====================================================

    private async void CartTab_Tapped(
        object sender,
        TappedEventArgs e)
    {
        try
        {
            await Navigation.PushAsync(
                new CartPage());
        }
        catch
        {
            await DisplayAlert(
                "Cart",
                "Cart page is not available yet.",
                "OK");
        }
    }


    // =====================================================
    // EXPLORE
    // =====================================================

    private async void ExploreNow_Clicked(
        object sender,
        EventArgs e)
    {
        await MainScroll.ScrollToAsync(
            ExploreSection,
            ScrollToPosition.Start,
            true);
    }


    // =====================================================
    // PRODUCT DETAILS
    // =====================================================

    private async void ProductDetails_Clicked(
        object sender,
        EventArgs e)
    {
        if (sender is Button button &&
            button.BindingContext is FeaturedProduct featuredProduct)
        {
            Product selectedProduct =
                featuredProduct.Product;

            await Navigation.PushAsync(
                new ProductsDetailsPage(
                    selectedProduct));
        }
    }


    // =====================================================
    // CATEGORIES
    // =====================================================

    private async void Fruits_Tapped(
        object sender,
        TappedEventArgs e)
    {
        await Navigation.PushAsync(
            new CategoryProductsPage("Fruits"));
    }


    private async void Vegetables_Tapped(
        object sender,
        TappedEventArgs e)
    {
        await Navigation.PushAsync(
            new CategoryProductsPage("Vegetables"));
    }


    private async void DryFruits_Tapped(
        object sender,
        TappedEventArgs e)
    {
        await Navigation.PushAsync(
            new CategoryProductsPage("Dry Fruits"));
    }


    private async void Clay_Tapped(
        object sender,
        TappedEventArgs e)
    {
        await Navigation.PushAsync(
            new CategoryProductsPage("Clay"));
    }


    private async void Spices_Tapped(
        object sender,
        TappedEventArgs e)
    {
        await Navigation.PushAsync(
            new CategoryProductsPage("Spices"));
    }


    private async void Coconut_Tapped(
        object sender,
        TappedEventArgs e)
    {
        await Navigation.PushAsync(
            new CategoryProductsPage("Coconut"));
    }


    // =====================================================
    // CATEGORIES VIEW ALL
    // =====================================================

    private async void CategoriesViewAll_Clicked(
        object sender,
        EventArgs e)
    {
        await MainScroll.ScrollToAsync(
            ExploreSection,
            ScrollToPosition.Start,
            true);
    }


    // =====================================================
    // FEATURED VIEW ALL
    // =====================================================

    private async void FeaturedViewAll_Clicked(
        object sender,
        EventArgs e)
    {
        await DisplayAlert(
            "Featured Products",
            "All featured products will be displayed here.",
            "OK");
    }


    // =====================================================
    // HOME TAB
    // =====================================================

    private async void HomeTab_Tapped(
        object sender,
        TappedEventArgs e)
    {
        await MainScroll.ScrollToAsync(
            0,
            0,
            true);
    }


    // =====================================================
    // CATEGORIES TAB
    // =====================================================

    private async void CategoriesTab_Tapped(
        object sender,
        TappedEventArgs e)
    {
        await MainScroll.ScrollToAsync(
            ExploreSection,
            ScrollToPosition.Start,
            true);
    }


    // =====================================================
    // PROFILE TAB
    // =====================================================

    private async void AccountTab_Tapped(
        object sender,
        TappedEventArgs e)
    {
        try
        {
            await Navigation.PushAsync(
                new CustomerProfilePage());
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Account",
                $"Unable to open profile.\n\n{ex.Message}",
                "OK");
        }
    }


    // =====================================================
    // WISHLIST TAB
    // =====================================================

    private async void WishlistTab_Tapped(
        object sender,
        TappedEventArgs e)
    {
        try
        {
            await Navigation.PushAsync(
                new WishlistPage());
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Wishlist",
                $"Unable to open wishlist.\n\n{ex.Message}",
                "OK");
        }
    }


    // =====================================================
    // LOAD WISHLIST STATUS
    // =====================================================

    private async Task LoadWishlistStatusAsync()
    {
        int customerId =
            Preferences.Get(
                "CustomerId",
                0);

        if (customerId <= 0)
            return;

        if (FeaturedProductsCollection.ItemsSource
            is not IEnumerable<FeaturedProduct> products)
        {
            return;
        }

        foreach (FeaturedProduct featuredProduct in products)
        {
            bool exists =
                await _databaseService
                    .IsProductInWishlistAsync(
                        customerId,
                        featuredProduct.Product.ProductId);

            featuredProduct.WishlistIcon =
                exists
                    ? "♥"
                    : "♡";

            featuredProduct.WishlistColor =
                exists
                    ? Colors.Goldenrod
                    : Colors.Gray;
        }

        FeaturedProductsCollection.ItemsSource =
            null;

        FeaturedProductsCollection.ItemsSource =
            products.ToList();
    }


    // =====================================================
    // PAGE DISAPPEARING
    // =====================================================

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        _bannerTimer?.Stop();
    }
}