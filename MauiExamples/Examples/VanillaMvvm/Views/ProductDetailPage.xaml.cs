using MauiExamples.Examples.VanillaMvvm.ViewModels;
using MauiExamples.Models;

namespace MauiExamples.Examples.VanillaMvvm.Views;

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
        var viewModel = serviceProvider.GetService<ProductDetailViewModel>();
        if (viewModel != null)
        {
            viewModel.Product = product;
            return new ProductDetailPage(viewModel);
        }
        
        throw new InvalidOperationException("Could not resolve ProductDetailViewModel from service provider");
    }
} 