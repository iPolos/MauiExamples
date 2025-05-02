using MauiExamples.Examples.MvvmToolkit.ViewModels;

namespace MauiExamples.Examples.MvvmToolkit.Views;

public partial class AddProductPage : ContentPage
{
    public AddProductPage(AddProductViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
} 