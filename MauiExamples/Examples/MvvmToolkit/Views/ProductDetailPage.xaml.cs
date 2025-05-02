using MauiExamples.Examples.MvvmToolkit.ViewModels;
using MauiExamples.Models;

namespace MauiExamples.Examples.MvvmToolkit.Views;

public partial class ProductDetailPage : ContentPage
{
    public ProductDetailPage(ProductDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
    
    // Factory method pattern for creating with product
    public static ProductDetailPage CreateWithProduct(IServiceProvider serviceProvider, Product product)
    {
        // Since our ProductDetailViewModel constructor requires a product
        // we need to create a new instance rather than resolving from DI
        var detailViewModel = new ProductDetailViewModel(product);
        return new ProductDetailPage(detailViewModel);
    }
} 