using CaptionsTranslator.Shared.Settings;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.Extensions.Options;

namespace CaptionsTranslator.Dependencies.YouTube;

public interface IYouTubeClientFactory
{
    Task<YouTubeService> GetYouTubeClient();
}

public class YouTubeClientFactory : IYouTubeClientFactory
{
    private readonly YouTubeSettings _configuration;
    private YouTubeService? YouTubeService { get; set; }

    private UserCredential? UserCredential { get; set; }


    public YouTubeClientFactory(IOptions<YouTubeSettings> configuration)
    {
        _configuration = configuration.Value;
        YouTubeService = null;
    }

    public async Task<YouTubeService> GetYouTubeClient()
    {
        TokenResponse authToken = await CalculateToken();

        return YouTubeService ??= new YouTubeService(new BaseClientService.Initializer()
        {
            ApplicationName = _configuration.ApplicationName,
            HttpClientInitializer = GoogleCredential.FromAccessToken(authToken.AccessToken)
        });
    }

    private async Task<TokenResponse> CalculateToken()
    {
        bool isRefreshed = await RefreshToken();
        
        if (UserCredential != null && !isRefreshed) //TODO: Validate if the token still valid if not refresh it.
            return UserCredential.Token;


        UserCredential? userCredential = await GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets()
        {
            ClientId = _configuration.OAuth2ClientId,
            ClientSecret = _configuration.OAuth2ClientSecret
        }, new string[]
        {
            "https://www.googleapis.com/auth/youtube.force-ssl",
        }, "user", default);


        // TokenResponse contains the tokens, access token expiry time etc.
        UserCredential = userCredential;
        await RefreshToken();

        return userCredential.Token;
    }

    private async Task<bool> RefreshToken()
    {
        if (UserCredential != null && IsTokenExpired(UserCredential.Token))
        {
            await GoogleWebAuthorizationBroker.ReauthorizeAsync(UserCredential, default);
            return true;
        }
            
        return false;

        bool IsTokenExpired(TokenResponse token)
        {
            DateTime expirationTime = token.IssuedUtc.AddSeconds(token.ExpiresInSeconds!.Value);

            if (DateTime.UtcNow.AddMinutes(+10) >= expirationTime)
                return true;

            return false;
        }
    }
}