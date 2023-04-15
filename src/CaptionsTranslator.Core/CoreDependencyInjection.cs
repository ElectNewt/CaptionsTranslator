using CaptionsTranslator.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CaptionsTranslator.Core;

public static class CoreDependencyInjection
{

    public static IServiceCollection AddCoreServices(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddScoped<IFileService, FileService>()
            .AddScoped<ICaptionService, CaptionService>()
            .AddScoped<ITranslateFileService, TranslateFileService>()
            .AddScoped<IDirectoryService, DirectoryService>();
    }
    
}