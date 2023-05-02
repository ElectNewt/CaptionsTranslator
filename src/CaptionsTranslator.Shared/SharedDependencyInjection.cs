using CaptionsTranslator.Shared.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CaptionsTranslator.Shared
{
    public static class SharedDependencyInjection
    {
        public static IServiceCollection AddSharedConfiguration(this IServiceCollection serviceCollection, IConfiguration config)
        => serviceCollection
            .Configure<OpenAiSettings>(config.GetRequiredSection("OpenAiSettings"))
            .Configure<TranslationSettings>(config.GetRequiredSection("TranslationSettings"))
            .Configure<YouTubeSettings>(config.GetRequiredSection("YouTubeSettings"));

        public static IConfiguration BuildConfiguration()
        => new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", optional: true)
               .AddJsonFile("appsettings.private.json", optional: true)
               .AddEnvironmentVariables()
               .Build();
    }
}
