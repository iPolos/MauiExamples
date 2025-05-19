using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MauiExamples.Examples.Components;

public partial class ComponentsPage : ContentPage
{
    private readonly IServiceProvider _serviceProvider;
    private ObservableCollection<ComponentItem> _components;

    public ComponentsPage(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
        
        _components = new ObservableCollection<ComponentItem>
        {
            new ComponentItem
            {
                Name = "Local Notifications",
                Description = "Examples of how to use local notifications in MAUI",
                PageType = typeof(LocalNotifications.LocalNotificationPage)
            },
            new ComponentItem
            {
                Name = "Camera",
                Description = "Take photos and manage a photo gallery",
                PageType = typeof(Camera.CameraPage)
            }
            // Add more components here as they are created
        };
        
        ComponentsCollection.ItemsSource = _components;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Debug.WriteLine("ComponentsPage OnAppearing");
    }

    protected override bool OnBackButtonPressed()
    {
        Debug.WriteLine("ComponentsPage OnBackButtonPressed");
        return base.OnBackButtonPressed();
    }

    private async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is ComponentItem selectedItem)
        {
            // Clear selection (need to use MainThread to avoid issues on Android)
            MainThread.BeginInvokeOnMainThread(() => {
                ((ListView)sender).SelectedItem = null;
            });
            
            await NavigateToComponent(selectedItem);
        }
    }
    
    private async void OnComponentTapped(object sender, EventArgs e)
    {
        if (sender is Element element && element.BindingContext is ComponentItem item)
        {
            await NavigateToComponent(item);
        }
    }
    
    private async Task NavigateToComponent(ComponentItem selectedItem)
    {
        Debug.WriteLine($"Selected component: {selectedItem.Name}");
        
        try 
        {
            // Get the page from the service provider
            var pageType = selectedItem.PageType;
            var page = _serviceProvider.GetService(pageType) as Page;
            
            if (page != null)
            {
                Debug.WriteLine($"Navigating to: {page.GetType().Name}");
                await Shell.Current.Navigation.PushAsync(page);
            }
            else
            {
                Debug.WriteLine($"Failed to resolve page type: {pageType.Name}");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Navigation error: {ex.Message}");
            await DisplayAlert("Navigation Error", $"Could not navigate to {selectedItem.Name}: {ex.Message}", "OK");
        }
    }
}

public class ComponentItem
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Type PageType { get; set; }
} 