using System.Collections.ObjectModel;
using MauiExamples.Examples.VanillaMvvm.ViewModels;
using MauiExamples.Models;
using MauiExamples.Services;
using Moq;
using Xunit;

namespace MauiExamples.Tests;

public class ProductsViewModelTests
{
    [Fact]
    public void LoadProducts_ShouldPopulateProductsCollection()
    {
        // Arrange
        var mockProductService = new Mock<IProductService>();
        var mockServiceProvider = new Mock<IServiceProvider>();
        
        var testProducts = new List<Product>
        {
            new Product { Id = 1, Name = "Test Product 1", Price = 99.99m },
            new Product { Id = 2, Name = "Test Product 2", Price = 149.99m }
        };
        
        mockProductService.Setup(x => x.GetAllProducts())
            .Returns(testProducts);
            
        var viewModel = new ProductsViewModel(mockProductService.Object, mockServiceProvider.Object);
        
        // Act
        viewModel.LoadProducts();
        
        // Assert
        Assert.NotNull(viewModel.Products);
        Assert.Equal(2, viewModel.Products.Count);
        Assert.Equal("Test Product 1", viewModel.Products[0].Name);
        Assert.Equal("Test Product 2", viewModel.Products[1].Name);
    }

    [Fact]
    public void LoadProducts_WhenBusy_ShouldNotLoadProducts()
    {
        // Arrange
        var mockProductService = new Mock<IProductService>();
        var mockServiceProvider = new Mock<IServiceProvider>();
        
        var testProducts = new List<Product>
        {
            new Product { Id = 1, Name = "Test Product 1", Price = 99.99m },
            new Product { Id = 2, Name = "Test Product 2", Price = 149.99m }
        };
        
        mockProductService.Setup(x => x.GetAllProducts())
            .Returns(testProducts);
            
        var viewModel = new ProductsViewModel(mockProductService.Object, mockServiceProvider.Object, loadOnInit: false);
        viewModel.Products = new ObservableCollection<Product>();
        viewModel.IsBusy = true;
        
        // Act
        viewModel.LoadProducts();
        
        // Assert
        Assert.NotNull(viewModel.Products);
        Assert.Equal(0, viewModel.Products.Count);
    }
} 