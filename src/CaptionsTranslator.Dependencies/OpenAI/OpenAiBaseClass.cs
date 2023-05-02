using CaptionsTranslator.Shared.Settings;
using Microsoft.Extensions.Options;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;

namespace CaptionsTranslator.Dependencies.OpenAI;

public abstract class OpenAiBaseClass
{
    /// <summary>
    /// contains the Conversation that will be use to chat with ChatGpt
    /// </summary>
    protected Conversation Conversation => _maybeConversation ?? InitiateConversation(ChatGptModel);
    /// <summary>
    /// Optional
    /// Message that will be appended when the conversation is created
    /// </summary>
    protected abstract string? SystemMessage { get; }

    /// <summary>
    /// allows to get a different version of chatGPT model default GTP4
    /// More information about pricing https://openai.com/pricing
    /// </summary>
    protected virtual string ChatGptModel { get; set; } = "default";
    
    /// <summary>
    /// What sampling temperature to use. Higher values means the model will take more risks.
    /// Try 0.9 for more creative applications, and 0 (argmax sampling) for ones with a well-defined answer.
    /// </summary>
    protected virtual double Temperature { get; } = 0.1;

    //TODO: convert into Ioptions
    private readonly OpenAiSettings _openAiSettings;
    private Conversation? _maybeConversation;

    protected OpenAiBaseClass(IOptions<OpenAiSettings> openAiSettings)
    {
        _maybeConversation = null;
        _openAiSettings = openAiSettings.Value;
    }


    private Conversation InitiateConversation(string model)
    {
        OpenAIAPI api = new OpenAIAPI(_openAiSettings.API_KEY);
        _maybeConversation = api.Chat.CreateConversation(new ChatRequest()
        {
            Temperature =Temperature,
            Model = SelectGptModel(model)
		});

        // Append system message with instructions for the chat
        if (!string.IsNullOrWhiteSpace(SystemMessage))
            _maybeConversation.AppendSystemMessage(SystemMessage);
        return _maybeConversation;
    }

    private Model SelectGptModel(string model)
        => model switch
        {
            "4" => Model.GPT4,
            "3.5turbo" => Model.ChatGPTTurbo,
            _ => Model.DefaultModel
        };
}