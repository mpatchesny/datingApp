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

    public async Task<string> SaveFileAsync(byte[] file, string filename, string extension)
    {
        BuildPath(_storageOptions.Value.StoragePath);
        string fileNameWithExt = $"{filename}.{extension}";
        string filePath = System.IO.Path.Combine(_storageOptions.Value.StoragePath, fileNameWithExt);
        System.IO.File.WriteAllBytes(filePath, file);
        return filePath;
    }

    public async Task DeleteFileAsync(string path)
    {
        if (System.IO.File.Exists(path))
        {
            try
            {
                System.IO.File.Delete(path);
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