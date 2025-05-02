using Microsoft.Extensions.Logging;
using MauiExamples.Services;
using MauiExamples.Examples.Standard;
using MauiExamples.Examples.VanillaMvvm.Views;
using MauiExamples.Examples.MvvmToolkit.Views;
using CommunityToolkit.Maui;

namespace MauiExamples;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Register services
        builder.Services.AddSingleton<ProductService>();

        // Register pages
        builder.Services.AddTransient<MainPage>();
        
        // Standard Implementation
        builder.Services.AddTransient<Examples.Standard.ProductsPage>();
        builder.Services.AddTransient<Examples.Standard.ProductDetailPage>();
        
        // Vanilla MVVM Implementation
        builder.Services.AddTransient<Examples.VanillaMvvm.Views.ProductsPage>();
        builder.Services.AddTransient<Examples.VanillaMvvm.Views.ProductDetailPage>();
        
        // MVVM Toolkit Implementation
        builder.Services.AddTransient<Examples.MvvmToolkit.Views.ProductsPage>();
        builder.Services.AddTransient<Examples.MvvmToolkit.Views.ProductDetailPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}