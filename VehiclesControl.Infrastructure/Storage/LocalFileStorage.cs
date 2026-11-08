using Microsoft.Extensions.Options;
using VehiclesControl.Application.Abstractions;

namespace VehiclesControl.Infrastructure.Storage;

public sealed class FileStorageOptions
{
    public const string SectionName = "FileStorage";

    /// <summary>Folder on disk. Defaults to the API's wwwroot/uploads, which is served at /uploads.</summary>
    public string RootPath { get; set; } = string.Empty;
    public string RequestPath { get; set; } = "/uploads";
}

/// <summary>Stores files on local disk under random names. Swap for blob storage in production if needed.</summary>
public sealed class LocalFileStorage(IOptions<FileStorageOptions> options) : IFileStorage
{
    private readonly FileStorageOptions _options = options.Value;

    public async Task<string> SaveAsync(Stream content, string folder, string extension, CancellationToken ct = default)
    {
        var safeFolder = string.Join('/', folder.Split('/', StringSplitOptions.RemoveEmptyEntries)
            .Where(part => part.All(char.IsLetterOrDigit)));
        var relative = $"{safeFolder}/{Guid.NewGuid():N}{extension}";
        var fullPath = Resolve(relative);

        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        await using var file = File.Create(fullPath);
        await content.CopyToAsync(file, ct);
        return relative;
    }

    public Task DeleteAsync(string path)
    {
        var fullPath = Resolve(path);
        if (File.Exists(fullPath))
            File.Delete(fullPath);
        return Task.CompletedTask;
    }

    public string GetUrl(string path) => $"{_options.RequestPath.TrimEnd('/')}/{path}";

    private string Resolve(string relative)
    {
        var root = Path.GetFullPath(_options.RootPath);
        var full = Path.GetFullPath(Path.Combine(root, relative));
        if (!full.StartsWith(root, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Invalid storage path.");
        return full;
    }
}
