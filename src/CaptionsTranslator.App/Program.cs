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

//TODO: This should be done on their own methods
ITranslateFileService translateFile = serviceProvider.GetRequiredService<ITranslateFileService>();
ITranslationService translationService = serviceProvider.GetRequiredService<ITranslationService>();
IPlainToCaptionService plainToCaptionService = serviceProvider.GetRequiredService<IPlainToCaptionService>();
IYouTube youTube = serviceProvider.GetRequiredService<IYouTube>();

/*******  PROGRAM STARTS HERE *********/

int menuOptionSelected = 0;
do
{
    menuOptionSelected = PrintMenu();
    if (menuOptionSelected != 0)
    {
        string videoId = GetVideoIdFromUser();
        switch (menuOptionSelected)
        {
            case 1:
                await TranslateVideoMetadata(videoId);
                break;
            case 2:
                await TranslateCaptions(videoId);
                break;
            case 3:
                await PlainToCaption(videoId);
                break;
        }
    }
} while (menuOptionSelected != 0);

string? MenuOption()
{
    Console.WriteLine("1 - Translate video title and description");
    Console.WriteLine("2 - Translate subtitles (very expensive)");
    Console.WriteLine("3 - plain file to captions");
    Console.WriteLine("0 - Exit");
    string? result = Console.ReadLine();
    return result;
}

int PrintMenu()
{
    int? result = null;
    do
    {
        string? selectedOption = MenuOption();

        if (int.TryParse(selectedOption, out int r))
            result = r;
    } while (result == null || (result >= 4 || result <= -1));

    return (int)result;
}

string GetVideoIdFromUser()
{
    Console.Write("Specify videoId: ");
    return Console.ReadLine() ?? string.Empty;
}


async Task TranslateVideoMetadata(string videoId)
{
    //Update video title and description
    var videoInformation = await youTube.GetVideoInformation(videoId);
    Console.WriteLine($"Original title: {videoInformation.Title}");
    Console.WriteLine("Translating it to English...");
    string translatedTitle = await translationService.PlainTranslation(videoInformation.Title);
    string translatedDescription = await translationService.PlainTranslation(videoInformation.Description);
    await youTube.UpdateVideoLocalization(videoId, translatedTitle, translatedDescription);

    Console.WriteLine("Translated and updated in Youtube.");
}

async Task TranslateCaptions(string videoId)
{
    Console.WriteLine($"Translate captions for video: {videoId}; this might take a while.");
    //Download the transcription file on the default language
    //Translate it
    //update it to Youtube translated into English
    string captionsFileName = await youTube.DownloadCaptionFile(videoId);
    await translateFile.Execute(captionsFileName);
    await youTube.UploadTranslatedFile(videoId);
}

async Task PlainToCaption(string videoId)
{
    Console.WriteLine($"plain to caption for video {videoId}");
    await plainToCaptionService.Execute($"{videoId}.srt");
}