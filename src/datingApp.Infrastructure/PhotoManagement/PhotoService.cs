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
    
    private readonly IDictionary<byte[], string> knownFileHeaders = new Dictionary<byte[], string>()
    {
        {new byte[] {0x42, 0x4D}, "bmp"},
        {new byte[] {0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A}, "png"},
        {new byte[] {0xFF, 0xD8, 0xFF}, "jpg"},
    };

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
        // Returns file extension associated with file format
        // if image file format is not known, returns null
        string ext = null;
        bool match = false;
        foreach (var item in knownFileHeaders)
        {
            for (int i = 0; i < item.Key.Length; i++)
            {
                match = (photo[i] == item.Key[i]);
                if (!match) break;
            }

            if (match)
            {
                ext = item.Value;
                break;
            }
        }

        return ext;
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
        if (GetImageFileFormat(photo) == null)
        {
            throw new InvalidPhotoException();
        }
    }
}