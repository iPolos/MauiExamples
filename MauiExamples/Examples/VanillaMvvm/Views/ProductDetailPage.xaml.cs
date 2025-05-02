using MauiExamples.Examples.VanillaMvvm.ViewModels;
using MauiExamples.Models;

namespace MauiExamples.Examples.VanillaMvvm.Views;

public partial class ProductDetailPage : ContentPage
{
    public ProductDetailPage(Product product)
    {
        InitializeComponent();
        BindingContext = new ProductDetailViewModel(product);
    }
} 