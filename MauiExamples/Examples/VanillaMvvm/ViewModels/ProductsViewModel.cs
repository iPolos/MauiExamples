using System.Collections.ObjectModel;
using System.Windows.Input;
using MauiExamples.Examples.VanillaMvvm.Views;
using MauiExamples.Models;
using MauiExamples.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MauiExamples.Examples.VanillaMvvm.ViewModels;

public class ProductsViewModel : BaseViewModel
{
    private readonly IProductService _productService;
    private readonly IServiceProvider _serviceProvider;
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
    public ICommand AddProductCommand { get; }

    public ProductsViewModel(IProductService productService, IServiceProvider serviceProvider)
    {
        _productService = productService;
        _serviceProvider = serviceProvider;
        Title = "Products";
        Products = new ObservableCollection<Product>();
        
        LoadProductsCommand = new Command(LoadProducts);
        SelectProductCommand = new Command<Product>(SelectProduct);
        AddProductCommand = new Command(AddProduct);
        
        // Load products immediately when ViewModel is created
        LoadProducts();
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

        // Use the factory method to create the detail page
        var detailPage = ProductDetailPage.CreateWithProduct(_serviceProvider, product);
        await Shell.Current.Navigation.PushAsync(detailPage);
    }
    
    private async void AddProduct()
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