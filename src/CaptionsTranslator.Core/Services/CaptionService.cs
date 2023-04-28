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

        var content = fileContent.Split(new string[] { "\n\n", "\r\n\r\n" },
                StringSplitOptions.None)
            .Select(a =>
            {
                var v1 = a.Split("\n", 3);
                if (v1.Length != 3)
                    return new Caption() { Number = 0, Time = "", Content = "" };

                return new Caption()
                {
                    Number = !string.IsNullOrWhiteSpace(v1[0].Trim()) ? Int32.Parse(v1[0].Trim()) : 0,
                    Time = v1[1],
                    Content = v1[2]
                };
            })
            .Where(a => a.Number != 0)
            //This is a bug on the youtube download that contains the captions duplicated
            .GroupBy(a => a.Number)
            .Select(a => a.First())
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
            .Split(new string[] { "\n\n", "\r\n\r\n" },
                StringSplitOptions.None)
            .Select(a =>
            {
                var v1 = a.Split("\n", 2);

                return new Caption()
                {
                    Number = !string.IsNullOrWhiteSpace(v1[0].Trim()) ? Int32.Parse(v1[0].Trim()) : 0,
                    Content = v1.Length >= 2 ? v1[1] : string.Empty,
                    Time = "From the other document"
                };
            })
            .ToList();


    public List<Caption> MissingCaptions(List<Caption> translatedCaptions)
        => _captionFile?.Captions.Where(a => translatedCaptions.All(t => a.Number != t.Number)).ToList() ?? throw new InvalidOperationException();


    public string ConvertIntoString(List<Caption> translated)
    {
        StringBuilder sb = new StringBuilder();
        if (_captionFile == null) return sb.ToString();

        foreach (Caption caption in _captionFile.Captions)
        {
            sb.AppendLine(caption.Number.ToString());
            sb.AppendLine(caption.Time);
            sb.AppendLine(translated.FirstOrDefault(a => a.Number == caption.Number)?.Content ?? string.Empty);
            sb.AppendLine(string.Empty);
        }

        return sb.ToString();
    }
}