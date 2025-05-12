using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.iOS;
using OpenQA.Selenium.Appium.Enums;
using Xunit;
using OpenQA.Selenium.Support.UI;

namespace MauiExamples.UITests
{
    public abstract class UITestBase : IDisposable
    {
        protected AppiumDriver? Driver { get; private set; }
        private const string AppiumServerUrl = "http://localhost:4723";

        protected UITestBase()
        {
            SetupDriver();
        }

        private void SetupDriver()
        {
            var options = new AppiumOptions();
            
            // Configure for iOS
            options.PlatformName = "iOS";
            options.AutomationName = "XCUITest";
            options.App = "/Users/poloswelsen/Documents/projects/ipolos/ID.MauiExamples/MauiExamples/MauiExamples.UITests/bin/Debug/net9.0/MauiExamples.app";
            options.AddAdditionalAppiumOption("bundleId", "com.companyname.mauiexamples");
            options.DeviceName = "iPhone 16";
            options.PlatformVersion = "18.4";
            options.AddAdditionalAppiumOption("newCommandTimeout", 120); // 2 minutes timeout
            options.AddAdditionalAppiumOption("launchTimeout", 120000); // 2 minutes launch timeout
            options.AddAdditionalAppiumOption("noReset", false); // Reset app state between tests
            options.AddAdditionalAppiumOption("autoAcceptAlerts", true); // Auto-accept system alerts
            options.AddAdditionalAppiumOption("showIOSLog", true); // Show iOS logs

            Driver = new IOSDriver(new Uri(AppiumServerUrl), options);
        }

        protected IWebElement WaitForElement(string id, int timeoutSeconds = 30)
        {
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutSeconds));
            return wait.Until(d => d.FindElement(By.Id(id)));
        }

        protected void ResetToMainPage()
        {
            try
            {
                // Try to find and click the back button if we're not on the main page
                var backButton = Driver?.FindElement(By.Id("BackButton"));
                if (backButton?.Displayed == true)
                {
                    backButton.Click();
                }
                
                // Wait for main page to be visible
                WaitForElement("MainPage");
            }
            catch
            {
                // If we can't navigate back, just restart the driver
                Driver?.Quit();
                Driver?.Dispose();
                SetupDriver();
                WaitForElement("MainPage");
            }
        }

        public void Dispose()
        {
            try
            {
                ResetToMainPage();
            }
            finally
            {
                Driver?.Quit();
                Driver?.Dispose();
            }
        }
    }
} 