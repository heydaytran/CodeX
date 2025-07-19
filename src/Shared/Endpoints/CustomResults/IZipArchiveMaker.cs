using System.IO.Compression;

namespace Endpoints.CustomResults;

public interface IZipArchiveMaker<T>
{
    Task WriteToAsync(ZipArchive archive, T value, CancellationToken cancellationToken);
}