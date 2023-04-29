namespace CaptionsTranslator.Shared.Settings;

#nullable  disable
public record TranslationSettings
{
    public string OriginalFolder { get; init; }
    public string TranslationFolder { get; init; }
    public string PlainTranslation { get; init; }
}