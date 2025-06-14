﻿using Microsoft.Extensions.Logging;
using MauiExamples.Services;
using MauiExamples.Views;
using CommunityToolkit.Maui;
using Plugin.LocalNotification;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace MauiExamples;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseLocalNotification()
            .UseMauiMaps()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
            
        // Register authentication service first
        builder.Services.AddSingleton<AuthService>();
            
        // Register services
        // Default implementation - use factory to inject AuthService
        builder.Services.AddSingleton<IProductService>(sp => 
            new ApiProductService(sp.GetRequiredService<AuthService>()));
        
        // Register the concrete types as well for direct injection when needed
        builder.Services.AddSingleton<ApiProductService>(sp => 
            new ApiProductService(sp.GetRequiredService<AuthService>()));

        // Register auth pages
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        
        // Register ViewModels
        // Vanilla MVVM ViewModels
        builder.Services.AddTransient<MauiExamples.Examples.VanillaMvvm.ViewModels.ProductsViewModel>();
        builder.Services.AddTransient<MauiExamples.Examples.VanillaMvvm.ViewModels.ProductDetailViewModel>();
        builder.Services.AddTransient<MauiExamples.Examples.VanillaMvvm.ViewModels.AddProductViewModel>();
        
        // MVVM Toolkit ViewModels
        builder.Services.AddTransient<MauiExamples.Examples.MvvmToolkit.ViewModels.ProductsViewModel>();
        builder.Services.AddTransient<MauiExamples.Examples.MvvmToolkit.ViewModels.ProductDetailViewModel>();
        builder.Services.AddTransient<MauiExamples.Examples.MvvmToolkit.ViewModels.AddProductViewModel>();

        // Register pages
        builder.Services.AddTransient<MainPage>();
        
        // Standard Implementation
        builder.Services.AddTransient<Examples.Standard.ProductsPage>();
        builder.Services.AddTransient<Examples.Standard.ProductDetailPage>();
        
        // Vanilla MVVM Implementation
        builder.Services.AddTransient<Examples.VanillaMvvm.Views.ProductsPage>();
        builder.Services.AddTransient<Examples.VanillaMvvm.Views.ProductDetailPage>();
        builder.Services.AddTransient<Examples.VanillaMvvm.Views.AddProductPage>();
        
        // MVVM Toolkit Implementation
        builder.Services.AddTransient<Examples.MvvmToolkit.Views.ProductsPage>();
        builder.Services.AddTransient<Examples.MvvmToolkit.Views.ProductDetailPage>();
        builder.Services.AddTransient<Examples.MvvmToolkit.Views.AddProductPage>();
        
        // Components Implementation
        builder.Services.AddTransient<Examples.Components.ComponentsPage>();
        builder.Services.AddTransient<Examples.Components.LocalNotifications.LocalNotificationPage>();
        builder.Services.AddTransient<Examples.Components.Camera.CameraPage>();
        builder.Services.AddTransient<Examples.Components.Camera.PhotoDetailPage>();
        builder.Services.AddTransient<Examples.Components.Maps.MapsPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}