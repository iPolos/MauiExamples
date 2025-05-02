using System.Windows.Input;
using MauiExamples.Models;

namespace MauiExamples.Examples.VanillaMvvm.ViewModels;

public class ProductDetailViewModel : BaseViewModel
{
    private Product _product;
    private string _name;
    private string _description;
    private decimal _price;
    private string _imageUrl;
    private bool _inStock;

    public Product Product
    {
        get => _product;
        set
        {
            if (SetProperty(ref _product, value))
            {
                LoadProductDetails();
            }
        }
    }

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

    public string FormattedPrice => $"${Price:F2}";

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

    public string StockStatus => InStock ? "Yes" : "No";

    public ICommand GoBackCommand { get; }

    public ProductDetailViewModel()
    {
        Title = "Product Details";
        GoBackCommand = new Command(GoBack);
    }

    public ProductDetailViewModel(Product product) : this()
    {
        Product = product;
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

    private async void GoBack()
    {
        await Shell.Current.Navigation.PopAsync();
    }
} 