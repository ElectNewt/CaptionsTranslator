using System.Text;
using CaptionsTranslator.Shared.Dtos;
using CaptionsTranslator.Shared.Settings;
using OpenAI_API;
using OpenAI_API.Chat;

namespace CaptionsTranslator.Core.Services;

public interface ITranslationService
{
    Task<string> TranslateFile(CaptionFile file);
    Task<string> RetrieveMissingTranslations(List<Caption> captions);
}

public class TranslationService : ITranslationService
{
    private readonly Conversation _conversation;

    public TranslationService(AppSettings appSettings)
    {
        OpenAIAPI api = new OpenAIAPI(appSettings.OpenAiSettings.API_KEY);
        _conversation = api.Chat.CreateConversation();
    }

    public async Task<string> TranslateFile(CaptionFile file)
    {
        return $"""
1
example 

2
this is the example

4
which obviously do not work

5
as expected due to the lack
of time in the response 
""";

        StringBuilder responses = new StringBuilder();
        foreach (var caption in file.Captions.Chunk(10).Select((value, iteration) => (value, iteration)))
        {
            string result = await ContinueConversation(_conversation, caption.value);
            responses.AppendLine(result);
        }

        return responses.ToString();
    }

    public async Task<string> RetrieveMissingTranslations(List<Caption> captions)
    {
        return $"""
3
example 3 

6
example 6

7
[music]
""";

        StringBuilder responses = new StringBuilder();
        foreach (var caption in captions.Chunk(10).Select((value, iteration) => (value, iteration)))
        {
            string result = await ContinueConversation(_conversation, caption.value);
            responses.AppendLine(result);
        }

        return responses.ToString();
    }


    private Conversation InitiateConversation()
    {
        // Append system message with instructions for the chat
        _conversation.AppendSystemMessage("You are the a translator that translates text into english, " +
                                          "when you see a number by itself on a line please ignore it, " +
                                          "because its a value control. Please keep in mind that is a translation of a " +
                                          "subtitles file which means that the sentences are related between them, try " +
                                          "to keep the meaning of what is being told more than a 1 to 1 translation");
        return _conversation;
    }


    private async Task<string> ContinueConversation(Conversation conversation, Caption[] captions)
    {
        conversation.AppendUserInput(BuildBatch(captions));
        var response = await conversation.GetResponseFromChatbotAsync();
        Console.WriteLine(response);
        return response;
    }

    private string BuildBatch(Caption[] captions)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var caption in captions)
        {
            sb.AppendLine(caption.Number);
            sb.AppendLine(caption.Content);
        }

        return sb.ToString();
    }
}