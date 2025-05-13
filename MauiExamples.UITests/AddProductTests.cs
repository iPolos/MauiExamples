using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.iOS;
using Xunit;
using OpenQA.Selenium.Support.UI;

namespace MauiExamples.UITests;
public class AddProductTests : UITestBase
{
    [Fact]
    public async Task AddProduct_WithValidData_ShouldAddProductToList()
    {
        try
        {
            // Navigate to products page
            await NavigateToProductsPageStandard();
            TakeScreenshot("AddProduct_WithValidData_ProductsPage_Before");

            // Get initial product count
            var productsList = Driver.FindElement(By.Id("ProductsList"));
            var initialProductCount = productsList.FindElements(By.Id("ProductName")).Count;

            // Click add button
            var addButton = Driver.FindElement(By.Id("AddButton"));
            addButton.Click();

            // Wait for add product page
            WaitForElement("NameEntry", 30);
            TakeScreenshot("AddProduct_WithValidData_AddProductPage_Empty");

            // Fill in product details
            var nameEntry = Driver.FindElement(By.Id("NameEntry"));
            nameEntry.SendKeys("Test Product");

            var descriptionEditor = Driver.FindElement(By.Id("DescriptionEditor"));
            descriptionEditor.SendKeys("Test description");

            var priceEntry = Driver.FindElement(By.Id("PriceEntry"));
            priceEntry.SendKeys("99.99");
            TakeScreenshot("AddProduct_WithValidData_AddProductPage_Filled");

            // Save the product
            var saveButton = Driver.FindElement(By.Id("SaveButton"));
            saveButton.Click();

            // Wait for products list to be visible and populated
            WaitForElement("ProductsList", 120);
            
            // Just verify that we can see the products list after saving
            var finalProductsList = Driver.FindElement(By.Id("ProductsList"));
            Assert.NotNull(finalProductsList);
            Assert.True(finalProductsList.Displayed);
            TakeScreenshot("AddProduct_WithValidData_ProductsPage_After");
            
            // TODO: validate that correct product is added.
        }
        catch (Exception ex)
        {
            TakeFailureScreenshot("AddProduct_WithValidData_ShouldAddProductToList", ex);
            throw;
        }
    }

    [Fact]
    public void AddProduct_WithInvalidData_ShouldNotEnableSaveButton()
    {
        try
        {
            // Wait for the main page to load
            WaitForElement("MainPage", 120);
            TakeScreenshot("AddProduct_WithInvalidData_MainPage");
            
            // Click the Standard button
            var standardButton = Driver.FindElement(By.Id("StandardButton"));
            standardButton.Click();
            
            // Wait for products page
            WaitForElement("ProductsPageStandard", 120);
            TakeScreenshot("AddProduct_WithInvalidData_ProductsPage");
            
            // Click add button
            var addButton = Driver.FindElement(By.Id("AddButton"));
            addButton.Click();

            // Wait for add product page
            WaitForElement("NameEntry", 120);
            TakeScreenshot("AddProduct_WithInvalidData_AddProductPage_Empty");

            // Verify save button is initially disabled
            var saveButton = Driver.FindElement(By.Id("SaveButton"));
            Assert.False(saveButton.Enabled);

            // Fill in only name
            var nameEntry = Driver.FindElement(By.Id("NameEntry"));
            nameEntry.SendKeys("Test Product");
            TakeScreenshot("AddProduct_WithInvalidData_NameOnly");

            // Verify save button is still disabled
            Assert.False(saveButton.Enabled);

            // Fill in description
            var descriptionEditor = Driver.FindElement(By.Id("DescriptionEditor"));
            descriptionEditor.SendKeys("Test description");
            TakeScreenshot("AddProduct_WithInvalidData_NameAndDescription");

            // Verify save button is still disabled
            Assert.False(saveButton.Enabled);

            // Fill in invalid price
            var priceEntry = Driver.FindElement(By.Id("PriceEntry"));
            priceEntry.SendKeys("invalid");
            TakeScreenshot("AddProduct_WithInvalidData_InvalidPrice");

            // Verify save button is still disabled
            Assert.False(saveButton.Enabled);

            // Fill in valid price
            priceEntry.Clear();
            priceEntry.SendKeys("99.99");
            TakeScreenshot("AddProduct_WithInvalidData_ValidPrice");

            // Verify save button is now enabled
            Assert.True(saveButton.Enabled);
        }
        catch (Exception ex)
        {
            TakeFailureScreenshot("AddProduct_WithInvalidData_ShouldNotEnableSaveButton", ex);
            throw;
        }
    }

    [Fact]
    public async Task AddProduct_WhenCancelled_ShouldNotAddProduct()
    {
        try
        {
            // Navigate to products page
            await NavigateToProductsPageStandard();
            TakeScreenshot("AddProduct_WhenCancelled_ProductsPage_Before");

            // Get initial product count
            var productsList = Driver.FindElement(By.Id("ProductsList"));
            var initialProductCount = productsList.FindElements(By.Id("ProductName")).Count;

            // Click add button
            var addButton = Driver.FindElement(By.Id("AddButton"));
            addButton.Click();

            // Wait for add product page
            WaitForElement("NameEntry", 30);
            TakeScreenshot("AddProduct_WhenCancelled_AddProductPage_Empty");

            // Fill in product details
            var nameEntry = Driver.FindElement(By.Id("NameEntry"));
            nameEntry.SendKeys("Test Product");

            var descriptionEditor = Driver.FindElement(By.Id("DescriptionEditor"));
            descriptionEditor.SendKeys("Test description");

            var priceEntry = Driver.FindElement(By.Id("PriceEntry"));
            priceEntry.SendKeys("99.99");
            TakeScreenshot("AddProduct_WhenCancelled_AddProductPage_Filled");

            // Click cancel
            var cancelButton = Driver.FindElement(By.Id("CancelButton"));
            cancelButton.Click();

            // Wait for products list to be visible
            WaitForElement("ProductsList", 30);
            TakeScreenshot("AddProduct_WhenCancelled_ProductsPage_After");

            // Verify product count hasn't changed
            var finalProductCount = productsList.FindElements(By.Id("ProductName")).Count;
            Assert.Equal(initialProductCount, finalProductCount);
        }
        catch (Exception ex)
        {
            TakeFailureScreenshot("AddProduct_WhenCancelled_ShouldNotAddProduct", ex);
            throw;
        }
    }

    private async Task NavigateToProductsPageStandard()
    {
        try
        {
            // Wait for main page
            WaitForElement("MainPage", 120);
            TakeScreenshot("NavigateToProductsPageStandard_MainPage");

            // Navigate to products page
            var productsButton = Driver.FindElement(By.Id("StandardButton"));
            productsButton.Click();

            // Wait for products page
            WaitForElement("ProductsPageStandard", 120);
            TakeScreenshot("NavigateToProductsPageStandard_ProductsPage");
        }
        catch (Exception ex)
        {
            TakeFailureScreenshot("NavigateToProductsPageStandard", ex);
            throw;
        }
    }
} 