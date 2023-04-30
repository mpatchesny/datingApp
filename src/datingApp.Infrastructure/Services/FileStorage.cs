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
    public FileStorage(IOptions<StorageOptions> storageOptions)
    {
        _storageOptions = storageOptions;
    }

    public async Task SaveFileAsync(byte[] file, string identification, string extension)
    {
        BuildPath(_storageOptions.Value.StoragePath);
        string filename = $"{identification}.{extension}";
        string filePath = System.IO.Path.Combine(_storageOptions.Value.StoragePath, filename);
        System.IO.File.WriteAllBytes(filePath, file);
    }

    public async Task DeleteFileAsync(string identification)
    {
        var storage = new System.IO.DirectoryInfo(_storageOptions.Value.StoragePath);
        FileInfo[] files = storage.GetFiles(identification + ".*");
        if (files.Count() == 0) return;

        foreach (var file in files)
        {
            try
            {
                file.Delete();
            }
            catch (System.Exception)
            {
                // pass
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