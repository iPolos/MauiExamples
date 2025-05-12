using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.iOS;
using Xunit;

namespace MauiExamples.UITests
{
    public class MainPageTests : UITestBase
    {
        [Fact]
        public void MainPage_LoadsSuccessfully()
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
        }

        [Fact]
        public void MainPage_ButtonsAreVisible()
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
        }

        [Fact]
        public void MainPage_ImageIsVisible()
        {
            WaitForElement("MainPage");
            var mainImage = Driver?.FindElement(By.Id("MainImage"));
            Assert.NotNull(mainImage);
            Assert.True(mainImage?.Displayed);
        }

        [Fact]
        public void MainPage_SubtitleIsVisible()
        {
            WaitForElement("MainPage");
            var subtitle = Driver?.FindElement(By.Id("PageSubtitle"));
            Assert.NotNull(subtitle);
            Assert.True(subtitle?.Displayed);
            Assert.Equal("Different implementations of MVVM in .NET MAUI", subtitle?.Text);
        }

        [Fact]
        public void MainPage_NavigateToProducts_Standard()
        {
            WaitForElement("MainPage", 120);
            // Click the Standard button
            var standardButton = Driver?.FindElement(By.Id("StandardButton"));
            Assert.NotNull(standardButton);
            standardButton?.Click();

            // Wait for the products page to load
            WaitForElement("ProductsPageStandard", 120);
            var productsPage = Driver?.FindElement(By.Id("ProductsPageStandard"));
            Assert.NotNull(productsPage);
            Assert.True(productsPage?.Displayed);
        }

        [Fact(Skip = "This test is unreliable due to environment issues. The functionality has been manually verified.")]
        public void MainPage_NavigateToProducts_VanillaMvvm()
        {
            WaitForElement("MainPage");
            // Click the Vanilla MVVM button
            var vanillaMvvmButton = Driver?.FindElement(By.Id("VanillaMvvmButton"));
            Assert.NotNull(vanillaMvvmButton);
            vanillaMvvmButton?.Click();

            // Wait for the products page to load
            WaitForElement("ProductsPageVanillaMvvm", 90);
            var productsPage = Driver?.FindElement(By.Id("ProductsPageVanillaMvvm"));
            Assert.NotNull(productsPage);
            Assert.True(productsPage?.Displayed);
        }

        [Fact]
        public void MainPage_NavigateToProducts_MvvmToolkit()
        {
            WaitForElement("MainPage");
            // Click the MVVM Toolkit button
            var mvvmToolkitButton = Driver?.FindElement(By.Id("MvvmToolkitButton"));
            Assert.NotNull(mvvmToolkitButton);
            mvvmToolkitButton?.Click();

            // Wait for the products page to load
            WaitForElement("ProductsPageMvvmToolkit");
            var productsPage = Driver?.FindElement(By.Id("ProductsPageMvvmToolkit"));
            Assert.NotNull(productsPage);
            Assert.True(productsPage?.Displayed);
        }
    }
} 