using MauiExamples.Examples.Standard;
using MauiExamples.Services;

namespace MauiExamples;

public partial class MainPage : ContentPage
{
    private readonly ProductService _productService;

    public MainPage(ProductService productService)
    {
        InitializeComponent();
        _productService = productService;
    }

    private async void OnStandardButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Examples.Standard.ProductsPage(_productService));
    }

    private async void OnVanillaMvvmButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Examples.VanillaMvvm.Views.ProductsPage(_productService));
    }

    private async void OnMvvmToolkitButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Examples.MvvmToolkit.Views.ProductsPage(_productService));
    }
}