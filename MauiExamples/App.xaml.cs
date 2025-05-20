using MauiExamples.Views;
using MauiExamples.Services;

namespace MauiExamples;

public partial class App : Application
{
    private readonly AuthService _authService;
    
    public App(AuthService authService)
    {
        InitializeComponent();
        _authService = authService;

        // Set startup page based on authentication status
        if (_authService.IsAuthenticated)
        {
            // User is already authenticated, go directly to main app
            MainPage = new AppShell();
        }
        else
        {
            // User needs to log in first
            MainPage = new NavigationPage(new LoginPage(_authService));
        }
    }
}