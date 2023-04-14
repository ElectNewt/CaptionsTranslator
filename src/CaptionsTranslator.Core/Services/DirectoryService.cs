using CaptionsTranslator.Shared.Settings;

namespace CaptionsTranslator.Core.Services;

public interface IDirectoryService
{
    string?[] GetFileNames();
}

public class DirectoryService : IDirectoryService
{
    private readonly AppSettings _appSettings;

    public DirectoryService(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }


    public string?[] GetFileNames()
        => Directory.GetFiles(_appSettings.TranslationSettings.OriginalFolder)
            .Select(Path.GetFileName).ToArray();
}