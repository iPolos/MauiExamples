using MauiExamples.Examples.VanillaMvvm.ViewModels;
using MauiExamples.Services;

namespace MauiExamples.Examples.VanillaMvvm.Views;

public partial class ProductsPage : ContentPage
{
    public ProductsPage(ProductService productService)
    {
        InitializeComponent();
        BindingContext = new ProductsViewModel(productService);
    }
} 