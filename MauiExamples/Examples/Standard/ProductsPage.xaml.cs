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

    private async void OnAddClicked(object sender, EventArgs e)
    {
        var addProductPage = new AddProductPage(_productService);
        
        // When returning from the AddProductPage, reload the products
        addProductPage.Disappearing += (s, args) => OnPageLoaded(s, args);
        
        await Shell.Current.Navigation.PushAsync(addProductPage);
    }
    
    private async void OnDeleteSwipeItemInvoked(object sender, EventArgs e)
    {
        // Get the SwipeItem that was invoked
        if (sender is SwipeItem swipeItem && swipeItem.BindingContext is Product product)
        {
            // Ask for confirmation
            bool confirmed = await DisplayAlert(
                "Delete Product", 
                $"Are you sure you want to delete {product.Name}?", 
                "Yes", "No");
            
            if (!confirmed)
                return;
                
            try
            {
                // Delete the product
                bool success = _productService.DeleteProduct(product.Id);
                
                if (success)
                {
                    // Show success message
                    await DisplayAlert("Success", $"{product.Name} was deleted successfully.", "OK");
                    
                    // Reload the products list
                    OnPageLoaded(this, EventArgs.Empty);
                }
                else
                {
                    // Show error message
                    await DisplayAlert("Error", $"Failed to delete {product.Name}.", "OK");
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                System.Diagnostics.Debug.WriteLine($"Error deleting product: {ex.Message}");
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }
    }
} 