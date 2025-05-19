namespace MauiExamples;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        
        // Register routes for navigation
        Routing.RegisterRoute("examples/standard/products", typeof(Examples.Standard.ProductsPage));
        Routing.RegisterRoute("examples/standard/details", typeof(Examples.Standard.ProductDetailPage));
        
        Routing.RegisterRoute("examples/vanilla/products", typeof(Examples.VanillaMvvm.Views.ProductsPage));
        Routing.RegisterRoute("examples/vanilla/details", typeof(Examples.VanillaMvvm.Views.ProductDetailPage));
        
        Routing.RegisterRoute("examples/toolkit/products", typeof(Examples.MvvmToolkit.Views.ProductsPage));
        Routing.RegisterRoute("examples/toolkit/details", typeof(Examples.MvvmToolkit.Views.ProductDetailPage));
        
        // Register routes for components
        Routing.RegisterRoute("examples/components", typeof(Examples.Components.ComponentsPage));
        Routing.RegisterRoute("examples/components/localnotification", typeof(Examples.Components.LocalNotifications.LocalNotificationPage));
        Routing.RegisterRoute("examples/components/camera", typeof(Examples.Components.Camera.CameraPage));
        Routing.RegisterRoute("examples/components/camera/detail", typeof(Examples.Components.Camera.PhotoDetailPage));
        Routing.RegisterRoute("examples/components/maps", typeof(Examples.Components.Maps.MapsPage));
    }
}