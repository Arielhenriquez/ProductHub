using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using ProductHub.Application.Interfaces;
using ProductHub.Domain.Settings;

namespace ProductHub.Infrastructure;

public class BlobService : IBlobService
{
    private readonly BlobContainerClient _containerClient;

    public BlobService(IOptions<StorageOptions> options)
    {
        var settings = options.Value ?? throw new ArgumentNullException(nameof(options));

        var blobServiceClient = new BlobServiceClient(settings.ConnectionString);
        _containerClient = blobServiceClient.GetBlobContainerClient(settings.ContainerName);
    }
    public async Task<string> UploadImageAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);

        await _containerClient.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: cancellationToken);

        var extension = Path.GetExtension(file.FileName);
        var blobName = $"{Guid.NewGuid()}{extension}";
        var blobClient = _containerClient.GetBlobClient(blobName);

        await using var stream = file.OpenReadStream();
        var headers = new BlobHttpHeaders { ContentType = file.ContentType ?? "application/octet-stream" };
        await blobClient.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = headers }, cancellationToken);

        return blobClient.Uri.ToString();
    }
}
