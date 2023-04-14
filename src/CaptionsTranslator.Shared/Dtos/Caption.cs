namespace CaptionsTranslator.Shared.Dtos;

public record Caption
{
    public required string Number { get; init; }
    public required string Time { get; init; }
    public required string Content { get; init; }
}