using CaptionsTranslator.Shared.Settings;
using Microsoft.Extensions.Options;

namespace CaptionsTranslator.Core.Services;

public interface IDirectoryService
{
    string?[] GetFileNames();
}

public class DirectoryService : IDirectoryService
{
    private readonly TranslationSettings _translationSettings;

    public DirectoryService(IOptions<TranslationSettings> translationSettings)
    {
        _translationSettings = translationSettings.Value;
    }


    public string?[] GetFileNames()
        => Directory.GetFiles(_translationSettings.OriginalFolder)
            .Select(Path.GetFileName).ToArray();
}