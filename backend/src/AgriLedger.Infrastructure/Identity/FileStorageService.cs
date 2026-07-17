using AgriLedger.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace AgriLedger.Infrastructure.Identity;

// Local disk implementation of receipt/bill storage. The interface is deliberately
// storage-agnostic so this can be swapped for Azure Blob Storage or AWS S3 in
// production (per the spec's Future Enhancements) without touching ReceiptService.
public class FileStorageService : IFileStorageService
{
    private readonly IConfiguration _config;
    private readonly string _rootPath;
    private readonly long _maxFileSizeBytes;
    private readonly HashSet<string> _allowedExtensions;

    public FileStorageService(IConfiguration config)
    {
        _config = config;
        _rootPath = config["FileStorage:RootPath"] ?? "wwwroot/uploads";
        _maxFileSizeBytes = long.Parse(config["FileStorage:MaxFileSizeMb"] ?? "10") * 1024 * 1024;
        _allowedExtensions = (config.GetSection("FileStorage:AllowedExtensions").Get<string[]>()
            ?? new[] { ".jpg", ".jpeg", ".png", ".pdf" })
            .Select(e => e.ToLowerInvariant()).ToHashSet();
    }

    public async Task<(string fileName, string relativePath, long sizeBytes)> SaveAsync(IFormFile file, string subFolder)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("No file was uploaded.");

        if (file.Length > _maxFileSizeBytes)
            throw new InvalidOperationException($"File exceeds the maximum allowed size of {_maxFileSizeBytes / (1024 * 1024)} MB.");

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(extension))
            throw new InvalidOperationException($"File type '{extension}' is not allowed. Allowed types: {string.Join(", ", _allowedExtensions)}.");

        var safeFileName = $"{Guid.NewGuid():N}{extension}";
        var folder = Path.Combine(_rootPath, subFolder);
        Directory.CreateDirectory(folder);
        var fullPath = Path.Combine(folder, safeFileName);

        // NOTE: for production-grade validation, also sniff the file's magic bytes
        // here to confirm the content actually matches the claimed extension.
        await using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var relativePath = Path.Combine(subFolder, safeFileName).Replace("\\", "/");
        return (file.FileName, relativePath, file.Length);
    }

    public string GetPublicUrl(string relativePath) => $"/uploads/{relativePath}";
}
