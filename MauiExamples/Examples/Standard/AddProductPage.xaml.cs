using MauiExamples.Models;
using MauiExamples.Services;

namespace MauiExamples.Examples.Standard;

public partial class AddProductPage : ContentPage
{
    private readonly IProductService _productService;
    
    public AddProductPage(IProductService productService)
    {
        InitializeComponent();
        _productService = productService;
    }

    private void OnEntryTextChanged(object sender, TextChangedEventArgs e)
    {
        // Enable the Save button only if all required fields have values
        SaveButton.IsEnabled = !string.IsNullOrWhiteSpace(NameEntry.Text) &&
                               !string.IsNullOrWhiteSpace(DescriptionEditor.Text) &&
                               decimal.TryParse(PriceEntry.Text, out decimal price) &&
                               price > 0;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        // Create new product from input fields
        var product = new Product
        {
            Name = NameEntry.Text,
            Description = DescriptionEditor.Text,
            Price = decimal.Parse(PriceEntry.Text),
            InStock = InStockSwitch.IsToggled,
            ImageUrl = ImageUrlEntry.Text
        };
        
        // Add to service
        _productService.AddProduct(product);
        
        // Return to previous page
        await Shell.Current.Navigation.PopAsync();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Shell.Current.Navigation.PopAsync();
    }
} 