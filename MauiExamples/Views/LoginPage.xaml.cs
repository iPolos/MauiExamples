using MauiExamples.Services;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;

namespace MauiExamples.Views;

public partial class LoginPage : ContentPage
{
    private readonly AuthService _authService;
    
    public LoginPage(AuthService authService)
    {
        InitializeComponent();
        _authService = authService;
        
        // If already authenticated, skip login
        if (_authService.IsAuthenticated)
        {
            Debug.WriteLine($"User already authenticated as {_authService.CurrentUsername}, role: {_authService.CurrentRole}");
            GoToMainPage();
        }
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        // Clear any previous entries
        UsernameEntry.Text = string.Empty;
        PasswordEntry.Text = string.Empty;
        MessageLabel.IsVisible = false;
    }
    
    private async void OnLoginClicked(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(UsernameEntry.Text) || string.IsNullOrWhiteSpace(PasswordEntry.Text))
            {
                ShowError("Please enter both username and password");
                return;
            }
            
            SetLoadingState(true);
            
            var success = await _authService.LoginAsync(UsernameEntry.Text, PasswordEntry.Text);
            
            if (success)
            {
                Debug.WriteLine($"Login successful for {_authService.CurrentUsername}, role: {_authService.CurrentRole}");
                await GoToMainPage();
            }
            else
            {
                ShowError("Invalid username or password");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Login error: {ex.Message}");
            // Show a more specific error message to help with debugging
            string errorMessage = "An error occurred during login";
            
            // Check for common network-related exceptions
            if (ex is HttpRequestException)
            {
                errorMessage = "Cannot connect to the server. Please check your network connection and ensure the API is running.";
            }
            else if (ex is TaskCanceledException)
            {
                errorMessage = "Login request timed out. Please try again.";
            }
            else if (ex is JsonException)
            {
                errorMessage = "Error processing server response. Please try again.";
            }
            
            ShowError(errorMessage);
        }
        finally
        {
            SetLoadingState(false);
        }
    }
    
    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage(_authService));
    }
    
    private async void OnSkipClicked(object sender, EventArgs e)
    {
        await GoToMainPage();
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
        LoginButton.IsEnabled = !isLoading;
        RegisterButton.IsEnabled = !isLoading;
        SkipButton.IsEnabled = !isLoading;
        UsernameEntry.IsEnabled = !isLoading;
        PasswordEntry.IsEnabled = !isLoading;
    }
    
    private async Task GoToMainPage()
    {
        try
        {
            // Check if we're inside a modal navigation context
            if (Navigation.ModalStack.Count > 0)
            {
                // Close the modal first
                await Navigation.PopModalAsync();
            }
            else if (Navigation.NavigationStack.Count > 1)
            {
                // If we're in a regular navigation stack, go back to root
                await Navigation.PopToRootAsync();
            }
            
            // Then navigate to the main page
            await Shell.Current.GoToAsync("//MainPage");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Navigation error: {ex.Message}");
            
            // Fallback navigation if the standard approach fails
            if (Application.Current?.MainPage != null)
            {
                Application.Current.MainPage = new AppShell();
            }
        }
    }
} 