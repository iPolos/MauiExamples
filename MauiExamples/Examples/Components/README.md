# .NET MAUI Component Examples

This directory contains examples of various components and features available in .NET MAUI. Each component is organized in its own folder with a dedicated README that explains its implementation and usage.

## Project Structure

```
Components/
├── README.md                     # This file
├── ComponentsPage.xaml           # Main navigation page for components
├── ComponentsPage.xaml.cs        # Code-behind for the main navigation
└── [ComponentName]/              # Folder for each component
    ├── README.md                 # Documentation for the component
    ├── [ComponentName]Page.xaml  # UI for the component demo
    └── [ComponentName]Page.xaml.cs # Implementation of the component demo
```

## Adding New Components

To add a new component example:

1. Create a new folder with the component name
2. Add the implementation files (.xaml and .xaml.cs) 
3. Create a README.md file explaining the component
4. Register the page in MauiProgram.cs:
   ```csharp
   builder.Services.AddTransient<Examples.Components.YourComponent.YourComponentPage>();
   ```
5. Add the route in AppShell.xaml.cs:
   ```csharp
   Routing.RegisterRoute("examples/components/yourcomponent", typeof(Examples.Components.YourComponent.YourComponentPage));
   ```
6. Add the component to the list in ComponentsPage.xaml.cs:
   ```csharp
   _components = new ObservableCollection<ComponentItem>
   {
       // Existing components...
       new ComponentItem
       {
           Name = "Your Component Name",
           Description = "Brief description of your component",
           PageType = typeof(YourComponent.YourComponentPage)
       }
   };
   ```

## Available Components

- **[Local Notifications](./LocalNotifications/README.md)**: Examples of how to use local notifications in MAUI
- **[Camera](./Camera/README.md)**: Take photos and manage a photo gallery

<!-- Add more components here as they are created --> 