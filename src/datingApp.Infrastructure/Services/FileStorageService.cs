using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace datingApp.Infrastructure.Services;

internal sealed class FileStorageService
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
        string filename = $"{fileId}.{extension}";
        string filePath = System.IO.Path.Combine(_storageOptions.Value.StoragePath, filename);
        return File.Exists(filePath);
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
            var error = $"{nameof(FileStorageService)}: Failed to save file to disk: Id: {fileId}, path: {filePath}.";
            _logger.LogError(ex, error);
        }
    }

    public byte[] GetFile(string fileId)
    {
        var storage = new System.IO.DirectoryInfo(_storageOptions.Value.StoragePath);
        var files = storage.GetFiles(fileId + ".*");
        if (files.Length > 0)
        {
            return System.IO.File.ReadAllBytes(files[0].FullName);
        }
        var error = $"{nameof(FileStorageService)}: File with id: {fileId} not found.";
        _logger.LogError(error);
        return null;
    }

    public void DeleteFile(string fileId)
    {
        var storage = new System.IO.DirectoryInfo(_storageOptions.Value.StoragePath);
        var files = storage.GetFiles(fileId + ".*");
        if (files.Length == 0) return;

        foreach (var file in files)
        {
            try
            {
                file.Delete();
            }
            catch (Exception ex)
            {
                var error = $"{nameof(FileStorageService)}: Failed to delete file: Id: {fileId}, path: {file.FullName}.";
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