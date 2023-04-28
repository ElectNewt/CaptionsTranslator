using CaptionsTranslator.Shared.Dtos;
using CaptionsTranslator.Shared.Settings;

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
    private readonly AppSettings _appSettings;


    public PlainToCaptionService(IFileService fileService, ICaptionService captionService, AppSettings appSettings)
    {
        _fileService = fileService;
        _captionService = captionService;
        _appSettings = appSettings;
    }

    public async Task Execute(string fileName)
    {
        var contentFile = await _fileService.LoadCaptionFile(_appSettings.TranslationSettings.PlainTranslation, fileName);
        
        List<Caption> translatedCaptions = _captionService.ConvertIntoCaptions(contentFile);
        
        //This is needed to compare with the original times.
        CaptionFile captionFile=  await _captionService.RetrieveCaptions(_appSettings.TranslationSettings.OriginalFolder, fileName);
        
        
        string translatedString = _captionService.ConvertIntoString(translatedCaptions);
        await _fileService.DumpCaptionsIntoFile(_appSettings.TranslationSettings.TranslationFolder, fileName,
            translatedString);
    }
}