using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Services;

namespace datingApp.Infrastructure.Services;

internal sealed class FileStorage : IFileStorage
{
    private readonly IOptions<StorageOptions> _storageOptions;
    public FileStorage(IOptions<StorageOptions> storageOptions)
    {
        _storageOptions = storageOptions;
    }

    public string SaveFile(byte[] file, string filename, string extension)
    {
        BuildPath(_storageOptions.Value.StoragePath);
        string fileNameWithExt = $"{filename}.{extension}";
        string filePath = System.IO.Path.Combine(_storageOptions.Value.StoragePath, fileNameWithExt);
        System.IO.File.WriteAllBytes(filePath, file);
        return filePath;
    }

    public void DeleteFile(string path)
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

}