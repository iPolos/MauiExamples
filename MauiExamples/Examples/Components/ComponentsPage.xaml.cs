using System.Collections.ObjectModel;

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
            }
            // Add more components here as they are created
        };
        
        ComponentsCollection.ItemsSource = _components;
    }

    private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is ComponentItem selectedItem)
        {
            // Clear selection
            ComponentsCollection.SelectedItem = null;
            
            // Get the page from the service provider
            var pageType = selectedItem.PageType;
            var page = _serviceProvider.GetService(pageType) as Page;
            
            if (page != null)
            {
                await Shell.Current.Navigation.PushAsync(page);
            }
        }
    }
}

public class ComponentItem
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Type PageType { get; set; }
} 