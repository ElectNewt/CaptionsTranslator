﻿namespace CaptionsTranslator.Core.Services;

public interface IFileService
{
    Task<string> LoadCaptionFile(string directory, string fileName);
    Task DumpCaptionsIntoFile(string directory, string fileName, string content);
    Task AppendToFile(string directory, string fileName, string content);
}

public class FileService : IFileService
{
    public async Task<string> LoadCaptionFile(string directory, string fileName)
        => await File.ReadAllTextAsync($@"{directory}\{fileName}");

    public async Task DumpCaptionsIntoFile(string directory, string fileName, string content)
        => await File.WriteAllTextAsync($@"{directory}\{fileName}", content);

    public async Task AppendToFile(string directory, string fileName, string content)
		=> await File.AppendAllTextAsync($@"{directory}\{fileName}", content);
}