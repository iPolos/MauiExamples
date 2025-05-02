using Microsoft.Extensions.DependencyInjection;
using MauiExamples.Services;

namespace MauiExamples;

public partial class MainPage : ContentPage
{
    private readonly IServiceProvider _serviceProvider;

    public MainPage(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
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
}