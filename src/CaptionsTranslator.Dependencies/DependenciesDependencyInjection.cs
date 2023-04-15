using CaptionsTranslator.Dependencies.OpenAI;
using CaptionsTranslator.Dependencies.YouTube;
using Microsoft.Extensions.DependencyInjection;

namespace CaptionsTranslator.Dependencies;

public static class DependenciesDependencyInjection
{
    public static IServiceCollection AddDependenciesServices(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddScoped<ISubtitleTranslationService, SubtitleSubtitleTranslationService>()
            .AddScoped<ITranslationService, TranslationService>()
            .AddSingleton<IYouTubeClientFactory, YouTubeClientFactory>()
            .AddScoped<IYouTube, YouTubeImplementation>();

    }
}