using CaptionsTranslator.Shared.Settings;


namespace CaptionsTranslator.Desktop
{

	public interface IAppState
	{
		OpenAiSettings OpenAiSettings { get; }
		TranslationSettings TranslationSettings { get; }
		YouTubeSettings YouTubeSettings { get; }

		void SaveConfiguration(OpenAiSettings openAiSettings, TranslationSettings translationSettings, YouTubeSettings youTubeSettings);

	}


	public class AppState : IAppState
	{
		public OpenAiSettings OpenAiSettings { get; private set; }
		public TranslationSettings TranslationSettings { get; private set; }
		public YouTubeSettings YouTubeSettings { get; private set; }

		public AppState()
		{
			LoadConfigurationFromEnvironmentVariables();
		}

		private void LoadConfigurationFromEnvironmentVariables()
		{
			OpenAiSettings = new OpenAiSettings
			{
				API_KEY = Environment.GetEnvironmentVariable("OpenAiSettings:API_KEY"),
				Model = Environment.GetEnvironmentVariable("OpenAiSettings:Model")
			};

			TranslationSettings = new TranslationSettings
			{
				OriginalFolder = Environment.GetEnvironmentVariable("TranslationSettings:OriginalFolder"),
				PlainTranslation = Environment.GetEnvironmentVariable("TranslationSettings:PlainTranslation"),
				TranslationFolder = Environment.GetEnvironmentVariable("TranslationSettings:TranslationFolder")
			};

			YouTubeSettings = new YouTubeSettings
			{
				ApplicationName = Environment.GetEnvironmentVariable("YouTubeSettings:ApplicationName"),
				OAuth2ClientId = Environment.GetEnvironmentVariable("YouTubeSettings:OAuth2ClientId"),
				OAuth2ClientSecret = Environment.GetEnvironmentVariable("YouTubeSettings:OAuth2ClientSecret")
			};

		}

		public void SaveConfiguration(OpenAiSettings openAiSettings, TranslationSettings translationSettings, YouTubeSettings youTubeSettings)
		{
			//TODO: change the way I do this, because I don't want to need admin permissions on the app
			Environment.SetEnvironmentVariable("OpenAiSettings:API_KEY", openAiSettings.API_KEY, EnvironmentVariableTarget.Machine);
			Environment.SetEnvironmentVariable("OpenAiSettings:Model", openAiSettings.Model, EnvironmentVariableTarget.Machine);
			OpenAiSettings = openAiSettings;
			Environment.SetEnvironmentVariable("TranslationSettings:OriginalFolder", translationSettings.OriginalFolder, EnvironmentVariableTarget.Machine);
			Environment.SetEnvironmentVariable("TranslationSettings:TranslationFolder", translationSettings.TranslationFolder, EnvironmentVariableTarget.Machine);
			Environment.SetEnvironmentVariable("TranslationSettings:PlainTranslation", translationSettings.PlainTranslation, EnvironmentVariableTarget.Machine);
			TranslationSettings = translationSettings;
			Environment.SetEnvironmentVariable("YouTubeSettings:ApplicationName", youTubeSettings.ApplicationName, EnvironmentVariableTarget.Machine);
			Environment.SetEnvironmentVariable("YouTubeSettings:OAuth2ClientId", youTubeSettings.OAuth2ClientId, EnvironmentVariableTarget.Machine);
			Environment.SetEnvironmentVariable("YouTubeSettings:OAuth2ClientSecret", youTubeSettings.OAuth2ClientSecret, EnvironmentVariableTarget.Machine);
			YouTubeSettings = youTubeSettings;
		}
	}
}
