using CaptionsTranslator.Dependencies.OpenAI;
using CaptionsTranslator.Shared.Dtos;
using CaptionsTranslator.Shared.Settings;
using Microsoft.Extensions.Options;

namespace CaptionsTranslator.Core.Services;

public interface ITranslateFileService
{
    Task Execute(string fileName);
}

public class TranslateFileService : ITranslateFileService
{
    private readonly IFileService _fileService;
    private readonly ICaptionService _captionService;
    private readonly ISubtitleTranslationService _subtitleTranslationService;
    private readonly TranslationSettings _translationSettings;

    public TranslateFileService(IFileService fileService, ICaptionService captionService,
        ISubtitleTranslationService subtitleTranslationService, IOptions<TranslationSettings> translationSettings)
    {
        _fileService = fileService;
        _captionService = captionService;
        _subtitleTranslationService = subtitleTranslationService;
        _translationSettings = translationSettings.Value;
    }


    public async Task Execute(string fileName)
    {
        //Load file with the original transcription
        CaptionFile captionFile =
            await _captionService.RetrieveCaptions(_translationSettings.OriginalFolder,
                fileName);

        //Translate into "Plain format" and store in an intermediate folder
        string translatedFile = await _subtitleTranslationService.TranslateFile(captionFile);
        await _fileService.DumpCaptionsIntoFile(_translationSettings.PlainTranslation, fileName,
            translatedFile);
        
        //Load again because sometimes chatgpt does not split properly the lines but when saving into a file, it does.
        translatedFile = await _fileService.LoadCaptionFile(_translationSettings.PlainTranslation, fileName);
        
        //Convert the string into the caption object
        List<Caption> translatedCaptions = _captionService.ConvertIntoCaptions(translatedFile);

        //Verify if all the captions are translated this should be done in a loop, if fails 3 times generate an exception?
        List<Caption> missingCaptions = _captionService.MissingCaptions(translatedCaptions);
        if (missingCaptions.Any())
        {
            string missingTranslatedString = await _subtitleTranslationService.RetrieveMissingTranslations(missingCaptions);
            List<Caption> missingTranslatedCaptions = _captionService.ConvertIntoCaptions(missingTranslatedString);
            translatedCaptions = translatedCaptions.Concat(missingTranslatedCaptions).OrderBy(a => a.Number).ToList();
        }

        //convert the captionslist into a string object and dumpit into a file
        string translatedString = _captionService.ConvertIntoString(translatedCaptions);
        await _fileService.DumpCaptionsIntoFile(_translationSettings.TranslationFolder, fileName,
            translatedString);
    }
}