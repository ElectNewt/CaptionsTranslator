namespace CaptionsTranslator.Shared.Settings;

public record AppSettings
{
    public required OpenAiSettings OpenAiSettings { get; init; }
    public required TranslationSettings TranslationSettings { get; init; }
    public required YouTubeSettings YouTubeSettings { get; init; }
}