using System.Collections.ObjectModel;
using MauiExamples.Examples.VanillaMvvm.ViewModels;
using MauiExamples.Models;
using MauiExamples.Services;
using Moq;
using Xunit;

namespace MauiExamples.Tests;

public class ProductsViewModelTests
{
    [Theory]
    [InlineData(1, "Test Product 1", 99.99)]
    [InlineData(2, "Test Product 2", 149.99)]
    [InlineData(3, "Test Product 3", 199.99)]
    [InlineData(4, "Premium Headphones", 299.99)]
    [InlineData(5, "Wireless Mouse", 49.99)]
    [InlineData(6, "Mechanical Keyboard", 129.99)]
    [InlineData(7, "Gaming Monitor", 399.99)]
    [InlineData(8, "USB-C Hub", 79.99)]
    public void LoadProducts_ShouldPopulateProductsCollection(int id, string name, decimal price)
    {
        // Arrange
        var mockProductService = new Mock<IProductService>();
        var mockServiceProvider = new Mock<IServiceProvider>();
        
        var testProducts = new List<Product>
        {
            new Product { Id = id, Name = name, Price = price }
        };
        
        mockProductService.Setup(x => x.GetAllProducts())
            .Returns(testProducts);
            
        var viewModel = new ProductsViewModel(mockProductService.Object, mockServiceProvider.Object);
        
        // Act
        viewModel.LoadProducts();
        
        // Assert
        Assert.NotNull(viewModel.Products);
        Assert.Single(viewModel.Products);
        Assert.Equal(name, viewModel.Products[0].Name);
        Assert.Equal(price, viewModel.Products[0].Price);
    }

    [Theory]
    [InlineData(true, 0)]  // When busy, should not load products
    [InlineData(false, 2)] // When not busy, should load products
    [InlineData(true, 0, true)]  // When busy and loadOnInit is true
    [InlineData(false, 2, true)] // When not busy and loadOnInit is true
    [InlineData(true, 0, false)] // When busy and loadOnInit is false
    [InlineData(false, 2, false)] // When not busy and loadOnInit is false
    [InlineData(true, 0, false, 3)] // When busy, loadOnInit is false, and products count is 3
    public void LoadProducts_WhenBusy_ShouldNotLoadProducts(bool isBusy, int expectedCount, bool loadOnInit = false, int productCount = 2)
    {
        // Arrange
        var mockProductService = new Mock<IProductService>();
        var mockServiceProvider = new Mock<IServiceProvider>();
        
        var testProducts = new List<Product>();
        for (int i = 1; i <= productCount; i++)
        {
            testProducts.Add(new Product 
            { 
                Id = i, 
                Name = $"Test Product {i}", 
                Price = 99.99m * i 
            });
        }
        
        mockProductService.Setup(x => x.GetAllProducts())
            .Returns(testProducts);
            
        var viewModel = new ProductsViewModel(mockProductService.Object, mockServiceProvider.Object, loadOnInit);
        viewModel.Products = new ObservableCollection<Product>();
        viewModel.IsBusy = isBusy;
        
        // Act
        viewModel.LoadProducts();
        
        // Assert
        Assert.NotNull(viewModel.Products);
        Assert.Equal(expectedCount, viewModel.Products.Count);
    }
} 