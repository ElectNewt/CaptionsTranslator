namespace CaptionsTranslator.Shared.Settings;

public record TranslationSettings
{
    public required string OriginalFolder { get; init; }
    public required string TranslationFolder { get; init; }
    public required string PlainTranslation { get; init; }
}