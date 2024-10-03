using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Services;
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
        return Exists(System.IO.Path.Combine(_storageOptions.Value.StoragePath, $"{fileId}.{extension}"));
    }

    public void SaveFile(byte[] fileContent, string fileId, string extension)
    {
        var fullPath = System.IO.Path.GetFullPath(_storageOptions.Value.StoragePath);
        BuildPath(fullPath);

        string fileName = $"{fileId}.{extension}";
        string filePath = System.IO.Path.Combine(_storageOptions.Value.StoragePath, fileName);

        try
        {
            System.IO.File.WriteAllBytes(filePath, fileContent);
        }
        catch (Exception ex)
        {
            var error = $"{nameof(FileStorageService)}: Failed to save file to disk: id: {fileId}, path: {filePath}.";
            _logger.LogError(ex, error);
        }
    }

    public byte[] GetFile(string fileId, string extension)
    {
        var filePath = System.IO.Path.Combine(_storageOptions.Value.StoragePath, $"{fileId}.{extension}");
        if (Exists(filePath))
        {
            return System.IO.File.ReadAllBytes(filePath);
        }
        var error = $"{nameof(FileStorageService)}: File id {fileId} not found.";
        _logger.LogError(error);
        return null;
    }
 
    public void DeleteFile(string fileId)
    {
        var files = new System.IO.DirectoryInfo(_storageOptions.Value.StoragePath).GetFiles($"{fileId}.*");
        if (files.Any())
        {
            foreach (var file in files)
            {
                try
                {
                    System.IO.File.Delete(file.FullName);
                }
                catch (Exception ex)
                {
                    var error = $"{nameof(FileStorageService)}: Failed to delete file: id: {fileId}, path: {file.FullName}.";
                    _logger.LogError(ex, error);
                }
            }
        }
    }

    public void DeleteFile(string fileId, string extension)
    {
        string filePath = System.IO.Path.Combine(_storageOptions.Value.StoragePath, $"{fileId}.{extension}");
        if (Exists(filePath))
        {
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

    private static bool Exists(string path)
    {
        return System.IO.File.Exists(path);
    }
}
