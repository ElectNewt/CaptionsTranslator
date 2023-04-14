using System.Text;
using CaptionsTranslator.Shared.Dtos;

namespace CaptionsTranslator.Core.Services;

public interface ICaptionService
{
    Task<CaptionFile> RetrieveCaptions(string directory, string fileName);
    List<Caption> ConvertIntoCaptions(string content);
    List<Caption> MissingCaptions(List<Caption> translatedCaptions);
    string ConvertIntoString(List<Caption> translated);
}

public class CaptionService : ICaptionService
{
    private readonly IFileService _fileService;
    private CaptionFile? _captionFile { get; set; }

    public CaptionService(IFileService fileService)
    {
        _fileService = fileService;
    }

    public async Task<CaptionFile> RetrieveCaptions(string directory, string fileName)
    {
        string fileContent = await _fileService.LoadCaptionFile(directory, fileName);

        var content = fileContent.Split("\r\n\r\n")
            .Select(a =>
            {
                var v1 = a.Split("\r\n", 3);
                return new Caption()
                {
                    Number = v1[0].Trim(),
                    Time = v1[1],
                    Content = v1[2]
                };
            })
            .ToList();

        _captionFile = new CaptionFile()
        {
            Captions = content,
            FileName = fileName
        };

        return _captionFile;
    }

    public List<Caption> ConvertIntoCaptions(string content)
        => content
            .Split("\r\n\r\n")
            .Select(a =>
            {
                var v1 = a.Split("\r\n", 2);
                return new Caption()
                {
                    Number = v1[0].Trim(),
                    Content = v1[1],
                    Time = "From the other document"
                };
            })
            .ToList();


    public List<Caption> MissingCaptions(List<Caption> translatedCaptions)
        => _captionFile.Captions.Where(a => translatedCaptions.All(t => a.Number != t.Number)).ToList();

    
    public string ConvertIntoString(List<Caption> translated)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < _captionFile.Captions.Count; i++)
        {
            sb.AppendLine(_captionFile.Captions[i].Number);
            sb.AppendLine(_captionFile.Captions[i].Time);
            sb.AppendLine(translated[i].Content);
            sb.AppendLine(string.Empty);
        }


        return sb.ToString();
    }
}