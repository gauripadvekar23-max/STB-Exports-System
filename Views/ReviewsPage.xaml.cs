using Microsoft.Maui.Storage;
using STB_Exports.Data;
using STB_Exports.Models;

namespace STB_Exports.Views;

public partial class ReviewsPage : ContentPage
{
    private readonly DatabaseService _databaseService;


    // =====================================================
    // CONSTRUCTOR
    // =====================================================

    public ReviewsPage()
    {
        InitializeComponent();

        _databaseService =
            new DatabaseService();
    }


    // =====================================================
    // PAGE APPEARING
    // =====================================================

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await LoadReviewsAsync();
    }


    // =====================================================
    // LOAD MY REVIEWS
    // =====================================================

    private async Task LoadReviewsAsync()
    {
        try
        {
            int customerId =
                Preferences.Get(
                    "CustomerId",
                    0);


            // =================================================
            // LOGIN CHECK
            // =================================================

            if (customerId <= 0)
            {
                ReviewsCollectionView.ItemsSource =
                    null;

                ReviewsCollectionView.IsVisible =
                    false;

                EmptyReviewsLayout.IsVisible =
                    true;

                return;
            }


            // =================================================
            // GET REVIEWS
            // =================================================

            List<Review> reviews =
                await _databaseService
                    .GetCustomerReviewsAsync(
                        customerId);


            // =================================================
            // EMPTY
            // =================================================

            if (reviews.Count == 0)
            {
                ReviewsCollectionView.ItemsSource =
                    null;

                ReviewsCollectionView.IsVisible =
                    false;

                EmptyReviewsLayout.IsVisible =
                    true;

                return;
            }


            // =================================================
            // SHOW
            // =================================================

            ReviewsCollectionView.ItemsSource =
                reviews;

            ReviewsCollectionView.IsVisible =
                true;

            EmptyReviewsLayout.IsVisible =
                false;
        }
        catch (Exception ex)
        {
            ReviewsCollectionView.ItemsSource =
                null;

            ReviewsCollectionView.IsVisible =
                false;

            EmptyReviewsLayout.IsVisible =
                true;

            await DisplayAlert(
                "Reviews Error",
                ex.Message,
                "OK");
        }
    }


    // =====================================================
    // BACK
    // =====================================================

    private async void BackButton_Clicked(
        object sender,
        EventArgs e)
    {
        await Navigation.PopAsync();
    }
}