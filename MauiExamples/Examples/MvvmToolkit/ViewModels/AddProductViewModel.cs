using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiExamples.Models;
using MauiExamples.Services;

namespace MauiExamples.Examples.MvvmToolkit.ViewModels;

public partial class AddProductViewModel : ObservableObject
{
    private readonly IProductService _productService;

    [ObservableProperty]
    private string _title = "Add Product";

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private decimal _price;

    [ObservableProperty]
    private string _imageUrl = "dotnet_bot.png";

    [ObservableProperty]
    private bool _inStock = true;

    public AddProductViewModel(IProductService productService)
    {
        _productService = productService;
    }

    [RelayCommand]
    private async Task SaveProduct()
    {
        var product = new Product
        {
            Name = Name,
            Description = Description,
            Price = Price,
            ImageUrl = ImageUrl,
            InStock = InStock
        };

        _productService.AddProduct(product);
        await Shell.Current.Navigation.PopAsync();
    }

    [RelayCommand]
    private async Task Cancel()
    {
        await Shell.Current.Navigation.PopAsync();
    }
} 