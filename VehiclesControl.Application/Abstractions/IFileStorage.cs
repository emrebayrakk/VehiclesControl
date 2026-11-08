namespace VehiclesControl.Application.Abstractions;

public interface IFileStorage
{
    /// <summary>Saves the stream under <paramref name="folder"/> with a random name; returns the relative path.</summary>
    Task<string> SaveAsync(Stream content, string folder, string extension, CancellationToken ct = default);
    Task DeleteAsync(string path);

    /// <summary>URL path the API serves the file from, e.g. "/uploads/3/12/abc.jpg".</summary>
    string GetUrl(string path);
}

public sealed record UploadedFile(Stream Content, string FileName, long Length);
