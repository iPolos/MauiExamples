using MauiExamples.Examples.MvvmToolkit.ViewModels;
using MauiExamples.Models;

namespace MauiExamples.Examples.MvvmToolkit.Views;

public partial class ProductDetailPage : ContentPage
{
    public ProductDetailPage(Product product)
    {
        InitializeComponent();
        BindingContext = new ProductDetailViewModel(product);
    }
} 