using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.PhotoManagement;
using Microsoft.Extensions.Options;

namespace datingApp.Infrastructure.PhotoManagement;

internal sealed class PhotoService : IPhotoService
{
    private readonly IOptions<PhotoServiceOptions> _options;
    public PhotoService(IOptions<PhotoServiceOptions> options)
    {
        _options = options;
    }

    public string SavePhoto(byte[] photo, string extension)
    {
        // TODO: add extension
        string fileName = System.IO.Path.GetRandomFileName();
        string filePath = System.IO.Path.Combine(_options.Value.StoragePath, fileName);
        BuildPath(_options.Value.StoragePath);
        System.IO.File.WriteAllBytes(filePath, photo);
        return filePath;
    }

    private void BuildPath(string path)
    {
        if (!System.IO.Directory.Exists(path))
        {
            System.IO.Directory.CreateDirectory(path);
        }
    }
}