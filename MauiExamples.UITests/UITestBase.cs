using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.iOS;
using OpenQA.Selenium.Appium.Enums;
using Xunit;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;

namespace MauiExamples.UITests
{
    public abstract class UITestBase : IDisposable
    {
        protected IOSDriver Driver { get; private set; }
        private const int DefaultTimeoutSeconds = 60;
        private const int RetryCount = 3;
        private const int RetryDelayMs = 1000;

        protected UITestBase()
        {
            SetupDriver();
        }

        private void SetupDriver()
        {
            var options = new AppiumOptions();
            options.PlatformName = "iOS";
            options.AutomationName = "XCUITest";
            options.DeviceName = "iPhone 16";
            options.PlatformVersion = "18.4";
            options.AddAdditionalAppiumOption("bundleId", "com.companyname.mauiexamples");
            options.AddAdditionalAppiumOption("newCommandTimeout", 600);
            options.AddAdditionalAppiumOption("launchTimeout", 300000);

            var retryCount = 0;
            while (retryCount < RetryCount)
            {
                try
                {
                    Driver = new IOSDriver(new Uri("http://localhost:4723"), options, TimeSpan.FromSeconds(DefaultTimeoutSeconds));
                    return;
                }
                catch (Exception ex) when (retryCount < RetryCount - 1)
                {
                    retryCount++;
                    Thread.Sleep(RetryDelayMs);
                }
            }

            throw new Exception($"Failed to initialize driver after {RetryCount} attempts");
        }

        protected void WaitForElement(string id, int timeoutSeconds = DefaultTimeoutSeconds)
        {
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutSeconds));
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
            wait.Until(driver =>
            {
                try
                {
                    return driver.FindElement(By.Id(id)).Displayed;
                }
                catch
                {
                    return false;
                }
            });
        }

        public void Dispose()
        {
            try
            {
                Driver?.Quit();
            }
            finally
            {
                Driver = null;
            }
        }
    }
} 