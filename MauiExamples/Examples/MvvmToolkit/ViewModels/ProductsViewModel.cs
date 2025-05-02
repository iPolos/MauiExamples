using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiExamples.Examples.MvvmToolkit.Views;
using MauiExamples.Models;
using MauiExamples.Services;

namespace MauiExamples.Examples.MvvmToolkit.ViewModels;

public partial class ProductsViewModel : ObservableObject
{
    private readonly ProductService _productService;

    [ObservableProperty]
    private string _title = "Products";

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private ObservableCollection<Product> _products = new();

    [ObservableProperty]
    private Product _selectedProduct;

    public ProductsViewModel(ProductService productService)
    {
        _productService = productService;
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

        await Shell.Current.Navigation.PushAsync(new ProductDetailPage(product));
    }
} 