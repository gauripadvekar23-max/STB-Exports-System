using System;
using System.Linq;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using STB_Exports.Data;

namespace STB_Exports.Views;

public partial class CustomerHeaderFooter : ContentView
{
    private readonly DatabaseService _databaseService;

    public CustomerHeaderFooter()
    {
        InitializeComponent();

        _databaseService = new DatabaseService();

        Loaded += CustomerHeaderFooter_Loaded;
    }


    // =====================================================
    // LOAD BADGES
    // =====================================================

    private async void CustomerHeaderFooter_Loaded(
        object? sender,
        EventArgs e)
    {
        await RefreshBadgesAsync();
    }


    // =====================================================
    // REFRESH NOTIFICATION + CART BADGES
    // =====================================================

    public async Task RefreshBadgesAsync()
    {
        try
        {
            int customerId =
                Preferences.Get(
                    "CustomerId",
                    0);

            if (customerId <= 0)
            {
                NotificationBadge.IsVisible = false;
                CartBadge.IsVisible = false;
                return;
            }


            // =============================================
            // NOTIFICATIONS
            // =============================================

            int unreadNotifications =
                await _databaseService
                    .GetUnreadNotificationCountAsync(
                        customerId);

            if (unreadNotifications > 0)
            {
                NotificationBadgeLabel.Text =
                    unreadNotifications > 99
                        ? "99+"
                        : unreadNotifications.ToString();

                NotificationBadge.IsVisible = true;
            }
            else
            {
                NotificationBadge.IsVisible = false;
            }


            // =============================================
            // CART
            // =============================================

            int cartCount =
                await _databaseService
                    .GetCartItemCountAsync(
                        customerId);

            if (cartCount > 0)
            {
                CartBadgeLabel.Text =
                    cartCount > 99
                        ? "99+"
                        : cartCount.ToString();

                CartBadge.IsVisible = true;
            }
            else
            {
                CartBadge.IsVisible = false;
            }
        }
        catch
        {
            NotificationBadge.IsVisible = false;
            CartBadge.IsVisible = false;
        }
    }


    // =====================================================
    // WISHLIST
    // =====================================================

    private async void WishlistHeader_Clicked(
        object sender,
        EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(
                new WishlistPage());
        }
        catch (Exception ex)
        {
            await ShowMessage(
                "Wishlist",
                ex.Message);
        }
    }


    // =====================================================
    // CART
    // =====================================================

    private async void CartHeader_Clicked(
        object sender,
        EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(
                new CartPage());
        }
        catch (Exception ex)
        {
            await ShowMessage(
                "Cart",
                ex.Message);
        }
    }


    // =====================================================
    // CATEGORIES
    // =====================================================

    private async void CategoriesHeader_Clicked(
        object sender,
        EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(
                new CategoryProductsPage("Fruits"));
        }
        catch (Exception ex)
        {
            await ShowMessage(
                "Categories",
                ex.Message);
        }
    }


    // =====================================================
    // NOTIFICATIONS
    // =====================================================

    private async void NotificationHeader_Clicked(
        object sender,
        EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(
                new NotificationsPage());
        }
        catch (Exception ex)
        {
            await ShowMessage(
                "Notifications",
                ex.Message);
        }
    }


    // =====================================================
    // PROFILE
    // =====================================================

    private async void ProfileHeader_Clicked(
        object sender,
        EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(
                new CustomerProfilePage());
        }
        catch (Exception ex)
        {
            await ShowMessage(
                "Profile",
                ex.Message);
        }
    }


    // =====================================================
    // MESSAGE
    // =====================================================

    private async Task ShowMessage(
        string title,
        string message)
    {
        Page? page =
            Application.Current?
                .Windows
                .FirstOrDefault()?
                .Page;

        if (page != null)
        {
            await page.DisplayAlert(
                title,
                message,
                "OK");
        }
    }
}