namespace CaptionsTranslator.Shared.Dtos;

public record CaptionFile
{
    public required string FileName { get; init; }
    public required List<Caption> Captions { get; init; }
}