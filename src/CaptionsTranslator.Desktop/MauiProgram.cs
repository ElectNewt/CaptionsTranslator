using CaptionsTranslator.Core;
using Microsoft.Extensions.Logging;
using CaptionsTranslator.Desktop.Data;
using CaptionsTranslator.Dependencies;
using Microsoft.Extensions.Configuration;
using CaptionsTranslator.Shared;

namespace CaptionsTranslator.Desktop;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.Configuration.AddConfiguration(SharedDependencyInjection.BuildConfiguration());

        builder.Services.AddSingleton<IAppState, AppState>();
        builder.Services.AddDependenciesServices();
        builder.Services.AddCoreServices();
        builder.Services.AddSharedConfiguration(builder.Configuration);

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); });

        

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<WeatherForecastService>();
        

        return builder.Build();
    }
}