namespace MauiExamples;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Create a NavigationPage with AppShell as the root
        MainPage = new NavigationPage(new AppShell());
    }
}