using System.Net.Http.Json;
using Microsoft.Maui.Storage;
using STB_Exports.Data;
using STB_Exports.Models;

namespace STB_Exports.Views;


// =====================================================
// API LOGIN REQUEST
// =====================================================

public class ApiLoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}


// =====================================================
// API CUSTOMER RESPONSE
// =====================================================

public class ApiCustomerResponse
{
    public string CustomerId { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string Pincode { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}


// =====================================================
// CUSTOMER LOGIN PAGE
// =====================================================

public partial class CustomerLoginPage : ContentPage
{
    private readonly DatabaseService _databaseService;


    // =====================================================
    // CONSTRUCTOR
    // =====================================================

    public CustomerLoginPage()
    {
        InitializeComponent();

        _databaseService = new DatabaseService();

        ClearLoginFields();
    }


    // =====================================================
    // PAGE APPEARING
    // =====================================================

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Always show a fresh login form.
        // This prevents old/accidentally entered
        // information from appearing again.
        ClearLoginFields();
    }


    // =====================================================
    // CLEAR LOGIN FIELDS
    // =====================================================

    private void ClearLoginFields()
    {
        if (FullNameEntry != null)
            FullNameEntry.Text = string.Empty;

        if (EmailEntry != null)
            EmailEntry.Text = string.Empty;

        if (PasswordEntry != null)
            PasswordEntry.Text = string.Empty;

        if (ShowPasswordButton != null)
            ShowPasswordButton.Source = "eye_closed.png";

        if (PasswordEntry != null)
            PasswordEntry.IsPassword = true;

        // Reset borders
        if (FullNameFrame != null)
            FullNameFrame.BorderColor = Colors.LightGray;

        if (EmailFrame != null)
            EmailFrame.BorderColor = Colors.LightGray;

        if (PasswordFrame != null)
            PasswordFrame.BorderColor = Colors.LightGray;

        // Clear validation messages
        if (FullNameErrorLabel != null)
        {
            FullNameErrorLabel.Text = string.Empty;
            FullNameErrorLabel.IsVisible = false;
        }

        if (EmailErrorLabel != null)
        {
            EmailErrorLabel.Text = string.Empty;
            EmailErrorLabel.IsVisible = false;
        }

        if (PasswordErrorLabel != null)
        {
            PasswordErrorLabel.Text = string.Empty;
            PasswordErrorLabel.IsVisible = false;
        }
    }


    // =====================================================
    // REGISTER
    // =====================================================

    private async void RegisterButton_Clicked(
        object sender,
        EventArgs e)
    {
        // Clear anything the customer accidentally entered
        // before opening Create Account.
        ClearLoginFields();

        await Shell.Current.GoToAsync(
            "//CustomerRegistrationPage");
    }


    // =====================================================
    // SHOW / HIDE PASSWORD
    // =====================================================

    private void ShowPassword_Clicked(
        object sender,
        EventArgs e)
    {
        PasswordEntry.IsPassword =
            !PasswordEntry.IsPassword;

        ShowPasswordButton.Source =
            PasswordEntry.IsPassword
                ? "eye_closed.png"
                : "eye.png";
    }


    // =====================================================
    // FULL NAME
    // =====================================================

    private void FullNameEntry_TextChanged(
        object sender,
        TextChangedEventArgs e)
    {
        ValidateFullName();
    }


    private void FullNameEntry_Unfocused(
        object sender,
        FocusEventArgs e)
    {
        ValidateFullName();
    }


    private bool ValidateFullName()
    {
        string name =
            FullNameEntry.Text?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(name))
        {
            FullNameFrame.BorderColor =
                Colors.Red;

            FullNameErrorLabel.Text =
                "Please enter your full name.";

            FullNameErrorLabel.IsVisible =
                true;

            return false;
        }

        if (name.Length < 2)
        {
            FullNameFrame.BorderColor =
                Colors.Red;

            FullNameErrorLabel.Text =
                "Enter name with at least 2 letters.";

            FullNameErrorLabel.IsVisible =
                true;

            return false;
        }

        if (!System.Text.RegularExpressions.Regex.IsMatch(
                name,
                @"^[A-Za-z]+(?:[ '-][A-Za-z]+)*$"))
        {
            FullNameFrame.BorderColor =
                Colors.Red;

            FullNameErrorLabel.Text =
                "Please enter a valid full name using letters only.";

            FullNameErrorLabel.IsVisible =
                true;

            return false;
        }

        FullNameFrame.BorderColor =
            Colors.Green;

        FullNameErrorLabel.Text =
            "";

        FullNameErrorLabel.IsVisible =
            false;

        return true;
    }


    // =====================================================
    // EMAIL
    // =====================================================

    private void EmailEntry_TextChanged(
        object sender,
        TextChangedEventArgs e)
    {
        ValidateEmail();
    }


    private void EmailEntry_Unfocused(
        object sender,
        FocusEventArgs e)
    {
        ValidateEmail();
    }


    private bool ValidateEmail()
    {
        string email =
            EmailEntry.Text?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(email))
        {
            EmailFrame.BorderColor =
                Colors.Red;

            EmailErrorLabel.Text =
                "Please enter your email address.";

            EmailErrorLabel.IsVisible =
                true;

            return false;
        }

        if (!System.Text.RegularExpressions.Regex.IsMatch(
                email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            EmailFrame.BorderColor =
                Colors.Red;

            EmailErrorLabel.Text =
                "Please enter a valid email address.";

            EmailErrorLabel.IsVisible =
                true;

            return false;
        }

        EmailFrame.BorderColor =
            Colors.Green;

        EmailErrorLabel.Text =
            "";

        EmailErrorLabel.IsVisible =
            false;

        return true;
    }


    // =====================================================
    // PASSWORD
    // =====================================================

    private void PasswordEntry_TextChanged(
        object sender,
        TextChangedEventArgs e)
    {
        ValidatePassword();
    }


    private void PasswordEntry_Unfocused(
        object sender,
        FocusEventArgs e)
    {
        ValidatePassword();
    }


    private bool ValidatePassword()
    {
        string password =
            PasswordEntry.Text ?? "";

        if (string.IsNullOrWhiteSpace(password))
        {
            PasswordFrame.BorderColor =
                Colors.Red;

            PasswordErrorLabel.Text =
                "Please enter your password.";

            PasswordErrorLabel.IsVisible =
                true;

            return false;
        }

        if (password.Length < 6)
        {
            PasswordFrame.BorderColor =
                Colors.Red;

            PasswordErrorLabel.Text =
                "Password must contain at least 6 characters.";

            PasswordErrorLabel.IsVisible =
                true;

            return false;
        }

        PasswordFrame.BorderColor =
            Colors.Green;

        PasswordErrorLabel.Text =
            "";

        PasswordErrorLabel.IsVisible =
            false;

        return true;
    }


    // =====================================================
    // LOGIN
    // =====================================================

    private async void LoginButton_Clicked(
        object sender,
        EventArgs e)
    {
        // =================================================
        // VALIDATE
        // =================================================

        bool validEmail =
            ValidateEmail();

        bool validPassword =
            ValidatePassword();

        if (!validEmail || !validPassword)
            return;


        // =================================================
        // GET INPUT
        // =================================================

        string email =
            EmailEntry.Text?
                .Trim()
                .ToLowerInvariant() ?? "";

        string password =
            PasswordEntry.Text ?? "";


        try
        {
            // =================================================
            // API CLIENT
            // =================================================

            var handler =
                new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        (message, certificate, chain, errors) => true
                };

            using HttpClient client =
                new HttpClient(handler);


            // =================================================
            // LOGIN REQUEST
            // =================================================

            var loginRequest =
                new ApiLoginRequest
                {
                    Email = email,
                    Password = password
                };


            // =================================================
            // CALL API
            // =================================================

            HttpResponseMessage response =
                await client.PostAsJsonAsync(
                    "https://localhost:7197/api/Customers/login",
                    loginRequest);


            // =================================================
            // LOGIN FAILED
            // =================================================

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode ==
                    System.Net.HttpStatusCode.Unauthorized)
                {
                    PasswordFrame.BorderColor =
                        Colors.Red;

                    PasswordErrorLabel.Text =
                        "Incorrect email or password.";

                    PasswordErrorLabel.IsVisible =
                        true;

                    return;
                }

                string serverError =
                    await response.Content.ReadAsStringAsync();

                await DisplayAlert(
                    "Login Failed",
                    string.IsNullOrWhiteSpace(serverError)
                        ? $"Unable to login. Server returned {(int)response.StatusCode}."
                        : serverError,
                    "OK");

                return;
            }


            // =================================================
            // READ API CUSTOMER
            // =================================================

            ApiCustomerResponse? customer =
                await response.Content
                    .ReadFromJsonAsync<ApiCustomerResponse>();


            if (customer == null ||
                string.IsNullOrWhiteSpace(customer.CustomerId))
            {
                await DisplayAlert(
                    "Login Failed",
                    "Customer information was not received from the server.",
                    "OK");

                return;
            }


            // =================================================
            // FIND / CREATE LOCAL CUSTOMER
            // =================================================

            Customer? localCustomer =
                await _databaseService
                    .GetCustomerByEmailAsync(customer.Email);


            if (localCustomer == null)
            {
                localCustomer =
                    new Customer
                    {
                        FullName =
                            customer.FullName,

                        Email =
                            customer.Email
                                .Trim()
                                .ToLowerInvariant(),

                        Password =
                            customer.Password,

                        PhoneNumber =
                            customer.PhoneNumber,

                        Address =
                            customer.Address,

                        City =
                            customer.City,

                        State =
                            customer.State,

                        Pincode =
                            customer.Pincode,

                        IsDeactivated =
                            false
                    };


                int localCustomerId =
                    await _databaseService
                        .RegisterCustomerAsync(
                            localCustomer);


                if (localCustomerId <= 0)
                {
                    localCustomer =
                        await _databaseService
                            .GetCustomerByEmailAsync(
                                customer.Email);
                }
                else
                {
                    localCustomer.CustomerId =
                        localCustomerId;
                }
            }


            // =================================================
            // MAKE SURE LOCAL CUSTOMER EXISTS
            // =================================================

            if (localCustomer == null ||
                localCustomer.CustomerId <= 0)
            {
                await DisplayAlert(
                    "Login Error",
                    "Unable to create local customer information.",
                    "OK");

                return;
            }


            // =================================================
            // SAVE BOTH CUSTOMER IDS
            // =================================================

            Preferences.Set(
                "CustomerId",
                localCustomer.CustomerId);

            Preferences.Set(
                "ApiCustomerId",
                customer.CustomerId);


            // =================================================
            // SAVE CUSTOMER INFORMATION
            // =================================================

            Preferences.Set(
                "CustomerName",
                customer.FullName);

            Preferences.Set(
                "CustomerEmail",
                customer.Email);

            Preferences.Set(
                "LoggedInCustomerEmail",
                customer.Email);

            Preferences.Set(
                "CustomerPhone",
                customer.PhoneNumber);

            Preferences.Set(
                "IsCustomerLoggedIn",
                true);


            // =================================================
            // SUCCESS UI
            // =================================================

            EmailFrame.BorderColor =
                Colors.Green;

            PasswordFrame.BorderColor =
                Colors.Green;

            EmailErrorLabel.Text =
                "";

            EmailErrorLabel.IsVisible =
                false;

            PasswordErrorLabel.Text =
                "";

            PasswordErrorLabel.IsVisible =
                false;


            // =================================================
            // SUCCESS MESSAGE
            // =================================================

            await DisplayAlert(
                "Login Successful",
                $"Welcome, {customer.FullName}!",
                "OK");


            // =================================================
            // CLEAR LOGIN FIELDS
            // =================================================

            ClearLoginFields();


            // =================================================
            // GO TO HOME
            // =================================================

            await Shell.Current.GoToAsync(
                "//HomePage");
        }
        catch (HttpRequestException ex)
        {
            await DisplayAlert(
                "Connection Error",
                "Unable to connect to the API.\n\n" +
                "Make sure STB_Exports_API is running.\n\n" +
                ex.Message,
                "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Login Error",
                ex.Message,
                "OK");
        }
    }
}