using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace datingApp.Infrastructure.Services;

internal sealed class FileStorageOptions
{
    private readonly IOptions<StorageOptions> _storageOptions;
    private readonly ILogger<FileStorageOptions> _logger;
    public FileStorageOptions(IOptions<StorageOptions> storageOptions, ILogger<FileStorageOptions> logger)
    {
        _storageOptions = storageOptions;
        _logger = logger;
    }

    public bool Exists(string identification)
    {
        var storage = new System.IO.DirectoryInfo(_storageOptions.Value.StoragePath);
        var files = storage.GetFiles(identification + ".*");
        return files.Length>0;
    }

    public void SaveFile(byte[] file, string identification, string extension)
    {
        BuildPath(_storageOptions.Value.StoragePath);
        string filename = $"{identification}.{extension}";
        string filePath = System.IO.Path.Combine(_storageOptions.Value.StoragePath, filename);
        try
        {
            System.IO.File.WriteAllBytes(filePath, file);
        }
        catch (Exception ex)
        {
            var error = $"{nameof(FileStorageOptions)}: Failed to save file to disk: Id: {identification}, path: {filePath}.";
            _logger.LogError(ex, error);
        }
    }

    public byte[] GetFile(string identification)
    {
        var storage = new System.IO.DirectoryInfo(_storageOptions.Value.StoragePath);
        var files = storage.GetFiles(identification + ".*");
        if (files.Length > 0)
        {
            return System.IO.File.ReadAllBytes(files[0].FullName);
        }
        var error = $"{nameof(FileStorageOptions)}: File with id: {identification} not found.";
        _logger.LogError(error);
        return null;
    }

    public void DeleteFile(string identification)
    {
        var storage = new System.IO.DirectoryInfo(_storageOptions.Value.StoragePath);
        var files = storage.GetFiles(identification + ".*");
        if (files.Count() == 0) return;

        foreach (var file in files)
        {
            try
            {
                file.Delete();
            }
            catch (Exception ex)
            {
                var error = $"{nameof(FileStorageOptions)}: Failed to delete file: Id: {identification}, path: {file.FullName}.";
                _logger.LogError(ex, error);
            }
        }
    }

    private void BuildPath(string path)
    {
        if (!System.IO.Directory.Exists(path))
        {
            System.IO.Directory.CreateDirectory(path);
        }
    }
}