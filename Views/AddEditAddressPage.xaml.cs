using System.Net.Http.Json;
using Microsoft.Maui.Storage;
using STB_Exports.Models;

namespace STB_Exports.Views;

public partial class AddEditAddressPage : ContentPage
{
    private readonly HttpClient _httpClient;

    private readonly string _addressId;

    private bool _isSaving;

    // =====================================================
    // CONSTRUCTOR
    // =====================================================

    public AddEditAddressPage(
        string addressId = "")
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

        _addressId =
            addressId?.Trim() ?? "";

        if (!string.IsNullOrWhiteSpace(_addressId))
        {
            PageTitle.Text = "Edit Address";
            SaveButton.Text = "Update Address";
        }
        else
        {
            PageTitle.Text = "Add New Address";
            SaveButton.Text = "Save Address";

            AddressTypePicker.SelectedIndex = 0;
        }
    }

    // =====================================================
    // PAGE APPEARING
    // =====================================================

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!string.IsNullOrWhiteSpace(_addressId))
        {
            await LoadAddressAsync();
        }
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
    // LOAD ADDRESS
    // =====================================================

    private async Task LoadAddressAsync()
    {
        try
        {
            string customerId =
                GetApiCustomerId();

            if (string.IsNullOrWhiteSpace(customerId))
            {
                await DisplayAlert(
                    "Login Required",
                    "Please login before editing an address.",
                    "OK");

                return;
            }

            HttpResponseMessage response =
                await _httpClient.GetAsync(
                    $"/api/Addresses/{_addressId}");

            if (!response.IsSuccessStatusCode)
            {
                await DisplayAlert(
                    "Address Not Found",
                    "The selected address could not be found.",
                    "OK");

                return;
            }

            ApiAddress? address =
                await response.Content
                    .ReadFromJsonAsync<ApiAddress>();

            if (address == null)
            {
                await DisplayAlert(
                    "Address Not Found",
                    "Address information was not received.",
                    "OK");

                return;
            }

            // Security check
            if (address.CustomerId != customerId)
            {
                await DisplayAlert(
                    "Access Denied",
                    "This address does not belong to your account.",
                    "OK");

                return;
            }

            FullNameEntry.Text =
                address.FullName;

            PhoneEntry.Text =
                address.Phone;

            AddressEntry.Text =
                address.AddressLine;

            CityEntry.Text =
                address.City;

            StateEntry.Text =
                address.State;

            PincodeEntry.Text =
                address.Pincode;

            // Existing API model has IsDefault,
            // so use Home as the default display type.
            AddressTypePicker.SelectedIndex = 0;
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
                "Unable to load address.\n\n" +
                ex.Message,
                "OK");
        }
    }

    // =====================================================
    // SAVE / UPDATE
    // =====================================================

    private async void SaveAddress_Clicked(
        object sender,
        EventArgs e)
    {
        if (_isSaving)
            return;

        _isSaving = true;
        SaveButton.IsEnabled = false;

        try
        {
            string customerId =
                GetApiCustomerId();

            if (string.IsNullOrWhiteSpace(customerId))
            {
                await DisplayAlert(
                    "Login Required",
                    "Please login before saving an address.",
                    "OK");

                return;
            }

            // =================================================
            // READ FORM
            // =================================================

            string name =
                FullNameEntry.Text?.Trim() ?? "";

            string phone =
                PhoneEntry.Text?.Trim() ?? "";

            string address =
                AddressEntry.Text?.Trim() ?? "";

            string city =
                CityEntry.Text?.Trim() ?? "";

            string state =
                StateEntry.Text?.Trim() ?? "";

            string pincode =
                PincodeEntry.Text?.Trim() ?? "";

            // =================================================
            // VALIDATION
            // =================================================

            if (string.IsNullOrWhiteSpace(name))
            {
                await DisplayAlert(
                    "Validation",
                    "Please enter your full name.",
                    "OK");

                return;
            }

            if (phone.Length != 10 ||
                !phone.All(char.IsDigit))
            {
                await DisplayAlert(
                    "Invalid Phone Number",
                    "Phone number must contain exactly 10 digits.",
                    "OK");

                return;
            }

            if (string.IsNullOrWhiteSpace(address))
            {
                await DisplayAlert(
                    "Validation",
                    "Please enter your address.",
                    "OK");

                return;
            }

            if (string.IsNullOrWhiteSpace(city))
            {
                await DisplayAlert(
                    "Validation",
                    "Please enter your city.",
                    "OK");

                return;
            }

            if (string.IsNullOrWhiteSpace(state))
            {
                await DisplayAlert(
                    "Validation",
                    "Please enter your state.",
                    "OK");

                return;
            }

            if (pincode.Length != 6 ||
                !pincode.All(char.IsDigit))
            {
                await DisplayAlert(
                    "Invalid Pincode",
                    "Pincode must contain exactly 6 digits.",
                    "OK");

                return;
            }

            if (AddressTypePicker.SelectedIndex < 0)
            {
                await DisplayAlert(
                    "Validation",
                    "Please select an address type.",
                    "OK");

                return;
            }

            // =================================================
            // CREATE API ADDRESS
            // =================================================

            ApiAddress apiAddress =
                new ApiAddress
                {
                    AddressId =
                        string.IsNullOrWhiteSpace(_addressId)
                            ? Guid.NewGuid().ToString()
                            : _addressId,

                    CustomerId =
                        customerId,

                    FullName =
                        name,

                    Phone =
                        phone,

                    AddressLine =
                        address,

                    City =
                        city,

                    State =
                        state,

                    Pincode =
                        pincode,

                    IsDefault =
                        false
                };

            // =================================================
            // UPDATE
            // =================================================

            if (!string.IsNullOrWhiteSpace(_addressId))
            {
                HttpResponseMessage response =
                    await _httpClient.PutAsJsonAsync(
                        $"/api/Addresses/{_addressId}",
                        apiAddress);

                if (!response.IsSuccessStatusCode)
                {
                    string error =
                        await response.Content
                            .ReadAsStringAsync();

                    await DisplayAlert(
                        "Update Failed",
                        string.IsNullOrWhiteSpace(error)
                            ? $"Unable to update address. Status: {(int)response.StatusCode}"
                            : error,
                        "OK");

                    return;
                }

                // Update selected address information
                string selectedAddressId =
                    Preferences.Get(
                        "SelectedAddressId",
                        string.Empty);

                if (selectedAddressId ==
                    _addressId)
                {
                    SaveSelectedAddressPreferences(
                        apiAddress);
                }

                await DisplayAlert(
                    "Address Updated",
                    "Your address has been updated successfully.",
                    "OK");
            }

            // =================================================
            // ADD
            // =================================================

            else
            {
                HttpResponseMessage response =
                    await _httpClient.PostAsJsonAsync(
                        "/api/Addresses",
                        apiAddress);

                if (!response.IsSuccessStatusCode)
                {
                    string error =
                        await response.Content
                            .ReadAsStringAsync();

                    await DisplayAlert(
                        "Save Failed",
                        string.IsNullOrWhiteSpace(error)
                            ? $"Unable to save address. Status: {(int)response.StatusCode}"
                            : error,
                        "OK");

                    return;
                }

                ApiAddress? createdAddress =
                    await response.Content
                        .ReadFromJsonAsync<ApiAddress>();

                if (createdAddress != null)
                {
                    // Automatically select newly added address
                    SaveSelectedAddressPreferences(
                        createdAddress);
                }

                await DisplayAlert(
                    "Address Saved",
                    "Your address has been saved successfully.",
                    "OK");
            }

            // =================================================
            // RETURN
            // =================================================

            if (Navigation.NavigationStack.Count > 1)
            {
                await Navigation.PopAsync();
            }
        }
        catch (HttpRequestException)
        {
            await DisplayAlert(
                "Connection Error",
                "Unable to connect to the API.\n\n" +
                "Make sure STB_Exports_API is running.",
                "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Address Error",
                "Something went wrong while saving the address.\n\n" +
                ex.Message,
                "OK");
        }
        finally
        {
            _isSaving = false;
            SaveButton.IsEnabled = true;
        }
    }

    // =====================================================
    // SAVE SELECTED ADDRESS
    // =====================================================

    private void SaveSelectedAddressPreferences(
        ApiAddress address)
    {
        Preferences.Set(
            "SelectedAddressId",
            address.AddressId);

        Preferences.Set(
            "SelectedAddressFullName",
            address.FullName);

        Preferences.Set(
            "SelectedAddressPhone",
            address.Phone);

        Preferences.Set(
            "SelectedAddressLine",
            address.AddressLine);

        Preferences.Set(
            "SelectedAddressCity",
            address.City);

        Preferences.Set(
            "SelectedAddressState",
            address.State);

        Preferences.Set(
            "SelectedAddressPincode",
            address.Pincode);
    }
}