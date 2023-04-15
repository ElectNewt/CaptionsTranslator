using CaptionsTranslator.Shared.Settings;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace CaptionsTranslator.Dependencies.YouTube;

public interface IYouTube
{
    Task<string> DownloadCaptionFile(string videoId, string format = "srt");
    Task<bool> UploadTranslatedFile(string videoId, string format = "srt");
    Task<(string Title, string Description)> GetVideoInformation(string videoId);
    Task<bool> UpdateVideoLocalization(string videoId, string title, string description, string languageCode = "en");
}

public class YouTubeImplementation : IYouTube
{
    private readonly IYouTubeClientFactory _youTubeClientFactory;
    private readonly AppSettings _settings;

    public YouTubeImplementation(IYouTubeClientFactory youTubeClientFactory, AppSettings settings)
    {
        _youTubeClientFactory = youTubeClientFactory;
        _settings = settings;
    }


    /// <summary>
    /// Downloads a file from youtube into the destination folder.
    /// </summary>
    /// <returns>filename</returns>
    public async Task<string> DownloadCaptionFile(string videoId, string format = "srt")
    {
        var captionInfo = await GetCaptionInformation(videoId);

        YouTubeService client = await _youTubeClientFactory.GetYouTubeClient();
        CaptionsResource.DownloadRequest? downloadRequest = client.Captions.Download(captionInfo.CaptionId);
        downloadRequest.Tfmt = format;
        downloadRequest.Tlang = captionInfo.Language;

        MemoryStream memoryStream = new MemoryStream();
        downloadRequest.Download(memoryStream);
        await downloadRequest.DownloadAsync(memoryStream);

        await File.WriteAllBytesAsync($@"{_settings.TranslationSettings.OriginalFolder}\\{videoId}.{format}", memoryStream.ToArray());
        return $"{videoId}.{format}";
    }

    public async Task<bool> UploadTranslatedFile(string videoId, string format = "srt")
    {
        YouTubeService client = await _youTubeClientFactory.GetYouTubeClient();
        var snippet = new CaptionSnippet()
        {
            Language = "en",
            VideoId = videoId,
            Name = "English Subtitles",
            IsDraft = false
        };
        Caption caption = new Caption
        {
            Snippet = snippet
        };

        await using var fileStream = new FileStream($@"{_settings.TranslationSettings.TranslationFolder}\\{videoId}.{format}", FileMode.Open);
        //create the request now and insert our params...
        var captionRequest = client.Captions.Insert(caption, "snippet", fileStream, "application/x-subrip");

        //finally upload the request... and wait.
        await captionRequest.UploadAsync();

        return true;
    }

    public async Task<(string Title, string Description)> GetVideoInformation(string videoId)
    {
        YouTubeService client = await _youTubeClientFactory.GetYouTubeClient();
        var request = client.Videos.List("snippet");
        request.Id = videoId;
        var response =await request.ExecuteAsync();

        var videoSnippet = response.Items.First().Snippet;

        return (videoSnippet.Title, videoSnippet.Description);
    }

    public async Task<bool> UpdateVideoLocalization(string videoId, string title, string description, string languageCode = "en")
    {
        YouTubeService client = await _youTubeClientFactory.GetYouTubeClient();
        VideosResource.UpdateRequest? updateRequest = client.Videos.Update(new Video()
        {
            Id = videoId,
            Localizations = new Dictionary<string, VideoLocalization>()
            {
                {
                    languageCode, new VideoLocalization()
                    {
                        Description = description,
                        Title = title
                    }
                }
            }
        }, "localizations");

        await updateRequest.ExecuteAsync();

        return true;
    }

    private async Task<(string CaptionId, string Language)> GetCaptionInformation(string videoId)
    {
        YouTubeService client = await _youTubeClientFactory.GetYouTubeClient();
        //This is how to get the Id of the caption track.
        var listCaptionRequest = client.Captions.List("snippet", videoId);
        var result = await listCaptionRequest.ExecuteAsync();
        return result.Items.Select(a => (a.Id, a.Snippet.Language)).First();
    }
}