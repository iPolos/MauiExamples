namespace MauiExamples;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Use AppShell directly without wrapping it in a NavigationPage
        MainPage = new AppShell();
    }
}