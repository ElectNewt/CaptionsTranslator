using System.Text;
using CaptionsTranslator.Shared.Settings;
using Microsoft.Extensions.Options;

namespace CaptionsTranslator.Dependencies.OpenAI;

public interface ITranslationService
{
    Task<string> PlainTranslation(string text);
}

public class TranslationService : OpenAiBaseClass, ITranslationService
{
    protected override string? SystemMessage => "You are a translation System.";
    
    public TranslationService(IOptions<OpenAiSettings> openAiSettings) : base(openAiSettings)
    {
    }

    public async Task<string> PlainTranslation(string text)
    {
        StringBuilder sb = new StringBuilder();
        string message = "Can you translate the next text into English? please respond only the translation:";
        sb.AppendLine(message);
        sb.AppendLine(text);

        Conversation.AppendUserInput(sb.ToString());
        string response = await Conversation.GetResponseFromChatbotAsync();

        return response;
    }
}
