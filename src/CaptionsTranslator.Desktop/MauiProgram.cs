using CaptionsTranslator.Core;
using Microsoft.Extensions.Logging;
using CaptionsTranslator.Desktop.Data;
using CaptionsTranslator.Dependencies;

namespace CaptionsTranslator.Desktop;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.Services.AddDependenciesServices();
        builder.Services.AddCoreServices();

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