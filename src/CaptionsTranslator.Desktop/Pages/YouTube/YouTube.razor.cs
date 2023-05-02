using CaptionsTranslator.Dependencies.OpenAI;
using CaptionsTranslator.Dependencies.YouTube;
using Microsoft.AspNetCore.Components;
using System.Text.RegularExpressions;
using System.Web;

namespace CaptionsTranslator.Desktop.Pages.YouTube;

public partial class YouTube
{
	[Inject] public IYouTube _youTube { get; set; }
	[Inject] public IAppState _appState { get; set; }
	[Inject] public ITranslationService _translationService { get; set; }

	public string VideoId { get; set; }
	public string VideoBoxVisibility { get; set; } = "invisible";
	public string LoadingVisibility { get; set; } = "invisible";
	public string ContentVisibility { get; set; } = "invisible";

	public string TitleContent { get; set; } = string.Empty;
	public string DescriptionContentOriginal { get; set;} = string.Empty;
	public MarkupString DescriptionContent { get; set; } = new MarkupString();

	public string TranslatedTitle { get; set; } = string.Empty;
	public string TranslatedDescriptionOriginal { get; set; } = string.Empty;
	public MarkupString TranslatedDescription { get; set; } = new MarkupString();
	public string LoadingTranslationVisibility = "invisible";
	public string TranslationContentVisibility = "invisible";
	public string TranslationVisibility = "invisible";

	public string UploadedVideoText = String.Empty;

	async Task LoadYouTubeVideoInformation()
	{
		VideoBoxVisibility = "visible";
		LoadingVisibility = "visible";
		ContentVisibility = "invisible";

		try
		{
			(string Title, string Description) result = await _youTube.GetVideoInformation(VideoId);
			TitleContent = result.Title;
			DescriptionContentOriginal = result.Description;
			DescriptionContent = (MarkupString)Regex.Replace(
				HttpUtility.HtmlEncode(DescriptionContentOriginal), "\r?\n|\r", "<br />");
		}
		catch (Exception e)
		{
			TitleContent = "video not found please see details of the error in the description";
			DescriptionContent = new MarkupString(e.Message);
		}

		LoadingVisibility = "invisible";
		ContentVisibility = "visible";
		TranslationVisibility = "visible";
	}

	async Task TranslateTitleAndDescription()
	{
		UploadedVideoText = string.Empty;
		LoadingTranslationVisibility = "visible";
		TranslationContentVisibility = "invisible";
		try
		{
			TranslatedTitle = await _translationService.PlainTranslation(TitleContent, _appState.OpenAiSettings.Model);

			TranslatedDescriptionOriginal = await _translationService.PlainTranslation(DescriptionContentOriginal, _appState.OpenAiSettings.Model);

			TranslatedDescription = (MarkupString)Regex.Replace(
					HttpUtility.HtmlEncode(TranslatedDescriptionOriginal), "\r?\n|\r", "<br />");

		}
		catch (Exception e)
		{
			TranslatedTitle = "video not found please see details of the error in the description";
			TranslatedDescription = new MarkupString(e.Message);
		}
		LoadingTranslationVisibility = "invisible";
		TranslationContentVisibility = "visible";

	}

	async Task UploadToYouTube()
	{
		try
		{
			UploadedVideoText = "uploading";
			await _youTube.UpdateVideoLocalization(VideoId, TranslatedTitle, TranslatedDescriptionOriginal, "en");
			UploadedVideoText = "Completed!";
		}
		catch (Exception e)
		{
			UploadedVideoText = $"Error uploading the video title or description to YouTube: {e.Message} ";
		}

	}
}