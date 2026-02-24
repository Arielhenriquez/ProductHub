using Microsoft.AspNetCore.Http;

namespace ProductHub.Application.Interfaces;

public interface IBlobService
{
    Task<string> UploadImageAsync(IFormFile file, CancellationToken cancellationToken = default);
}
