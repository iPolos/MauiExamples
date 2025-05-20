using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiExamples.Examples.MvvmToolkit.Views;
using MauiExamples.Models;
using MauiExamples.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MauiExamples.Examples.MvvmToolkit.ViewModels;

public partial class ProductsViewModel : ObservableObject
{
    private readonly IProductService _productService;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private string _title = "Products";

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private ObservableCollection<Product> _products = new();

    [ObservableProperty]
    private Product _selectedProduct;

    public ProductsViewModel(IProductService productService, IServiceProvider serviceProvider)
    {
        _productService = productService;
        _serviceProvider = serviceProvider;
        
        // Load products immediately
        LoadProducts();
    }

    [RelayCommand]
    private void LoadProducts()
    {
        if (IsBusy)
            return;

        IsBusy = true;

        try
        {
            Products.Clear();
            var products = _productService.GetAllProducts();
            foreach (var product in products)
            {
                Products.Add(product);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task DeleteProduct(Product product)
    {
        if (product == null)
            return;

        // Optional: Ask for confirmation
        bool confirmed = await Shell.Current.DisplayAlert(
            "Delete Product", 
            $"Are you sure you want to delete {product.Name}?", 
            "Yes", "No");

        if (!confirmed)
            return;

        IsBusy = true;
        try
        {
            // Call the API to delete the product
            bool success = _productService.DeleteProduct(product.Id);

            if (success)
            {
                // If successful, remove it from the observable collection
                Products.Remove(product);
                // Show success message
                await Shell.Current.DisplayAlert("Success", $"{product.Name} was deleted successfully.", "OK");
            }
            else
            {
                // Show error message
                await Shell.Current.DisplayAlert("Error", $"Failed to delete {product.Name}.", "OK");
            }
        }
        catch (Exception ex)
        {
            // Log the exception
            System.Diagnostics.Debug.WriteLine($"Error deleting product: {ex.Message}");
            await Shell.Current.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnSelectedProductChanged(Product value)
    {
        if (value != null)
        {
            GoToProductDetail(value);
            // Reset selected item
            SelectedProduct = null;
        }
    }

    [RelayCommand]
    private async Task GoToProductDetail(Product product)
    {
        if (product == null)
            return;

        // Use the factory method to create the detail page
        var detailPage = ProductDetailPage.CreateWithProduct(_serviceProvider, product);
        await Shell.Current.Navigation.PushAsync(detailPage);
    }

    [RelayCommand]
    private async Task AddProduct()
    {
        var viewModel = _serviceProvider.GetService<AddProductViewModel>();
        if (viewModel != null)
        {
            var page = new AddProductPage(viewModel);
            
            // When returning from the AddProductPage, reload the products
            page.Disappearing += (s, e) => LoadProducts();
            
            await Shell.Current.Navigation.PushAsync(page);
        }
    }
} 