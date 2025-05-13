using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.iOS;
using Xunit;

namespace MauiExamples.UITests
{
    // Removed IClassFixture<TestFixture> as it's unnecessary:
    // - Exception handling is implemented directly in each test method with try/catch blocks
    // - Screenshots are taken at key points and on failures through UITestBase methods
    // - No shared test context is needed between tests
    public class MainPageTests : UITestBase
    {
        [Fact]
        public void MainPage_LoadsSuccessfully()
        {
            try
            {
                // Wait for the main page to load
                var mainPage = Driver?.FindElement(By.Id("MainPage"));
                Assert.NotNull(mainPage);
                Assert.True(mainPage?.Displayed);

                // Verify the page title
                var pageTitle = Driver?.FindElement(By.Id("PageTitle"));
                Assert.NotNull(pageTitle);
                Assert.True(pageTitle?.Displayed);
                Assert.Equal("MVVM Examples", pageTitle?.Text);
                
                // Take screenshot of main page
                TakeScreenshot("MainPage_LoadsSuccessfully");
            }
            catch (Exception ex)
            {
                TakeFailureScreenshot("MainPage_LoadsSuccessfully", ex);
                throw;
            }
        }

        [Fact]
        public void MainPage_ButtonsAreVisible()
        {
            try
            {
                // Verify all three buttons are visible
                var standardButton = Driver?.FindElement(By.Id("StandardButton"));
                var vanillaMvvmButton = Driver?.FindElement(By.Id("VanillaMvvmButton"));
                var mvvmToolkitButton = Driver?.FindElement(By.Id("MvvmToolkitButton"));

                Assert.NotNull(standardButton);
                Assert.NotNull(vanillaMvvmButton);
                Assert.NotNull(mvvmToolkitButton);

                Assert.True(standardButton?.Displayed);
                Assert.True(vanillaMvvmButton?.Displayed);
                Assert.True(mvvmToolkitButton?.Displayed);
                
                // Take screenshot showing buttons
                TakeScreenshot("MainPage_ButtonsAreVisible");
            }
            catch (Exception ex)
            {
                TakeFailureScreenshot("MainPage_ButtonsAreVisible", ex);
                throw;
            }
        }

        [Fact]
        public void MainPage_ImageIsVisible()
        {
            try
            {
                WaitForElement("MainPage");
                var mainImage = Driver?.FindElement(By.Id("MainImage"));
                Assert.NotNull(mainImage);
                Assert.True(mainImage?.Displayed);
                
                // Take screenshot showing image
                TakeScreenshot("MainPage_ImageIsVisible");
            }
            catch (Exception ex)
            {
                TakeFailureScreenshot("MainPage_ImageIsVisible", ex);
                throw;
            }
        }

        [Fact]
        public void MainPage_SubtitleIsVisible()
        {
            try
            {
                WaitForElement("MainPage");
                var subtitle = Driver?.FindElement(By.Id("PageSubtitle"));
                Assert.NotNull(subtitle);
                Assert.True(subtitle?.Displayed);
                Assert.Equal("Different implementations of MVVM in .NET MAUI", subtitle?.Text);
                
                // Take screenshot showing subtitle
                TakeScreenshot("MainPage_SubtitleIsVisible");
            }
            catch (Exception ex)
            {
                TakeFailureScreenshot("MainPage_SubtitleIsVisible", ex);
                throw;
            }
        }

        [Fact]
        public void MainPage_NavigateToProducts_Standard()
        {
            try
            {
                WaitForElement("MainPage", 120);
                // Take screenshot of main page before navigation
                TakeScreenshot("MainPage_NavigateToProducts_Standard_Before");
                
                // Click the Standard button
                var standardButton = Driver?.FindElement(By.Id("StandardButton"));
                Assert.NotNull(standardButton);
                standardButton?.Click();

                // Wait for the products page to load
                WaitForElement("ProductsPageStandard", 120);
                var productsPage = Driver?.FindElement(By.Id("ProductsPageStandard"));
                Assert.NotNull(productsPage);
                Assert.True(productsPage?.Displayed);
                
                // Take screenshot after navigation
                TakeScreenshot("MainPage_NavigateToProducts_Standard_After");
            }
            catch (Exception ex)
            {
                TakeFailureScreenshot("MainPage_NavigateToProducts_Standard", ex);
                throw;
            }
        }

        [Fact]
        public void MainPage_NavigateToProducts_VanillaMvvm()
        {
            try
            {
                WaitForElement("MainPage");
                // Take screenshot of main page before navigation
                TakeScreenshot("MainPage_NavigateToProducts_VanillaMvvm_Before");
                
                // Click the Vanilla MVVM button
                var vanillaMvvmButton = Driver?.FindElement(By.Id("VanillaMvvmButton"));
                Assert.NotNull(vanillaMvvmButton);
                vanillaMvvmButton?.Click();

                // Wait for the products page to load
                WaitForElement("ProductsPageVanillaMvvm", 90);
                var productsPage = Driver?.FindElement(By.Id("ProductsPageVanillaMvvm"));
                Assert.NotNull(productsPage);
                Assert.True(productsPage?.Displayed);
                
                // Take screenshot after navigation
                TakeScreenshot("MainPage_NavigateToProducts_VanillaMvvm_After");
            }
            catch (Exception ex)
            {
                TakeFailureScreenshot("MainPage_NavigateToProducts_VanillaMvvm", ex);
                throw;
            }
        }

        [Fact]
        public void MainPage_NavigateToProducts_MvvmToolkit()
        {
            try
            {
                WaitForElement("MainPage");
                // Take screenshot of main page before navigation
                TakeScreenshot("MainPage_NavigateToProducts_MvvmToolkit_Before");
                
                // Click the MVVM Toolkit button
                var mvvmToolkitButton = Driver?.FindElement(By.Id("MvvmToolkitButton"));
                Assert.NotNull(mvvmToolkitButton);
                mvvmToolkitButton?.Click();

                // Wait for the products page to load
                WaitForElement("ProductsPageMvvmToolkit");
                var productsPage = Driver?.FindElement(By.Id("ProductsPageMvvmToolkit"));
                Assert.NotNull(productsPage);
                Assert.True(productsPage?.Displayed);
                
                // Take screenshot after navigation
                TakeScreenshot("MainPage_NavigateToProducts_MvvmToolkit_After");
            }
            catch (Exception ex)
            {
                TakeFailureScreenshot("MainPage_NavigateToProducts_MvvmToolkit", ex);
                throw;
            }
        }
    }
} 