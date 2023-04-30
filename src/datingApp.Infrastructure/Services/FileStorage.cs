using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Services;
using Microsoft.Extensions.Options;

namespace datingApp.Infrastructure.Services;

internal sealed class FileStorage : IFileStorage
{
    private readonly IOptions<StorageOptions> _storageOptions;
    private readonly ILogger<IFileStorage> _logger;
    public FileStorage(IOptions<StorageOptions> storageOptions, ILogger<IFileStorage> logger)
    {
        _storageOptions = storageOptions;
        _logger = logger;
    }

    public async Task SaveFileAsync(byte[] file, string identification, string extension)
    {
        BuildPath(_storageOptions.Value.StoragePath);
        string filename = $"{identification}.{extension}";
        string filePath = System.IO.Path.Combine(_storageOptions.Value.StoragePath, filename);
        try
        {
            await System.IO.File.WriteAllBytesAsync(filePath, file);
        }
        catch (Exception ex)
        {
            var error = $"{nameof(FileStorage)}: Failed to save file to disk: Id: {identification}, path: {filePath}.";
            _logger.LogError(ex, error);
        }
    }

    public async Task DeleteFileAsync(string identification)
    {
        var storage = new System.IO.DirectoryInfo(_storageOptions.Value.StoragePath);
        var task = new Task<FileInfo[]>(() => { return storage.GetFiles(identification + ".*"); });
        var files = await task;
        if (files.Count() == 0) return;

        foreach (var file in files)
        {
            try
            {
                file.Delete();
            }
            catch (Exception ex)
            {
                var error = $"{nameof(FileStorage)}: Failed to delete file: Id: {identification}, path: {file.FullName}.";
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