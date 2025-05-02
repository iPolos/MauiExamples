using MauiExamples.Examples.VanillaMvvm.ViewModels;

namespace MauiExamples.Examples.VanillaMvvm.Views;

public partial class AddProductPage : ContentPage
{
    public AddProductPage(AddProductViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
} 