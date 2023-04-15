using CaptionsTranslator.Core;
using CaptionsTranslator.Core.Services;
using CaptionsTranslator.Dependencies;
using CaptionsTranslator.Dependencies.OpenAI;
using CaptionsTranslator.Dependencies.YouTube;
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
YouTubeSettings youTubeSettings = config.GetRequiredSection("YouTubeSettings").Get<YouTubeSettings>();

IServiceCollection serviceCollection = new ServiceCollection()
    .AddDependenciesServices()
    .AddCoreServices()
    .AddScoped<AppSettings>(_ => new AppSettings()
    {
        OpenAiSettings = openAiSettings,
        TranslationSettings = translationSettings,
        YouTubeSettings = youTubeSettings
    });

ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

/*******  PROGRAM STARTS HERE *********/

ITranslateFileService translateFile = serviceProvider.GetRequiredService<ITranslateFileService>();
ITranslationService translationService = serviceProvider.GetRequiredService<ITranslationService>();
IYouTube youTube = serviceProvider.GetRequiredService<IYouTube>();

string videoId = "PasteYourOwn";

//Update video title and description
var videoInformation = await youTube.GetVideoInformation(videoId);
 string translatedTitle = await translationService.PlainTranslation(videoInformation.Title);
 string translatedDescription = await translationService.PlainTranslation(videoInformation.Description);
 await youTube.UpdateVideoLocalization(videoId,translatedTitle,translatedDescription);

//Download the transcription file on the default language
//Translate it
//update it to Youtube translated into English
string captionsFileName = await youTube.DownloadCaptionFile(videoId);
await translateFile.Execute(captionsFileName);
await youTube.UploadTranslatedFile(videoId);
