using Microsoft.Extensions.DependencyInjection;
using MauiExamples.Models;
using MauiExamples.Services;

namespace MauiExamples.Examples.Standard;

public partial class ProductsPage : ContentPage
{
    private readonly IProductService _productService;
    private readonly IServiceProvider _serviceProvider;
    
    public ProductsPage(IProductService productService, IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _productService = productService;
        _serviceProvider = serviceProvider;
        
        // Load products when page appears
        Loaded += OnPageLoaded;
    }

    private void OnPageLoaded(object sender, EventArgs e)
    {
        ProductsCollectionView.ItemsSource = _productService.GetAllProducts();
    }

    private async void OnProductSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Product selectedProduct)
        {
            // Clear selection
            ProductsCollectionView.SelectedItem = null;
            
            // Navigate to product details page
            // For standard implementation, we don't use DI for the detail page
            // since it's a simple pass-through of data
            await Shell.Current.Navigation.PushAsync(new ProductDetailPage(selectedProduct));
        }
    }
} 