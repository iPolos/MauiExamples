# Setting Up Unit Tests for .NET MAUI Apps

This guide explains how to set up and run unit tests for a .NET MAUI application using xUnit.

## Prerequisites

- .NET SDK 9.0 or later
- Visual Studio 2022 or JetBrains Rider
- Basic understanding of unit testing concepts

## Step 1: Create the Test Project

1. Right-click your solution in Solution Explorer
2. Select "Add" > "New Project"
3. Search for "xUnit Test Project" and select it
4. Name it `MauiExamples.Tests`
5. Click "Create"

## Step 2: Configure the Project File

1. Right-click the test project
2. Select "Edit Project File"
3. Update the project file with the following content:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
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
    <PackageReference Include="Moq" Version="4.20.70" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\MauiExamples\MauiExamples.csproj" />
  </ItemGroup>

</Project>
```

## Step 3: Writing Your First Unit Test

Create a test class for a ViewModel:

```csharp
using System;
using System.Threading.Tasks;
using MauiExamples.ViewModels;
using Xunit;

namespace MauiExamples.Tests.ViewModels
{
    public class MainViewModelTests
    {
        [Fact]
        public void Initialize_SetsDefaultValues()
        {
            // Arrange
            var viewModel = new MainViewModel();
            
            // Act
            viewModel.Initialize();
            
            // Assert
            Assert.False(viewModel.IsLoading);
            Assert.NotNull(viewModel.Items);
            Assert.Empty(viewModel.Items);
        }
        
        [Fact]
        public async Task LoadItemsCommand_WhenExecuted_LoadsItems()
        {
            // Arrange
            var viewModel = new MainViewModel();
            
            // Act
            await viewModel.LoadItemsCommand.ExecuteAsync(null);
            
            // Assert
            Assert.False(viewModel.IsLoading);
            Assert.NotNull(viewModel.Items);
            Assert.NotEmpty(viewModel.Items);
        }
    }
}
```

## Step 4: Using Moq for Dependencies

For testing components with dependencies:

```csharp
using System.Threading.Tasks;
using MauiExamples.Models;
using MauiExamples.Services;
using MauiExamples.ViewModels;
using Moq;
using Xunit;

namespace MauiExamples.Tests.ViewModels
{
    public class ItemDetailViewModelTests
    {
        [Fact]
        public async Task LoadItem_WhenCalled_LoadsItemFromService()
        {
            // Arrange
            var mockItemService = new Mock<IItemService>();
            var expectedItem = new Item { Id = 1, Name = "Test Item", Description = "Test Description" };
            
            mockItemService
                .Setup(service => service.GetItemAsync(1))
                .ReturnsAsync(expectedItem);
                
            var viewModel = new ItemDetailViewModel(mockItemService.Object);
            
            // Act
            await viewModel.LoadItemAsync(1);
            
            // Assert
            Assert.Equal(expectedItem.Id, viewModel.Item.Id);
            Assert.Equal(expectedItem.Name, viewModel.Item.Name);
            Assert.Equal(expectedItem.Description, viewModel.Item.Description);
            
            mockItemService.Verify(service => service.GetItemAsync(1), Times.Once);
        }
    }
}
```

## Step 5: Testing MAUI-Specific Code

When testing MAUI-specific components, you may need to mock the platform services:

```csharp
using MauiExamples.Services;
using Moq;
using Xunit;

namespace MauiExamples.Tests.Services
{
    public class PlatformServiceTests
    {
        [Fact]
        public void GetPlatformName_ReturnsCorrectPlatform()
        {
            // Arrange
            var mockDeviceInfo = new Mock<IDeviceInfo>();
            mockDeviceInfo.Setup(di => di.Platform).Returns(DevicePlatform.iOS);
            
            var platformService = new PlatformService(mockDeviceInfo.Object);
            
            // Act
            var result = platformService.GetPlatformName();
            
            // Assert
            Assert.Equal("iOS", result);
        }
    }
}
```

## Step 6: Running the Tests

### From Visual Studio:
1. Open Test Explorer (Test > Test Explorer)
2. Click "Run All" or run individual tests

### From Command Line:
```bash
dotnet test MauiExamples.Tests/MauiExamples.Tests.csproj
```

## Best Practices for Unit Testing MAUI Apps

1. **Isolate UI and Business Logic**:
   - Separate UI-specific code from business logic
   - Use MVVM pattern to make ViewModels testable
   - Limit platform-specific code in testable components

2. **Use Dependency Injection**:
   - Inject services rather than creating them directly
   - This makes it easier to mock dependencies during testing

3. **Focus on Business Logic**:
   - Prioritize testing business logic over UI behavior
   - UI testing should be handled through UI tests (see the MauiExamples.UITests project)

Happy testing! 