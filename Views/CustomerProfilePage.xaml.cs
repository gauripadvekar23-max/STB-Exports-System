using System.Text.RegularExpressions;
using Microsoft.Maui.Storage;
using STB_Exports.Data;
using STB_Exports.Models;

namespace STB_Exports.Views;

public partial class CustomerProfilePage : ContentPage
{
    private readonly DatabaseService _databaseService;

    public CustomerProfilePage()
    {
        InitializeComponent();

        _databaseService = new DatabaseService();

        LoadCustomerInformation();
    }


    // =====================================================
    // GET CURRENT CUSTOMER ID
    // =====================================================

    private int GetCustomerId()
    {
        return Preferences.Get(
            "CustomerId",
            0);
    }


    // =====================================================
    // GET CURRENT CUSTOMER KEY
    // =====================================================

    private string GetCustomerKey()
    {
        string email =
            Preferences.Get(
                "LoggedInCustomerEmail",
                "");

        if (string.IsNullOrWhiteSpace(email))
            return "";

        return email
            .Trim()
            .ToLowerInvariant()
            .Replace("@", "_")
            .Replace(".", "_");
    }


    // =====================================================
    // LOAD CUSTOMER INFORMATION
    // =====================================================

    private async void LoadCustomerInformation()
    {
        try
        {
            int customerId = GetCustomerId();

            string key = GetCustomerKey();


            // =================================================
            // CUSTOMER NOT LOGGED IN
            // =================================================

            if (customerId <= 0 ||
                string.IsNullOrWhiteSpace(key))
            {
                ClearProfile();

                SetFieldsReadOnly(false);

                ProfileActionButton.Text =
                    "SAVE PROFILE";

                return;
            }


            // =================================================
            // LOAD PROFILE FROM SQLITE
            // =================================================

            CustomerProfile? profile =
                await _databaseService
                    .GetCustomerProfileAsync(customerId);


            if (profile != null)
            {
                FullNameEntry.Text =
                    profile.FullName;

                EmailEntry.Text =
                    profile.Email;

                PhoneEntry.Text =
                    profile.PhoneNumber;


                // Profile exists in database
                SetFieldsReadOnly(true);

                ProfileActionButton.Text =
                    "EDIT PROFILE";

                return;
            }


            // =================================================
            // FALLBACK TO PREFERENCES
            // =================================================

            FullNameEntry.Text =
                Preferences.Get(
                    $"CustomerName_{key}",
                    "");

            EmailEntry.Text =
                Preferences.Get(
                    $"CustomerEmail_{key}",
                    Preferences.Get(
                        "LoggedInCustomerEmail",
                        ""));

            PhoneEntry.Text =
                Preferences.Get(
                    $"CustomerPhone_{key}",
                    "");

            AddressEntry.Text =
                Preferences.Get(
                    $"CustomerAddress_{key}",
                    "");

            CityEntry.Text =
                Preferences.Get(
                    $"CustomerCity_{key}",
                    "");

            DistrictEntry.Text =
                Preferences.Get(
                    $"CustomerDistrict_{key}",
                    "");

            StateEntry.Text =
                Preferences.Get(
                    $"CustomerState_{key}",
                    "");

            PincodeEntry.Text =
                Preferences.Get(
                    $"CustomerPincode_{key}",
                    "");


            // =================================================
            // CHECK WHETHER PROFILE WAS SAVED
            // =================================================

            bool profileSaved =
                Preferences.Get(
                    $"CustomerProfileSaved_{key}",
                    false);


            if (profileSaved)
            {
                SetFieldsReadOnly(true);

                ProfileActionButton.Text =
                    "EDIT PROFILE";
            }
            else
            {
                SetFieldsReadOnly(false);

                ProfileActionButton.Text =
                    "SAVE PROFILE";
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Profile Error",
                ex.Message,
                "OK");
        }
    }


    // =====================================================
    // CLEAR PROFILE
    // =====================================================

    private void ClearProfile()
    {
        FullNameEntry.Text = "";
        EmailEntry.Text = "";
        PhoneEntry.Text = "";
        AddressEntry.Text = "";
        CityEntry.Text = "";
        DistrictEntry.Text = "";
        StateEntry.Text = "";
        PincodeEntry.Text = "";
    }


    // =====================================================
    // SAVE / EDIT PROFILE
    // =====================================================

    private async void ProfileActionButton_Clicked(
        object sender,
        EventArgs e)
    {
        try
        {
            int customerId =
                GetCustomerId();

            string key =
                GetCustomerKey();


            // =================================================
            // LOGIN CHECK
            // =================================================

            if (customerId <= 0 ||
                string.IsNullOrWhiteSpace(key))
            {
                await DisplayAlert(
                    "Login Required",
                    "Please login before saving your profile.",
                    "OK");

                return;
            }


            // =================================================
            // EDIT MODE
            // =================================================

            if (ProfileActionButton.Text ==
                "EDIT PROFILE")
            {
                SetFieldsReadOnly(false);

                ProfileActionButton.Text =
                    "SAVE PROFILE";

                return;
            }


            // =================================================
            // VALIDATION
            // =================================================

            if (string.IsNullOrWhiteSpace(
                FullNameEntry.Text))
            {
                await DisplayAlert(
                    "Missing Information",
                    "Please enter your full name.",
                    "OK");

                return;
            }


            // =================================================
            // EMAIL VALIDATION
            // =================================================

            string email =
                EmailEntry.Text?.Trim() ?? "";

            if (!Regex.IsMatch(
                email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                await DisplayAlert(
                    "Invalid Email",
                    "Please enter a valid email address.",
                    "OK");

                return;
            }


            // =================================================
            // PHONE VALIDATION
            // =================================================

            string phone =
                PhoneEntry.Text?.Trim() ?? "";

            if (!Regex.IsMatch(
                phone,
                @"^[6-9]\d{9}$"))
            {
                await DisplayAlert(
                    "Invalid Phone Number",
                    "Please enter a valid 10-digit Indian mobile number starting with 6, 7, 8, or 9.",
                    "OK");

                return;
            }


            // =================================================
            // ADDRESS
            // =================================================

            if (string.IsNullOrWhiteSpace(
                AddressEntry.Text))
            {
                await DisplayAlert(
                    "Missing Information",
                    "Please enter your complete delivery address.",
                    "OK");

                return;
            }


            // =================================================
            // CITY
            // =================================================

            if (string.IsNullOrWhiteSpace(
                CityEntry.Text))
            {
                await DisplayAlert(
                    "Missing Information",
                    "Please enter your city.",
                    "OK");

                return;
            }


            // =================================================
            // DISTRICT
            // =================================================

            if (string.IsNullOrWhiteSpace(
                DistrictEntry.Text))
            {
                await DisplayAlert(
                    "Missing Information",
                    "Please enter your district.",
                    "OK");

                return;
            }


            // =================================================
            // STATE
            // =================================================

            if (string.IsNullOrWhiteSpace(
                StateEntry.Text))
            {
                await DisplayAlert(
                    "Missing Information",
                    "Please enter your state.",
                    "OK");

                return;
            }


            // =================================================
            // PINCODE
            // =================================================

            string pincode =
                PincodeEntry.Text?.Trim() ?? "";

            if (!Regex.IsMatch(
                pincode,
                @"^\d{6}$"))
            {
                await DisplayAlert(
                    "Invalid Pincode",
                    "Please enter a valid 6-digit pincode.",
                    "OK");

                return;
            }


            // =================================================
            // CREATE PROFILE OBJECT
            // =================================================

            CustomerProfile profile =
                new CustomerProfile
                {
                    CustomerId =
                        customerId,

                    FullName =
                        FullNameEntry.Text.Trim(),

                    Email =
                        email,

                    PhoneNumber =
                        phone,
                };


            // =================================================
            // SAVE TO SQLITE
            // =================================================

            int result =
                await _databaseService
                    .SaveCustomerProfileAsync(
                        profile);


            if (result <= 0)
            {
                await DisplayAlert(
                    "Save Failed",
                    "Your profile could not be saved. Please try again.",
                    "OK");

                return;
            }


            // =================================================
            // SAVE BACKUP TO PREFERENCES
            // =================================================

            Preferences.Set(
                $"CustomerName_{key}",
                profile.FullName);

            Preferences.Set(
                $"CustomerEmail_{key}",
                profile.Email);

            Preferences.Set(
                $"CustomerPhone_{key}",
                profile.PhoneNumber);


            // =================================================
            // MARK PROFILE AS SAVED
            // =================================================

            Preferences.Set(
                $"CustomerProfileSaved_{key}",
                true);


            // =================================================
            // LOCK FIELDS
            // =================================================

            SetFieldsReadOnly(true);


            // =================================================
            // VERY IMPORTANT
            // ALWAYS CHANGE BUTTON BACK TO EDIT
            // =================================================

            ProfileActionButton.Text =
                "EDIT PROFILE";


            // =================================================
            // SUCCESS
            // =================================================

            await DisplayAlert(
                "Profile Saved",
                "Your profile has been saved successfully.",
                "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Profile Error",
                ex.Message,
                "OK");
        }
    }


    // =====================================================
    // ENABLE / DISABLE PROFILE FIELDS
    // =====================================================

    private void SetFieldsReadOnly(
        bool readOnly)
    {
        FullNameEntry.IsReadOnly =
            readOnly;

        EmailEntry.IsReadOnly =
            readOnly;

        PhoneEntry.IsReadOnly =
            readOnly;

        AddressEntry.IsReadOnly =
            readOnly;

        CityEntry.IsReadOnly =
            readOnly;

        DistrictEntry.IsReadOnly =
            readOnly;

        StateEntry.IsReadOnly =
            readOnly;

        PincodeEntry.IsReadOnly =
            readOnly;
    }


    // =====================================================
    // ORDERS
    // =====================================================

    private async void OrdersButton_Clicked(
    object sender,
    EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(
                new OrdersPage());
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Orders",
                $"Unable to open orders.\n\n{ex.Message}",
                "OK");
        }
    }


    // =====================================================
    // REVIEWS
    // =====================================================

    private async void ReviewsButton_Clicked(
    object sender,
    EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(
                new ReviewsPage());
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Reviews",
                $"Unable to open reviews.\n\n{ex.Message}",
                "OK");
        }
    }


    // =====================================================
    // WISHLIST
    // =====================================================

    private async void WishlistButton_Clicked(
        object sender,
        EventArgs e)
    {
        await Navigation.PushAsync(
     new WishlistPage());
    }


    // =====================================================
    // HELP CENTER
    // =====================================================

    private async void HelpCenterButton_Clicked(
        object sender,
        EventArgs e)
    {
        await Navigation.PushAsync(
    new HelpCenterPage());
    }


    // =====================================================
    // SETTINGS
    // =====================================================

    private async void SettingsButton_Clicked(
        object sender,
        EventArgs e)
    {
        await Navigation.PushAsync(
    new SettingsPage());
    }


    // =====================================================
    // LOGOUT
    // =====================================================

    private async void LogoutButton_Clicked(
        object sender,
        EventArgs e)
    {
        bool logout =
            await DisplayAlert(
                "Logout",
                "Are you sure you want to logout?",
                "Yes",
                "No");

        if (!logout)
            return;


        Preferences.Remove(
            "LoggedInCustomerEmail");

        Preferences.Set(
            "IsCustomerLoggedIn",
            false);


        await Shell.Current.GoToAsync(
            "//CustomerLoginPage");
    }
}