using Microsoft.Maui.Storage;
using STB_Exports.Data;
using STB_Exports.Models;

namespace STB_Exports.Views;

public partial class WriteReviewPage : ContentPage
{
    private readonly DatabaseService _databaseService;

    private readonly Product _product;

    private int _selectedRating;


    // =====================================================
    // CONSTRUCTOR
    // =====================================================

    public WriteReviewPage(Product product)
    {
        InitializeComponent();

        _databaseService = new DatabaseService();

        _product = product;

        ProductNameLabel.Text =
            _product.Name;

        ProductImage.Source =
            _product.Image;

        UpdateRatingStars();
    }


    // =====================================================
    // RATING
    // =====================================================

    private void RatingButton_Clicked(
        object sender,
        EventArgs e)
    {
        if (sender is not Button button)
            return;

        if (!int.TryParse(
                button.CommandParameter?.ToString(),
                out int rating))
        {
            return;
        }

        _selectedRating = rating;

        UpdateRatingStars();
    }


    // =====================================================
    // UPDATE STARS
    // =====================================================

    private void UpdateRatingStars()
    {
        Rating1Button.Text =
            _selectedRating >= 1 ? "★" : "☆";

        Rating2Button.Text =
            _selectedRating >= 2 ? "★" : "☆";

        Rating3Button.Text =
            _selectedRating >= 3 ? "★" : "☆";

        Rating4Button.Text =
            _selectedRating >= 4 ? "★" : "☆";

        Rating5Button.Text =
            _selectedRating >= 5 ? "★" : "☆";


        if (_selectedRating == 0)
        {
            RatingLabel.Text =
                "Select a rating";
        }
        else
        {
            RatingLabel.Text =
                $"{_selectedRating} out of 5 stars";
        }
    }


    // =====================================================
    // SUBMIT REVIEW
    // =====================================================

    private async void SubmitReviewButton_Clicked(
        object sender,
        EventArgs e)
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
                await DisplayAlert(
                    "Login Required",
                    "Please login before writing a review.",
                    "OK");

                return;
            }


            // =================================================
            // RATING VALIDATION
            // =================================================

            if (_selectedRating < 1 ||
                _selectedRating > 5)
            {
                await DisplayAlert(
                    "Rating Required",
                    "Please select a rating from 1 to 5 stars.",
                    "OK");

                return;
            }


            // =================================================
            // REVIEW VALIDATION
            // =================================================

            string reviewText =
                ReviewEditor.Text?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(reviewText))
            {
                await DisplayAlert(
                    "Review Required",
                    "Please write a review before submitting.",
                    "OK");

                return;
            }


            if (reviewText.Length < 5)
            {
                await DisplayAlert(
                    "Review Too Short",
                    "Please write at least 5 characters.",
                    "OK");

                return;
            }


            // =================================================
            // CHECK ELIGIBILITY
            // =================================================

            bool canReview =
                await _databaseService
                    .CanCustomerReviewProductAsync(
                        customerId,
                        _product.ProductId);

            if (!canReview)
            {
                await DisplayAlert(
                    "Review Not Available",
                    "You can review this product only after receiving your order.",
                    "OK");

                return;
            }


            // =================================================
            // CREATE REVIEW
            // =================================================

            Review review =
                new Review
                {
                    CustomerId =
                        customerId,

                    ProductId =
                        _product.ProductId,

                    Rating =
                        _selectedRating,

                    ReviewText =
                        reviewText,

                    ReviewDate =
                        DateTime.Now.ToString(
                            "dd MMM yyyy")
                };


            // =================================================
            // SAVE
            // =================================================

            int result =
                await _databaseService
                    .AddReviewAsync(review);


            if (result <= 0)
            {
                await DisplayAlert(
                    "Review Error",
                    "Unable to submit your review. You may have already reviewed this product.",
                    "OK");

                return;
            }


            // =================================================
            // SUCCESS
            // =================================================

            await DisplayAlert(
                "Review Submitted",
                "Thank you! Your review has been submitted successfully.",
                "OK");

            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Review Error",
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