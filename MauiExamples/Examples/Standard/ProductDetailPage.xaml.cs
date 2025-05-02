using MauiExamples.Models;

namespace MauiExamples.Examples.Standard;

public partial class ProductDetailPage : ContentPage
{
    private readonly Product _product;
    
    public ProductDetailPage(Product product)
    {
        InitializeComponent();
        _product = product;
        
        // Load product details
        LoadProductDetails();
    }

    private void LoadProductDetails()
    {
        // Set the product details to the UI elements
        ProductImage.Source = _product.ImageUrl;
        NameLabel.Text = _product.Name;
        PriceLabel.Text = $"${_product.Price:F2}";
        StockLabel.Text = _product.InStock ? "Yes" : "No";
        DescriptionLabel.Text = _product.Description;
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.Navigation.PopAsync();
    }
} 