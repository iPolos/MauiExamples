# Setting Up UI Tests for .NET MAUI Apps with Appium

This guide walks you through the process of setting up UI tests for a .NET MAUI application using Appium and xUnit.

## Prerequisites

- .NET SDK 9.0 or later
- Visual Studio 2022 or JetBrains Rider
- iOS or Android development environment set up
- Appium server installed (`npm install -g appium`)
- Appium drivers for your target platform:
  - iOS: `appium driver install xcuitest`
  - Android: `appium driver install uiautomator2`

## Step 1: Create the Test Project

1. Right-click your solution in Solution Explorer
2. Select "Add" > "New Project"
3. Search for "xUnit Test Project" and select it
4. Name it `[YourProjectName].UITests` (e.g., `MauiExamples.UITests`)
5. Click "Create"

## Step 2: Configure the Project File

1. Right-click the test project
2. Select "Edit Project File"
3. Update the target framework to include `net9.0` (required for MAUI compatibility)
4. Add required NuGet packages

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    ...
  </PropertyGroup>
  ...
  <ItemGroup>
    <PackageReference Include="Appium.WebDriver" Version="5.0.0-beta05" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.9.0" />
    <PackageReference Include="xunit" Version="2.6.6" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.5.6">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="coverlet.collector" Version="6.0.0">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
  </ItemGroup>
  ...
</Project>
```

## Step 3: Create the Base Test Class

Create a new file called `UITestBase.cs` with the following code:

```csharp
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.iOS;
using OpenQA.Selenium.Support.UI;
using System;
using System.IO;
using Xunit;

namespace YourNamespace.UITests
{
    public abstract class UITestBase : IDisposable
    {
        protected IOSDriver? Driver { get; private set; }
        private const int DefaultTimeoutSeconds = 60;
        private const int RetryCount = 3;
        private const int RetryDelayMs = 1000;
        
        // Configure screenshot path
        private static readonly string ScreenshotBaseDir = Path.Combine(
            Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(
                Path.GetDirectoryName(Directory.GetCurrentDirectory())))),
            "testresults");
        
        // Create a unique run ID for this test session
        private static readonly string RunId = $"Run_{DateTime.Now:yyyyMMdd_HHmmss}";
        private static readonly string ScreenshotDir = Path.Combine(ScreenshotBaseDir, RunId);

        protected UITestBase()
        {
            SetupDriver();
            EnsureScreenshotDirectoryExists();
        }

        private void SetupDriver()
        {
            var options = new AppiumOptions();
            
            // Configure for iOS - adjust for Android if needed
            options.PlatformName = "iOS";
            options.AutomationName = "XCUITest";
            options.DeviceName = "iPhone 16"; // Update with your device/simulator name
            options.PlatformVersion = "18.4"; // Update with your iOS version
            
            // Set your app's bundle ID
            options.AddAdditionalAppiumOption("bundleId", "com.companyname.yourapp");
            options.AddAdditionalAppiumOption("newCommandTimeout", 600);
            options.AddAdditionalAppiumOption("launchTimeout", 300000);

            var retryCount = 0;
            while (retryCount < RetryCount)
            {
                try
                {
                    Driver = new IOSDriver(new Uri("http://localhost:4723"), options, 
                        TimeSpan.FromSeconds(DefaultTimeoutSeconds));
                    return;
                }
                catch when (retryCount < RetryCount - 1)
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

        // Setup screenshot directory
        private void EnsureScreenshotDirectoryExists()
        {
            if (!Directory.Exists(ScreenshotBaseDir))
            {
                Directory.CreateDirectory(ScreenshotBaseDir);
            }

            if (!Directory.Exists(ScreenshotDir))
            {
                Directory.CreateDirectory(ScreenshotDir);
                CreateTestRunReadme();
            }
        }

        // Create README file for the test run
        private void CreateTestRunReadme()
        {
            try
            {
                var readmePath = Path.Combine(ScreenshotDir, "README.md");
                var osVersion = Environment.OSVersion.ToString();
                var machineName = Environment.MachineName;
                var userName = Environment.UserName;
                var currentDirectory = Directory.GetCurrentDirectory();
                
                var readmeContent = $@"# UI Test Run: {RunId}

                    ## Environment Information
                    - **Date/Time**: {DateTime.Now}
                    - **Machine**: {machineName}
                    - **User**: {userName}
                    - **OS**: {osVersion}
                    - **Working Directory**: {currentDirectory}

                    ## Test Information
                    This folder contains screenshots captured during automated UI testing.
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

        // Take screenshots during tests
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

        // Take failure screenshots
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
                File.WriteAllText(logFilePath, 
                    $"Test: {testName}\nTimestamp: {DateTime.Now}\n" +
                    $"Exception: {ex.GetType().Name}\nMessage: {ex.Message}\n" +
                    $"Stack Trace: {ex.StackTrace}");
                
                Console.WriteLine($"Failure screenshot saved to {filePath}");
                Console.WriteLine($"Failure log saved to {logFilePath}");
            }
            catch (Exception screenshotEx)
            {
                Console.WriteLine($"Failed to take failure screenshot: {screenshotEx.Message}");
            }
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
```

## Step 4: Add Automation IDs to Your MAUI App

In your XAML files, add `AutomationId` properties to elements you want to test:

```xml
<ContentPage
    x:Class="YourNamespace.MainPage"
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    AutomationId="MainPage">
    
    <StackLayout>
        <Label 
            Text="Welcome to .NET MAUI!"
            AutomationId="WelcomeLabel" />
        
        <Button 
            Text="Click Me" 
            AutomationId="ClickMeButton" />
    </StackLayout>
</ContentPage>
```

## Step 5: Create Your First Test Class

Create a file named `MainPageTests.cs`:

```csharp
using OpenQA.Selenium;
using Xunit;

namespace YourNamespace.UITests
{
    public class MainPageTests : UITestBase
    {
        [Fact]
        public void MainPage_LoadsSuccessfully()
        {
            try
            {
                // Wait for the main page to load
                WaitForElement("MainPage", 120);
                
                // Take screenshot of the main page
                TakeScreenshot("MainPage_LoadsSuccessfully");
                
                // Verify the welcome label is displayed
                var welcomeLabel = Driver?.FindElement(By.Id("WelcomeLabel"));
                Assert.NotNull(welcomeLabel);
                Assert.True(welcomeLabel?.Displayed);
                
                // Verify the button is displayed
                var clickMeButton = Driver?.FindElement(By.Id("ClickMeButton"));
                Assert.NotNull(clickMeButton);
                Assert.True(clickMeButton?.Displayed);
            }
            catch (Exception ex)
            {
                TakeFailureScreenshot("MainPage_LoadsSuccessfully", ex);
                throw;
            }
        }
        
        [Fact]
        public void ClickMeButton_ClickWorks()
        {
            try
            {
                // Wait for the main page to load
                WaitForElement("MainPage", 120);
                
                // Take screenshot before clicking
                TakeScreenshot("ClickMeButton_Before");
                
                // Click the button
                var clickMeButton = Driver?.FindElement(By.Id("ClickMeButton"));
                clickMeButton?.Click();
                
                // Wait for result element to appear
                WaitForElement("ResultLabel", 30);
                
                // Take screenshot after clicking
                TakeScreenshot("ClickMeButton_After");
                
                // Verify the result
                var resultLabel = Driver?.FindElement(By.Id("ResultLabel"));
                Assert.NotNull(resultLabel);
                Assert.True(resultLabel?.Displayed);
                Assert.Contains("clicked", resultLabel?.Text);
            }
            catch (Exception ex)
            {
                TakeFailureScreenshot("ClickMeButton_ClickWorks", ex);
                throw;
            }
        }
    }
}
```

## Step 6: Start Appium Server

Before running tests, start the Appium server:

```bash
appium
```

## Step 7: Build and Deploy Your App

1. Build your MAUI app for the target platform (iOS/Android)

### For iOS:
- This creates an .app file for simulators or .ipa file for real devices
- The output will be in `YourProject/bin/Debug/net9.0-ios/`

```bash
dotnet build -t:Build -f net9.0-ios /p:Configuration=Debug /p:CreatePackage=true /p:BuildIpa=true
```

- This moves the `.app` to the correct directory for the UI Tests to pick up the application

```bash
mkdir -p UITestApps/iOS
cp -R YourProject/bin/Debug/net9.0-ios/[DeviceType]/YourApp.app UITestApps/iOS/
```

- Update the Appium configuration in your `UITestBase.cs` file to point to the app location:

```csharp
options.App = Path.Combine(Directory.GetCurrentDirectory(), "UITestApps/iOS/YourApp.app");
```

### For Android:
- This creates an .apk file
- The output will be in `YourProject/bin/Debug/net9.0-android/`

```bash
dotnet build -t:Build -f net9.0-android /p:Configuration=Debug
```

- This moves the `.apk` to the correct directory for the UI Tests to pick up the application

```bash
mkdir -p UITestApps/Android
cp YourProject/bin/Debug/net9.0-android/YourApp.apk UITestApps/Android/
```

- Update the Appium configuration in your `UITestBase.cs` file to point to the app location:

```csharp
options.App = Path.Combine(Directory.GetCurrentDirectory(), "UITestApps/Android/YourApp.apk");
```

3. Do not launch the app manually - Appium will handle the installation and launch process

## Step 8: Run the Tests

From the terminal:
```bash
dotnet test YourProject.UITests/YourProject.UITests.csproj
```

Or use the Test Explorer in Visual Studio / Rider.

## Troubleshooting

### Common Issues

1. **Appium Connection Errors**:
   - Ensure Appium server is running
   - Check the server port (default is 4723)
   - Verify drivers are installed for your platform

2. **Element Not Found Errors**:
   - Verify the AutomationId matches exactly
   - Increase timeouts for WaitForElement
   - Check if the element is actually visible in the app

3. **App Launch Issues**:
   - Verify the bundleId is correct
   - Ensure app is installed on the device/simulator
   - Check device/simulator name and platform version

4. **Build Errors**:
   - Ensure target framework is set to net9.0
   - Check for NuGet package compatibility
   - Verify all dependencies are properly referenced

### Tips for Successful UI Testing

1. **Add Try/Catch Blocks**: Wrap test code in try/catch and take failure screenshots
2. **Use Element Waiting**: Always wait for elements before interacting with them
3. **Take Screenshots**: Capture screenshots at key points for visual verification
4. **Add Sufficient Timeouts**: UI tests often need longer timeouts than expected
5. **Keep Tests Independent**: Each test should be able to run independently
6. **Skip Unreliable Tests**: Use `[Fact(Skip = "reason")]` for tests with environment issues


Happy testing!