namespace CaptionsTranslator.Shared.Settings;

public record YouTubeSettings
{
    public required string ApplicationName { get; init; }
    public required string OAuth2ClientId { get; init; }
    public required string OAuth2ClientSecret { get; init; }
}
