using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace datingApp.Infrastructure.Services;

internal sealed class FileStorageService : IFileStorageService
{
    private readonly IOptions<StorageOptions> _storageOptions;
    private readonly ILogger<FileStorageService> _logger;
    public FileStorageService(IOptions<StorageOptions> storageOptions, ILogger<FileStorageService> logger)
    {
        _storageOptions = storageOptions;
        _logger = logger;
    }

    public bool Exists(string fileId, string extension)
    {
        return System.IO.File.Exists(System.IO.Path.Combine(_storageOptions.Value.StoragePath, $"{fileId}.{extension}"));
    }

    public void SaveFile(byte[] file, string fileId, string extension)
    {
        var fullPath = System.IO.Path.GetFullPath(_storageOptions.Value.StoragePath);
        BuildPath(fullPath);

        string filename = $"{fileId}.{extension}";
        string filePath = System.IO.Path.Combine(_storageOptions.Value.StoragePath, filename);

        try
        {
            System.IO.File.WriteAllBytes(filePath, file);
        }
        catch (Exception ex)
        {
            var error = $"{nameof(FileStorageService)}: Failed to save file to disk: id: {fileId}, path: {filePath}.";
            _logger.LogError(ex, error);
        }
    }

    public byte[] GetFile(string fileId, string extension)
    {
        if (Exists(fileId, extension))
        {
            return System.IO.File.ReadAllBytes(System.IO.Path.Combine(_storageOptions.Value.StoragePath, $"{fileId}.{extension}"));
        }
        var error = $"{nameof(FileStorageService)}: File id {fileId} not found.";
        _logger.LogError(error);
        return null;
    }

    public void DeleteFile(string fileId, string extension)
    {
        if (Exists(fileId, extension))
        {
            string filePath = System.IO.Path.Combine(_storageOptions.Value.StoragePath, $"{fileId}.{extension}");
            try
            {
                System.IO.File.Delete(filePath);
            }
            catch (Exception ex)
            {
                var error = $"{nameof(FileStorageService)}: Failed to delete file: id: {fileId}, path: {filePath}.";
                _logger.LogError(ex, error);
            }
        }
    }

    private static void BuildPath(string path)
    {
        if (!System.IO.Directory.Exists(path))
        {
            System.IO.Directory.CreateDirectory(path);
        }
    }
}