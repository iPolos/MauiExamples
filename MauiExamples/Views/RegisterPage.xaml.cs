using MauiExamples.Services;
using System.Diagnostics;

namespace MauiExamples.Views;

public partial class RegisterPage : ContentPage
{
    private readonly AuthService _authService;
    
    public RegisterPage(AuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }
    
    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        try
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(UsernameEntry.Text))
            {
                ShowError("Please enter a username");
                return;
            }
            
            if (string.IsNullOrWhiteSpace(EmailEntry.Text))
            {
                ShowError("Please enter an email address");
                return;
            }
            
            if (string.IsNullOrWhiteSpace(PasswordEntry.Text))
            {
                ShowError("Please enter a password");
                return;
            }
            
            if (PasswordEntry.Text != ConfirmPasswordEntry.Text)
            {
                ShowError("Passwords do not match");
                return;
            }
            
            // Simple email validation
            if (!EmailEntry.Text.Contains('@') || !EmailEntry.Text.Contains('.'))
            {
                ShowError("Please enter a valid email address");
                return;
            }
            
            SetLoadingState(true);
            
            // Register the user
            var success = await _authService.RegisterAsync(
                UsernameEntry.Text,
                PasswordEntry.Text,
                EmailEntry.Text);
            
            if (success)
            {
                Debug.WriteLine($"Registration successful for {UsernameEntry.Text}");
                
                // Show success message and go back to login
                await DisplayAlert("Success", "Account created successfully! You can now log in.", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                ShowError("Registration failed. Username may already be taken.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Registration error: {ex.Message}");
            ShowError("An error occurred during registration");
        }
        finally
        {
            SetLoadingState(false);
        }
    }
    
    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
    
    private void ShowError(string message)
    {
        MessageLabel.Text = message;
        MessageLabel.IsVisible = true;
    }
    
    private void SetLoadingState(bool isLoading)
    {
        LoadingIndicator.IsRunning = isLoading;
        LoadingIndicator.IsVisible = isLoading;
        RegisterButton.IsEnabled = !isLoading;
        CancelButton.IsEnabled = !isLoading;
        UsernameEntry.IsEnabled = !isLoading;
        EmailEntry.IsEnabled = !isLoading;
        PasswordEntry.IsEnabled = !isLoading;
        ConfirmPasswordEntry.IsEnabled = !isLoading;
    }
} 