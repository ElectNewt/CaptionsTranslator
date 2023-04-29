using CaptionsTranslator.Dependencies.YouTube;
using Microsoft.AspNetCore.Components;
using System.Text.RegularExpressions;
using System.Web;

namespace CaptionsTranslator.Desktop.Pages;

public partial class YouTube
{
	public string VideoId { get; set; }
	public string VideoBoxVisibility { get; set; } = "invisible";
	public string LoadingVisibility { get; set; } = "invisible";
	public string ContentVisibility { get; set; } = "invisible";

	public string TitleContent { get; set; } = "";
	public MarkupString DescriptionContent { get; set; } = new MarkupString();

	[Inject]
	public IYouTube _youTube { get; set; }

	async Task LoadYTInformation()
	{
		VideoBoxVisibility = "visible";
		LoadingVisibility = "visible";

		ContentVisibility = "invisible";

		try
		{
			(string Title, string Description) result = await _youTube.GetVideoInformation(VideoId);
			TitleContent = result.Title;
			DescriptionContent = (MarkupString)Regex.Replace(
								HttpUtility.HtmlEncode(result.Description), "\r?\n|\r", "<br />");
		}
		catch (Exception e)
		{
			TitleContent = "video not found please see details of the error in the description";
			DescriptionContent = new MarkupString(e.Message);
		}



		LoadingVisibility = "invisible";
		ContentVisibility = "visible";
	}



}