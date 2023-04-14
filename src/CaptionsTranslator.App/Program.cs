using CaptionsTranslator.Core;
using CaptionsTranslator.Core.Services;
using CaptionsTranslator.Shared.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.private.json", optional: true)
    .AddEnvironmentVariables()
    .Build();


OpenAiSettings openAiSettings = config.GetRequiredSection("OpenAiSettings").Get<OpenAiSettings>();
TranslationSettings translationSettings = config.GetRequiredSection("TranslationSettings").Get<TranslationSettings>();


IServiceCollection serviceCollection = new ServiceCollection()
    .AddCoreServices()
    .AddScoped<AppSettings>(_ => new AppSettings()
    {
        OpenAiSettings = openAiSettings,
        TranslationSettings = translationSettings
    });

ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
ITranslateFileService translateFile = serviceProvider.GetRequiredService<ITranslateFileService>();

IDirectoryService directoryService = serviceProvider.GetRequiredService<IDirectoryService>();

foreach (string file in directoryService.GetFileNames())
{
    await translateFile.Execute(file);
}


Console.Write("stop");