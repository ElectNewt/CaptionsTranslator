using CaptionsTranslator.Shared.Dtos;
using CaptionsTranslator.Shared.Settings;

namespace CaptionsTranslator.Core.Services;

public interface ITranslateFileService
{
    Task Execute(string fileName);
}

public class TranslateFileService : ITranslateFileService
{
    private readonly IFileService _fileService;
    private readonly ICaptionService _captionService;
    private readonly ITranslationService _translationService;
    private readonly AppSettings _appSettings;

    public TranslateFileService(IFileService fileService, ICaptionService captionService,
        ITranslationService translationService, AppSettings appSettings)
    {
        _fileService = fileService;
        _captionService = captionService;
        _translationService = translationService;
        _appSettings = appSettings;
    }


    public async Task Execute(string fileName)
    {
        CaptionFile captionFile =
            await _captionService.RetrieveCaptions(_appSettings.TranslationSettings.OriginalFolder,
                fileName);

        string translatedFile = await _translationService.TranslateFile(captionFile);
        List<Caption> translatedCaptions = _captionService.ConvertIntoCaptions(translatedFile);

        //Verify if all the captions are translated this should be done in a loop, if fails 3 times generate an exception?
        List<Caption> missingCaptions = _captionService.MissingCaptions(translatedCaptions);
        if (missingCaptions.Any())
        {
            string missingTranslatedString = await _translationService.RetrieveMissingTranslations(missingCaptions);
            List<Caption> missingTranslatedCaptions = _captionService.ConvertIntoCaptions(missingTranslatedString);
            translatedCaptions = translatedCaptions.Concat(missingTranslatedCaptions).OrderBy(a => a.Number).ToList();
        }

        string translatedString = _captionService.ConvertIntoString(translatedCaptions);

        //Dump into a file
        await _fileService.DumpCaptionsIntoFile(_appSettings.TranslationSettings.TranslationFolder, fileName,
            translatedString);
    }
}