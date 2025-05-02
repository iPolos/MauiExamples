using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiExamples.Models;

namespace MauiExamples.Examples.MvvmToolkit.ViewModels;

public partial class ProductDetailViewModel : ObservableObject
{
    [ObservableProperty]
    private string _title = "Product Details";

    [ObservableProperty]
    private Product _product;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _description;

    [ObservableProperty]
    private decimal _price;

    [ObservableProperty]
    private string _imageUrl;

    [ObservableProperty]
    private bool _inStock;

    public string FormattedPrice => $"${Price:F2}";

    public string StockStatus => InStock ? "Yes" : "No";

    public ProductDetailViewModel(Product product)
    {
        Product = product;
        LoadProductDetails();
    }

    private void LoadProductDetails()
    {
        if (Product == null)
            return;

        Name = Product.Name;
        Description = Product.Description;
        Price = Product.Price;
        ImageUrl = Product.ImageUrl;
        InStock = Product.InStock;
    }

    [RelayCommand]
    private async Task GoBack()
    {
        await Shell.Current.Navigation.PopAsync();
    }
} 