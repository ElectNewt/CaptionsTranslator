using System.Text;
using CaptionsTranslator.Shared.Settings;
using OpenAI_API;
using OpenAI_API.Chat;

namespace CaptionsTranslator.Dependencies.OpenAI;

public interface ITranslationService
{
    Task<string> PlainTranslation(string text);
}

public class TranslationService : ITranslationService
{
    private readonly Conversation _conversation;

    public TranslationService(AppSettings appSettings)
    {
        _conversation = InitiateConversation(appSettings);
    }

    public async Task<string> PlainTranslation(string text)
    {
        StringBuilder sb = new StringBuilder();
        string message = "Can you translate the next text into English? please respond only the translation:";
        sb.AppendLine(message);
        sb.AppendLine(text);

        _conversation.AppendUserInput(sb.ToString());
        string response = await _conversation.GetResponseFromChatbotAsync();

        return response;
    }

    private Conversation InitiateConversation(AppSettings appSettings)
    {
        string systemMessage = "You are a translation System.";

        OpenAIAPI api = new OpenAIAPI(appSettings.OpenAiSettings.API_KEY);
        Conversation conversation = api.Chat.CreateConversation();

        conversation.AppendSystemMessage(systemMessage);
        return conversation;
    }
}