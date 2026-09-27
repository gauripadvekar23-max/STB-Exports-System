using System;
using System.Collections.ObjectModel;
using STB_Exports.Models;

namespace STB_Exports.Views;

public partial class SearchResultsPage : ContentPage
{
    public ObservableCollection<Product> SearchProducts { get; set; }

    public SearchResultsPage(string searchText)
    {
        InitializeComponent();

        SearchProducts = new ObservableCollection<Product>();

        SearchResultsCollection.ItemsSource = SearchProducts;

        SearchTextLabel.Text =
            $"Results for \"{searchText}\"";

        LoadSearchResults(searchText);
    }

    // =====================================================
    // LOAD SEARCH RESULTS
    // =====================================================

    private void LoadSearchResults(string searchText)
    {
        SearchProducts.Clear();

        if (string.IsNullOrWhiteSpace(searchText))
        {
            NoResultsLabel.IsVisible = false;
            return;
        }

        string search = searchText.Trim();

        foreach (Product product in ProductCatalogue.Products)
        {
            if (product.Name != null &&
                product.Name.Contains(
                    search,
                    StringComparison.OrdinalIgnoreCase))
            {
                SearchProducts.Add(product);
            }
        }

        NoResultsLabel.IsVisible =
            SearchProducts.Count == 0;
    }

    // =====================================================
    // PRODUCT DETAILS
    // =====================================================

    private async void ProductDetails_Clicked(
        object sender,
        EventArgs e)
    {
        if (sender is Button button &&
            button.BindingContext is Product product)
        {
            await Navigation.PushAsync(
                new ProductsDetailsPage(product));
        }
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
}