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
    public ICommand DeleteProductCommand { get; }

    public ProductsViewModel(IProductService productService, IServiceProvider serviceProvider, bool loadOnInit = true)
    {
        _productService = productService;
        _serviceProvider = serviceProvider;
        Title = "Products";
        Products = new ObservableCollection<Product>();
        
        LoadProductsCommand = new Command(LoadProducts);
        SelectProductCommand = new Command<Product>(SelectProduct);
        AddProductCommand = new Command(AddProduct);
        DeleteProductCommand = new Command<Product>(DeleteProduct);
        
        // Load products immediately when ViewModel is created
        if (loadOnInit)
            LoadProducts();
    }

    public void LoadProducts()
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
    
    private async void DeleteProduct(Product product)
    {
        if (product == null)
            return;

        // Ask for confirmation
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