using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.PhotoManagement;
using datingApp.Infrastructure.Exceptions;
using Microsoft.Extensions.Options;

namespace datingApp.Infrastructure.PhotoManagement;

internal sealed class PhotoService : IPhotoService
{
    private readonly IOptions<PhotoServiceOptions> _options;
    public PhotoService(IOptions<PhotoServiceOptions> options)
    {
        _options = options;
    }

    public byte[] ConvertToArrayOfBytes(string Base64Bytes)
    {
        // https://stackoverflow.com/questions/51300523/how-to-use-span-in-convert-tryfrombase64string
        byte[] bytes = new byte[((Base64Bytes.Length * 3) + 3) / 4 -
            (Base64Bytes.Length > 0 && Base64Bytes[Base64Bytes.Length - 1] == '=' ?
                Base64Bytes.Length > 1 && Base64Bytes[Base64Bytes.Length - 2] == '=' ?
                    2 : 1 : 0)];

        if (!Convert.TryFromBase64String(Base64Bytes, bytes, out int bytesWritten))
        {
            throw new FailToConvertBase64StringToArrayOfBytes();
        }

        return bytes;
    }

    public string GetImageFileFormat(byte[] photo)
    {
        throw new NotImplementedException();
    }

    public string SavePhoto(byte[] photo, string extension)
    {
        string fileName = $"{System.IO.Path.GetRandomFileName()}.{extension}";
        string filePath = System.IO.Path.Combine(_options.Value.StoragePath, fileName);
        BuildPath(_options.Value.StoragePath);
        System.IO.File.WriteAllBytes(filePath, photo);
        return filePath;
    }

    public void ValidatePhoto(byte[] photo)
    {
        int maxPhotoSizeMB = _options.Value.MaxPhotoSizeBytes / (1024*1024);
        int minPhotoSizeKB = _options.Value.MinPhotoSizeBytes / 1024;

        if (photo.Count() > _options.Value.MaxPhotoSizeBytes)
        {
            throw new InvalidPhotoSizeException(minPhotoSizeKB, maxPhotoSizeMB);
        }
        if (photo.Count() < _options.Value.MinPhotoSizeBytes)
        {
            throw new InvalidPhotoSizeException(minPhotoSizeKB, maxPhotoSizeMB);
        }

        // TODO: is a valid photo?
    }

    private void BuildPath(string path)
    {
        if (!System.IO.Directory.Exists(path))
        {
            System.IO.Directory.CreateDirectory(path);
        }
    }
}