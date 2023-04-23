using System.Text;
using CaptionsTranslator.Shared.Dtos;
using CaptionsTranslator.Shared.Settings;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;

namespace CaptionsTranslator.Dependencies.OpenAI;

public interface ISubtitleTranslationService
{
    Task<string> TranslateFile(CaptionFile file);
    Task<string> RetrieveMissingTranslations(List<Caption> captions);
    
}

public class SubtitleSubtitleTranslationService : ISubtitleTranslationService
{
    private Conversation Conversation => _maybeConversation ??= InitiateConversation(_appSettings);
    private readonly AppSettings _appSettings;
    private Conversation? _maybeConversation;

    public SubtitleSubtitleTranslationService(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }
    public async Task<string> TranslateFile(CaptionFile file)
    {
        StringBuilder responses = new StringBuilder();
        foreach (var caption in file.Captions.Chunk(5).Select((value, iteration) => (value, iteration)))
        {
            string result = await ContinueConversation(caption.value);
            responses.AppendLine(result);
        }

        return responses.ToString();
    }

    public async Task<string> RetrieveMissingTranslations(List<Caption> captions)
    {
        StringBuilder responses = new StringBuilder();
        foreach (var caption in captions.Chunk(5).Select((value, iteration) => (value, iteration)))
        {
            string result = await ContinueConversation(caption.value);
            responses.AppendLine(result);
        }

        return responses.ToString();
    }

    


    private Conversation InitiateConversation(AppSettings appSettings)
    {
        string systemMessage = """
Task: Translate movie subtitles from any language to English based on the instructions
 
Objective: 
- Translate subtitles accurately while maintaining the original meaning and tone of the content.
- each input contains a N number of a combination of ID, and content to translate, you should return the same number of combinations as the input contained. 
 
Roles:
- Linguist: Responsible for translating the subtitles from any known language to English.
 
Strategy: 
- Translate subtitles accurately while maintaining the original meaning and tone of the content.
- Use user feedback and engagement metrics to assess the effectiveness of the translations generated.
 
Instructions:
- User Inputs any language of subtitles they want to translate 
- Detect the Language and Essence of the text
- Generate natural-sounding English translations that accurately convey the meaning of the original text.
- Output all the translations together as separate Prompts, with each Prompt containing the translated subtitles of a single batch.
- Ensure the number of combination of ID, Translations is the same in the output and in the input.
- Check the accuracy and naturalness of the translations before submitting them to the user.
- Do not include the examples in the output when the use is finished. 

Do you have any doubt?
""";
        
        OpenAIAPI api = new OpenAIAPI(appSettings.OpenAiSettings.API_KEY);
        Conversation conversation = api.Chat.CreateConversation(new ChatRequest()
        {
            Temperature = 0.1,
            Model = Model.ChatGPTTurbo
        });
        
        // Append system message with instructions for the chat
        conversation.AppendSystemMessage(systemMessage);
        return conversation;
    }
    
    private async Task<string> ContinueConversation(Caption[] captions)
    {
        Conversation.AppendUserInput(BuildBatch(captions));
        var response = await Conversation.GetResponseFromChatbotAsync();
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