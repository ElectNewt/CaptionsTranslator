namespace CaptionsTranslator.Shared.Settings;

public record OpenAiSettings
{
    public required string API_KEY { get; init; }
}