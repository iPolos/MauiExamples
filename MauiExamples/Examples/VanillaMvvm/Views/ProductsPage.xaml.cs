using MauiExamples.Examples.VanillaMvvm.ViewModels;
using MauiExamples.Services;

namespace MauiExamples.Examples.VanillaMvvm.Views;

public partial class ProductsPage : ContentPage
{
    private readonly ProductsViewModel _viewModel;
    
    public ProductsPage(ProductService productService)
    {
        InitializeComponent();
        _viewModel = new ProductsViewModel(productService);
        BindingContext = _viewModel;
        
        // Manually hook up appearing event as a fallback
        this.Appearing += OnPageAppearing;
    }
    
    private void OnPageAppearing(object sender, EventArgs e)
    {
        // Execute the load command if products are not loaded
        if (_viewModel.Products.Count == 0)
        {
            _viewModel.LoadProductsCommand.Execute(null);
        }
    }
} 