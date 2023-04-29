namespace CaptionsTranslator.Shared.Settings;

#nullable  disable
public record YouTubeSettings
{
    public string ApplicationName { get; init; }
    public string OAuth2ClientId { get; init; }
    public string OAuth2ClientSecret { get; init; }
}
