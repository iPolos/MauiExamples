using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.iOS;
using OpenQA.Selenium.Appium.Enums;
using Xunit;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;
using System.IO;
using System.Reflection;
using System.Text;

namespace MauiExamples.UITests
{
    public abstract class UITestBase : IDisposable
    {
        protected IOSDriver? Driver { get; private set; }
        private const int DefaultTimeoutSeconds = 60;
        private const int RetryCount = 3;
        private const int RetryDelayMs = 1000;
        private static readonly string ScreenshotBaseDir =
            Path.Combine(Enumerable.Range(0, 5).Aggregate(Directory.GetCurrentDirectory(),
                (path, _) => Path.GetDirectoryName(path)), "testresults");
        
        // Create a unique run ID for this test session
        private static readonly string RunId = $"Run_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}";
        private static readonly string ScreenshotDir = Path.Combine(ScreenshotBaseDir, RunId);
        
        // Store Appium log entries 
        private readonly StringBuilder _appiumLogBuilder = new StringBuilder();

        protected UITestBase()
        {
            SetupDriver();
            EnsureScreenshotDirectoryExists();
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

        private void EnsureScreenshotDirectoryExists()
        {
            if (!Directory.Exists(ScreenshotBaseDir))
            {
                Directory.CreateDirectory(ScreenshotBaseDir);
            }

            // Create a unique directory for this test run
            if (!Directory.Exists(ScreenshotDir))
            {
                Directory.CreateDirectory(ScreenshotDir);
                
                // Create a README.md file with test run information
                CreateTestRunReadme();
            }
        }

        private void CreateTestRunReadme()
        {
            try
            {
                var readmePath = Path.Combine(ScreenshotDir, "README.md");
                var osVersion = Environment.OSVersion.ToString();
                var machineName = Environment.MachineName;
                var userName = Environment.UserName;
                var currentDirectory = Directory.GetCurrentDirectory();
                var appiumVersion = "Appium" + (GetAppiumVersionFromEnvironment() ?? " (version unknown)");
                var deviceInfo = GetDeviceInfo();
                
                var readmeContent = $@"# UI Test Run: {RunId}

## Environment Information
- **Date/Time**: {DateTime.Now}
- **Machine**: {machineName}
- **User**: {userName}
- **OS**: {osVersion}
- **Working Directory**: {currentDirectory}
- **Automation**: {appiumVersion}

## Device Information
{deviceInfo}

## Test Information
This folder contains screenshots captured during automated UI testing of the MAUI Examples app.
Each screenshot is named with the test name and timestamp when it was taken.

## Failure Information
Any test failures will have additional files with `_FAILURE_` in the filename, 
including both screenshots and text logs with exception details.
";

                File.WriteAllText(readmePath, readmeContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create test run README: {ex.Message}");
            }
        }

        private string GetDeviceInfo()
        {
            if (Driver == null)
                return "- No device information available";
            
            try
            {
                var platformName = Driver.Capabilities.GetCapability("platformName") ?? "Unknown";
                var platformVersion = Driver.Capabilities.GetCapability("platformVersion") ?? "Unknown";
                var deviceName = Driver.Capabilities.GetCapability("deviceName") ?? "Unknown";
                var udid = Driver.Capabilities.GetCapability("udid") ?? "Unknown";
                var appBundleId = Driver.Capabilities.GetCapability("bundleId") ?? "Unknown";
                
                return $@"- **Platform**: {platformName}
- **Version**: {platformVersion}
- **Device**: {deviceName}
- **UDID**: {udid}
- **App Bundle**: {appBundleId}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to get device information: {ex.Message}");
                return "- Failed to retrieve device information";
            }
        }

        private string? GetAppiumVersionFromEnvironment()
        {
            try
            {
                // Try to get Appium version from environment variables or other sources
                // This is just a placeholder - implement as needed
                return null;
            }
            catch
            {
                return null;
            }
        }

        protected void TakeScreenshot(string testName)
        {
            if (Driver == null) return;
            
            try
            {
                var timestamp = DateTime.Now.ToString("HHmmss");
                var fileName = $"{testName}_{timestamp}.png";
                var filePath = Path.Combine(ScreenshotDir, fileName);
                var screenshot = Driver.GetScreenshot();
                screenshot.SaveAsFile(filePath);
                Console.WriteLine($"Screenshot saved to {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to take screenshot: {ex.Message}");
            }
        }

        protected void TakeFailureScreenshot(string testName, Exception ex)
        {
            if (Driver == null) return;
            
            try
            {
                var timestamp = DateTime.Now.ToString("HHmmss");
                var fileName = $"{testName}_FAILURE_{timestamp}.png";
                var filePath = Path.Combine(ScreenshotDir, fileName);
                var screenshot = Driver.GetScreenshot();
                screenshot.SaveAsFile(filePath);
                
                // Create failure log with exception details
                var logFileName = $"{testName}_FAILURE_{timestamp}.txt";
                var logFilePath = Path.Combine(ScreenshotDir, logFileName);
                File.WriteAllText(logFilePath, $"Test: {testName}\nTimestamp: {DateTime.Now}\nException: {ex.GetType().Name}\nMessage: {ex.Message}\nStack Trace: {ex.StackTrace}");
                
                Console.WriteLine($"Failure screenshot saved to {filePath}");
                Console.WriteLine($"Failure log saved to {logFilePath}");
            }
            catch (Exception screenshotEx)
            {
                Console.WriteLine($"Failed to take failure screenshot: {screenshotEx.Message}");
            }
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
        
        private void SaveAppiumLog()
        {
            try
            {
                var logPath = Path.Combine(ScreenshotDir, "appium_log.txt");
                File.AppendAllText(logPath, _appiumLogBuilder.ToString());
                _appiumLogBuilder.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save Appium log: {ex.Message}");
            }
        }

        public void Dispose()
        {
            try
            {
                // Save any remaining log entries
                SaveAppiumLog();
                
                Driver?.Quit();
            }
            finally
            {
                Driver = null;
            }
        }
    }
} 