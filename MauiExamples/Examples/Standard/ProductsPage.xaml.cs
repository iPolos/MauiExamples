using MauiExamples.Models;
using MauiExamples.Services;

namespace MauiExamples.Examples.Standard;

public partial class ProductsPage : ContentPage
{
    private readonly ProductService _productService;
    
    public ProductsPage(ProductService productService)
    {
        InitializeComponent();
        _productService = productService;
        
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
            await Navigation.PushAsync(new ProductDetailPage(selectedProduct));
        }
    }
} 