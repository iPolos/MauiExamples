using System.Collections.ObjectModel;
using System.Windows.Input;
using MauiExamples.Examples.VanillaMvvm.Views;
using MauiExamples.Models;
using MauiExamples.Services;

namespace MauiExamples.Examples.VanillaMvvm.ViewModels;

public class ProductsViewModel : BaseViewModel
{
    private readonly ProductService _productService;
    private ObservableCollection<Product> _products;
    private Product _selectedProduct;

    public ObservableCollection<Product> Products
    {
        get => _products;
        set => SetProperty(ref _products, value);
    }

    public Product SelectedProduct
    {
        get => _selectedProduct;
        set
        {
            if (SetProperty(ref _selectedProduct, value) && value != null)
            {
                SelectProductCommand.Execute(value);
                // Reset selected item
                SelectedProduct = null;
            }
        }
    }

    public ICommand LoadProductsCommand { get; }
    public ICommand SelectProductCommand { get; }

    public ProductsViewModel(ProductService productService)
    {
        _productService = productService;
        Title = "Products";
        Products = new ObservableCollection<Product>();
        
        LoadProductsCommand = new Command(LoadProducts);
        SelectProductCommand = new Command<Product>(SelectProduct);
    }

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

    private async void SelectProduct(Product product)
    {
        if (product == null)
            return;

        await Shell.Current.Navigation.PushAsync(new ProductDetailPage(product));
    }
} 