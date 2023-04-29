using System.Text;
using CaptionsTranslator.Shared.Dtos;
using CaptionsTranslator.Shared.Settings;
using Microsoft.Extensions.Options;

namespace CaptionsTranslator.Dependencies.OpenAI;

public interface ISubtitleTranslationService
{
    Task<string> TranslateFile(CaptionFile file);
    Task<string> RetrieveMissingTranslations(List<Caption> captions);
    
}

public class SubtitleSubtitleTranslationService : OpenAiBaseClass, ISubtitleTranslationService
{
    protected override string? SystemMessage => """
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
    
    public SubtitleSubtitleTranslationService(IOptions<OpenAiSettings> openAiSettings) : base(openAiSettings)
    {
    }
    
    //TODO: not sure if this batching should be here or in a previous class.
    //Probably I should try to calculate the units and use as max as possible. 
    public async Task<string> TranslateFile(CaptionFile file)
    {
        StringBuilder responses = new StringBuilder();
        foreach (var caption in file.Captions.Chunk(40).Select((value, iteration) => (value, iteration)))
        {
            string result = await ContinueConversation(caption.value);
            responses.AppendLine(result);
            responses.AppendLine(String.Empty);
        }

        return responses.ToString();
    }

    public async Task<string> RetrieveMissingTranslations(List<Caption> captions)
    {
        StringBuilder responses = new StringBuilder();
        foreach (var caption in captions.Chunk(25).Select((value, iteration) => (value, iteration)))
        {
            string result = await ContinueConversation(caption.value);
            responses.AppendLine(result);
        }

        return responses.ToString();
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
            sb.AppendLine(caption.Number.ToString());
            sb.AppendLine(caption.Content);
            sb.AppendLine();
        }

        return sb.ToString();
    }
}