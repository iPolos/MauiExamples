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
} 