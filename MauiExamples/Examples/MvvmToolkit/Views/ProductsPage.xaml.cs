using MauiExamples.Examples.MvvmToolkit.ViewModels;
using MauiExamples.Services;

namespace MauiExamples.Examples.MvvmToolkit.Views;

public partial class ProductsPage : ContentPage
{
    public ProductsPage(ProductService productService)
    {
        InitializeComponent();
        BindingContext = new ProductsViewModel(productService);
    }
} 