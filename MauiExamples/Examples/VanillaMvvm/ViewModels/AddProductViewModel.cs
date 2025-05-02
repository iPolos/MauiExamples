using System.Windows.Input;
using MauiExamples.Models;
using MauiExamples.Services;

namespace MauiExamples.Examples.VanillaMvvm.ViewModels;

public class AddProductViewModel : BaseViewModel
{
    private readonly IProductService _productService;
    
    private string _name = string.Empty;
    private string _description = string.Empty;
    private decimal _price;
    private string _imageUrl = "dotnet_bot.png";
    private bool _inStock = true;
    
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }
    
    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }
    
    public decimal Price
    {
        get => _price;
        set => SetProperty(ref _price, value);
    }

    public string ImageUrl
    {
        get => _imageUrl;
        set => SetProperty(ref _imageUrl, value);
    }
    
    public bool InStock
    {
        get => _inStock;
        set => SetProperty(ref _inStock, value);
    }
    
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    
    public AddProductViewModel(IProductService productService)
    {
        _productService = productService;
        Title = "Add Product";
        
        SaveCommand = new Command(SaveProduct);
        CancelCommand = new Command(Cancel);
    }
    
    private async void SaveProduct()
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
    
    private async void Cancel()
    {
        await Shell.Current.Navigation.PopAsync();
    }
} 