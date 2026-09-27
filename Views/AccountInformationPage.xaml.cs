using System.Net.Http.Json;
using Microsoft.Maui.Storage;
using STB_Exports.Models;

namespace STB_Exports.Views;

public partial class AccountInformationPage : ContentPage
{
    private readonly HttpClient _httpClient;

    private List<ApiAddressDisplayItem> _savedAddresses = new();

    private readonly bool _isSelectingAddress;

    public bool IsSelectingAddress
    {
        get
        {
            return _isSelectingAddress;
        }
    }

    public AccountInformationPage(
        bool isSelectingAddress = false)
    {
        InitializeComponent();

        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                (message, certificate, chain, errors) => true
        };

        _httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(
                "https://localhost:7197")
        };

        _isSelectingAddress =
            isSelectingAddress;
    }

    // =====================================================
    // PAGE APPEARING
    // =====================================================

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await LoadSavedAddressesAsync();
    }

    // =====================================================
    // GET API CUSTOMER ID
    // =====================================================

    private string GetApiCustomerId()
    {
        return Preferences.Get(
            "ApiCustomerId",
            string.Empty).Trim();
    }

    // =====================================================
    // LOAD ADDRESSES FROM API
    // =====================================================

    private async Task LoadSavedAddressesAsync()
    {
        try
        {
            string customerId =
                GetApiCustomerId();

            if (string.IsNullOrWhiteSpace(customerId))
            {
                _savedAddresses.Clear();

                AddressesCollection.ItemsSource = null;

                await DisplayAlert(
                    "Login Required",
                    "Please login before managing your addresses.",
                    "OK");

                return;
            }

            HttpResponseMessage response =
                await _httpClient.GetAsync(
                    $"/api/Addresses/customer/{customerId}");

            if (!response.IsSuccessStatusCode)
            {
                string error =
                    await response.Content.ReadAsStringAsync();

                await DisplayAlert(
                    "Address Error",
                    string.IsNullOrWhiteSpace(error)
                        ? $"Unable to load addresses. Status: {(int)response.StatusCode}"
                        : error,
                    "OK");

                return;
            }

            List<ApiAddress>? addresses =
                await response.Content
                    .ReadFromJsonAsync<List<ApiAddress>>();

            _savedAddresses =
                addresses?
                    .Select(a => new ApiAddressDisplayItem
                    {
                        AddressId = a.AddressId,
                        CustomerId = a.CustomerId,
                        FullName = a.FullName,
                        PhoneNumber = a.Phone,
                        Address = a.AddressLine,
                        City = a.City,
                        State = a.State,
                        Pincode = a.Pincode,
                        AddressType =
                            a.IsDefault ? "Default" : "Address",
                        IsDefault = a.IsDefault
                    })
                    .ToList()
                ?? new List<ApiAddressDisplayItem>();

            AddressesCollection.ItemsSource =
                null;

            AddressesCollection.ItemsSource =
                _savedAddresses;
        }
        catch (HttpRequestException)
        {
            await DisplayAlert(
                "Connection Error",
                "Unable to connect to the API.\n\n" +
                "Please make sure STB_Exports_API is running.",
                "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Address Error",
                "Unable to load saved addresses.\n\n" +
                ex.Message,
                "OK");
        }
    }

    // =====================================================
    // SELECT ADDRESS
    // =====================================================

    private async void SelectAddress_Clicked(
        object sender,
        EventArgs e)
    {
        try
        {
            if (sender is not Button button)
                return;

            if (button.BindingContext
                is not ApiAddressDisplayItem address)
                return;

            // Save API address ID as STRING
            Preferences.Set(
                "SelectedAddressId",
                address.AddressId);

            // Also save complete selected address information
            Preferences.Set(
                "SelectedAddressFullName",
                address.FullName);

            Preferences.Set(
                "SelectedAddressPhone",
                address.PhoneNumber);

            Preferences.Set(
                "SelectedAddressLine",
                address.Address);

            Preferences.Set(
                "SelectedAddressCity",
                address.City);

            Preferences.Set(
                "SelectedAddressState",
                address.State);

            Preferences.Set(
                "SelectedAddressPincode",
                address.Pincode);

            // =================================================
            // RETURN TO CHECKOUT
            // =================================================

            if (_isSelectingAddress)
            {
                if (Navigation.NavigationStack.Count > 1)
                {
                    await Navigation.PopAsync();
                }

                return;
            }

            await DisplayAlert(
                "Address Selected",
                "This address has been selected.",
                "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Address Error",
                "Unable to select address.\n\n" +
                ex.Message,
                "OK");
        }
    }

    // =====================================================
    // ADD NEW ADDRESS
    // =====================================================

    private async void AddAddress_Clicked(
        object sender,
        EventArgs e)
    {
        try
        {
            string customerId =
                GetApiCustomerId();

            if (string.IsNullOrWhiteSpace(customerId))
            {
                await DisplayAlert(
                    "Login Required",
                    "Please login before adding an address.",
                    "OK");

                return;
            }

            // Get current addresses from API
            HttpResponseMessage response =
                await _httpClient.GetAsync(
                    $"/api/Addresses/customer/{customerId}");

            if (!response.IsSuccessStatusCode)
            {
                await DisplayAlert(
                    "Address Error",
                    "Unable to check your saved addresses.",
                    "OK");

                return;
            }

            List<ApiAddress>? addresses =
                await response.Content
                    .ReadFromJsonAsync<List<ApiAddress>>();

            int count =
                addresses?.Count ?? 0;

            if (count >= 5)
            {
                await DisplayAlert(
                    "Address Limit",
                    "You can save a maximum of 5 addresses.",
                    "OK");

                return;
            }

            await Navigation.PushAsync(
                new AddEditAddressPage());
        }
        catch (HttpRequestException)
        {
            await DisplayAlert(
                "Connection Error",
                "Unable to connect to the API.",
                "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Address Error",
                "Unable to open Add Address page.\n\n" +
                ex.Message,
                "OK");
        }
    }

    // =====================================================
    // ADDRESS MENU
    // =====================================================

    private async void AddressMenu_Clicked(
        object sender,
        EventArgs e)
    {
        try
        {
            if (sender is not Button button)
                return;

            if (button.BindingContext
                is not ApiAddressDisplayItem address)
                return;

            string action =
                await DisplayActionSheet(
                    $"{address.AddressType} Address",
                    "Cancel",
                    null,
                    "Edit Address",
                    "Delete Address");

            // =================================================
            // EDIT
            // =================================================

            if (action == "Edit Address")
            {
                await Navigation.PushAsync(
                    new AddEditAddressPage(
                        address.AddressId));
            }

            // =================================================
            // DELETE
            // =================================================

            else if (action == "Delete Address")
            {
                await DeleteAddressAsync(
                    address.AddressId);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Address Error",
                "Unable to process address action.\n\n" +
                ex.Message,
                "OK");
        }
    }

    // =====================================================
    // DELETE ADDRESS
    // =====================================================

    private async Task DeleteAddressAsync(
        string addressId)
    {
        try
        {
            bool confirm =
                await DisplayAlert(
                    "Delete Address",
                    "Are you sure you want to delete this address?",
                    "Delete",
                    "Cancel");

            if (!confirm)
                return;

            HttpResponseMessage response =
                await _httpClient.DeleteAsync(
                    $"/api/Addresses/{addressId}");

            if (response.IsSuccessStatusCode)
            {
                string selectedAddressId =
                    Preferences.Get(
                        "SelectedAddressId",
                        string.Empty);

                if (selectedAddressId ==
                    addressId)
                {
                    Preferences.Remove(
                        "SelectedAddressId");

                    Preferences.Remove(
                        "SelectedAddressFullName");

                    Preferences.Remove(
                        "SelectedAddressPhone");

                    Preferences.Remove(
                        "SelectedAddressLine");

                    Preferences.Remove(
                        "SelectedAddressCity");

                    Preferences.Remove(
                        "SelectedAddressState");

                    Preferences.Remove(
                        "SelectedAddressPincode");
                }

                await DisplayAlert(
                    "Deleted",
                    "Address deleted successfully.",
                    "OK");

                await LoadSavedAddressesAsync();
            }
            else
            {
                string error =
                    await response.Content
                        .ReadAsStringAsync();

                await DisplayAlert(
                    "Delete Failed",
                    string.IsNullOrWhiteSpace(error)
                        ? "Unable to delete address."
                        : error,
                    "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Delete Error",
                "Unable to delete address.\n\n" +
                ex.Message,
                "OK");
        }
    }
}