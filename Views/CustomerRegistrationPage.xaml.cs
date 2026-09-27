using System.Net.Http.Json;
using Microsoft.Maui.Storage;

namespace STB_Exports.Views;

public class ApiRegistrationRequest
{
    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}

public class ApiRegistrationResponse
{
    public string CustomerId { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}


public partial class CustomerRegistrationPage : ContentPage
{
    public CustomerRegistrationPage()
    {
        InitializeComponent();
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
                "Name must contain at least 2 letters.";

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
                "Please enter a password.";

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
    // CONFIRM PASSWORD
    // =====================================================

    private void ConfirmPasswordEntry_TextChanged(
        object sender,
        TextChangedEventArgs e)
    {
        ValidateConfirmPassword();
    }


    private void ConfirmPasswordEntry_Unfocused(
        object sender,
        FocusEventArgs e)
    {
        ValidateConfirmPassword();
    }


    private bool ValidateConfirmPassword()
    {
        string password =
            PasswordEntry.Text ?? "";

        string confirmPassword =
            ConfirmPasswordEntry.Text ?? "";


        if (string.IsNullOrWhiteSpace(confirmPassword))
        {
            ConfirmPasswordFrame.BorderColor =
                Colors.Red;

            ConfirmPasswordErrorLabel.Text =
                "Please confirm your password.";

            ConfirmPasswordErrorLabel.IsVisible =
                true;

            return false;
        }


        if (password != confirmPassword)
        {
            ConfirmPasswordFrame.BorderColor =
                Colors.Red;

            ConfirmPasswordErrorLabel.Text =
                "Passwords do not match.";

            ConfirmPasswordErrorLabel.IsVisible =
                true;

            return false;
        }


        ConfirmPasswordFrame.BorderColor =
            Colors.Green;

        ConfirmPasswordErrorLabel.Text =
            "";

        ConfirmPasswordErrorLabel.IsVisible =
            false;

        return true;
    }


    // =====================================================
    // SHOW PASSWORD
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


    private void ShowConfirmPassword_Clicked(
        object sender,
        EventArgs e)
    {
        ConfirmPasswordEntry.IsPassword =
            !ConfirmPasswordEntry.IsPassword;

        ShowConfirmPasswordButton.Source =
            ConfirmPasswordEntry.IsPassword
                ? "eye_closed.png"
                : "eye.png";
    }


    // =====================================================
    // REGISTER
    // =====================================================

    private async void RegisterButton_Clicked(
        object sender,
        EventArgs e)
    {
        // =================================================
        // VALIDATION
        // =================================================

        bool validName =
            ValidateFullName();

        bool validEmail =
            ValidateEmail();

        bool validPassword =
            ValidatePassword();

        bool validConfirmPassword =
            ValidateConfirmPassword();


        if (!validName ||
            !validEmail ||
            !validPassword ||
            !validConfirmPassword)
        {
            return;
        }


        // =================================================
        // GET INPUT
        // =================================================

        string fullName =
            FullNameEntry.Text?.Trim() ?? "";

        string email =
            EmailEntry.Text?
                .Trim()
                .ToLowerInvariant() ?? "";

        string password =
            PasswordEntry.Text ?? "";


        try
        {
            // =================================================
            // CREATE HTTP CLIENT
            // =================================================

            using HttpClient client =
                new HttpClient();


            // =================================================
            // CREATE REGISTRATION REQUEST
            // =================================================

            var registrationRequest =
                new ApiRegistrationRequest
                {
                    FullName = fullName,
                    Email = email,
                    Password = password
                };


            // =================================================
            // CALL CUSTOMER REGISTRATION API
            // =================================================

            HttpResponseMessage response =
                await client.PostAsJsonAsync(
                    "https://localhost:7197/api/Customers",
                    registrationRequest);


            // =================================================
            // DUPLICATE EMAIL
            // =================================================

            if (response.StatusCode ==
                System.Net.HttpStatusCode.Conflict)
            {
                EmailFrame.BorderColor =
                    Colors.Red;

                EmailErrorLabel.Text =
                    "This email is already registered.";

                EmailErrorLabel.IsVisible =
                    true;

                return;
            }


            // =================================================
            // OTHER REGISTRATION ERRORS
            // =================================================

            if (!response.IsSuccessStatusCode)
            {
                string errorMessage =
                    await response.Content.ReadAsStringAsync();

                await DisplayAlert(
                    "Registration Failed",
                    errorMessage,
                    "OK");

                return;
            }


            // =================================================
            // READ API RESPONSE
            // =================================================

            ApiRegistrationResponse? customer =
                await response.Content
                    .ReadFromJsonAsync<ApiRegistrationResponse>();


            if (customer == null)
            {
                await DisplayAlert(
                    "Registration Failed",
                    "The server did not return customer information.",
                    "OK");

                return;
            }


            // =================================================
            // CLEAR OLD LOGIN INFORMATION
            // =================================================

            Preferences.Remove("CustomerId");
            Preferences.Remove("ApiCustomerId");
            Preferences.Remove("CustomerName");
            Preferences.Remove("CustomerEmail");
            Preferences.Remove("LoggedInCustomerEmail");

            Preferences.Set(
                "IsCustomerLoggedIn",
                false);


            // =================================================
            // SUCCESS MESSAGE
            // =================================================

            await DisplayAlert(
                "Registration Successful",
                "Your customer account has been created successfully.\n\nPlease login with your email and password.",
                "OK");


            // =================================================
            // GO TO LOGIN
            // =================================================

            await Shell.Current.GoToAsync(
                "//CustomerLoginPage");
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
                "Registration Error",
                ex.Message,
                "OK");
        }
    }


    // =====================================================
    // BACK TO LOGIN
    // =====================================================

    private async void BackToLoginButton_Clicked(
        object sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync(
            "//CustomerLoginPage");
    }
}