using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using MauiExamples.Services;
using MauiExamples.Views;

namespace MauiExamples;

public partial class MainPage : ContentPage
{
    private readonly IServiceProvider _serviceProvider;
    private readonly AuthService _authService;

    public MainPage(IServiceProvider serviceProvider, AuthService authService)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
        _authService = authService;
        
        // Subscribe to authentication changes
        _authService.AuthenticationChanged += OnAuthenticationChanged;
        
        // Update the UI based on current auth state
        UpdateAuthStatusUI();
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateAuthStatusUI();
    }

    private void OnAuthenticationChanged(object sender, EventArgs e)
    {
        // Update the UI when auth state changes
        UpdateAuthStatusUI();
    }
    
    private void UpdateAuthStatusUI()
    {
        if (_authService.IsAuthenticated)
        {
            AuthStatusLabel.Text = $"Logged in as: {_authService.CurrentUsername}";
            LoginButton.IsVisible = false;
            LogoutButton.IsVisible = true;
        }
        else
        {
            AuthStatusLabel.Text = "Not logged in";
            LoginButton.IsVisible = true;
            LogoutButton.IsVisible = false;
        }
    }
    
    private async void OnLoginButtonClicked(object sender, EventArgs e)
    {
        var loginPage = _serviceProvider.GetRequiredService<LoginPage>();
        await Navigation.PushModalAsync(new NavigationPage(loginPage));
    }
    
    private async void OnLogoutButtonClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Logout", "Are you sure you want to log out?", "Yes", "No");
        
        if (confirm)
        {
            await _authService.LogoutAsync();
            // UI will be updated via the AuthenticationChanged event
            
            // Optionally show a confirmation
            await DisplayAlert("Logged Out", "You have been logged out successfully.", "OK");
        }
    }

    private async void OnStandardButtonClicked(object sender, EventArgs e)
    {
        var page = _serviceProvider.GetRequiredService<Examples.Standard.ProductsPage>();
        await Shell.Current.Navigation.PushAsync(page);
    }

    private async void OnVanillaMvvmButtonClicked(object sender, EventArgs e)
    {
        var page = _serviceProvider.GetRequiredService<Examples.VanillaMvvm.Views.ProductsPage>();
        await Shell.Current.Navigation.PushAsync(page);
    }

    private async void OnMvvmToolkitButtonClicked(object sender, EventArgs e)
    {
        var page = _serviceProvider.GetRequiredService<Examples.MvvmToolkit.Views.ProductsPage>();
        await Shell.Current.Navigation.PushAsync(page);
    }

    private async void OnComponentsButtonClicked(object sender, EventArgs e)
    {
        var page = _serviceProvider.GetRequiredService<Examples.Components.ComponentsPage>();
        await Shell.Current.Navigation.PushAsync(page);
    }
}