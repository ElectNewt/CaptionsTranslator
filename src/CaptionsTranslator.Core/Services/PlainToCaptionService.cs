using CaptionsTranslator.Shared.Dtos;
using CaptionsTranslator.Shared.Settings;
using Microsoft.Extensions.Options;

namespace CaptionsTranslator.Core.Services;

public interface IPlainToCaptionService
{
    Task Execute(string fileName);
}

/// <summary>
/// Allows to translate the intermediate file (in case there was any problem during the other step)
/// </summary>
public class PlainToCaptionService : IPlainToCaptionService
{
    private readonly IFileService _fileService;
    private readonly ICaptionService _captionService;
    private readonly TranslationSettings _translationSettings;


    public PlainToCaptionService(IFileService fileService, ICaptionService captionService, IOptions<TranslationSettings> translationSettings)
    {
        _fileService = fileService;
        _captionService = captionService;
        _translationSettings = translationSettings.Value;
    }

    public async Task Execute(string fileName)
    {
        var contentFile = await _fileService.LoadCaptionFile(_translationSettings.PlainTranslation, fileName);
        
        List<Caption> translatedCaptions = _captionService.ConvertIntoCaptions(contentFile);
        
        //This is needed to compare with the original times.
        CaptionFile captionFile=  await _captionService.RetrieveCaptions(_translationSettings.OriginalFolder, fileName);
        
        
        string translatedString = _captionService.ConvertIntoString(translatedCaptions);
        await _fileService.DumpCaptionsIntoFile(_translationSettings.TranslationFolder, fileName,
            translatedString);
    }
}