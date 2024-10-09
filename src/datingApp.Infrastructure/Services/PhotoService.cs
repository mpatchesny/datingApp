using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using datingApp.Application.PhotoManagement;
using datingApp.Application.Services;
using datingApp.Core.Exceptions;
using datingApp.Infrastructure.Exceptions;
using Microsoft.Extensions.Options;

namespace datingApp.Infrastructure.Services;

internal sealed class PhotoService : IPhotoService
{
    private readonly IOptions<PhotoServiceOptions> _options;
    private readonly IDictionary<byte[], string> _knownFileHeaders = new Dictionary<byte[], string>()
    {
        {new byte[] {0x42, 0x4D}, "bmp"},
        {new byte[] {0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A}, "png"},
        {new byte[] {0xFF, 0xD8, 0xFF}, "jpg"},
    };

    public PhotoService(IOptions<PhotoServiceOptions> options)
    {
        _options = options;
    }

    public PhotoServiceProcessOutput ProcessBase64Photo(string base64content)
    {
        if (string.IsNullOrEmpty(base64content))
        {
            throw new EmptyBase64StringException();
        }

        int minPhotoSizeKB = _options.Value.MinPhotoSizeBytes / 1024;
        int maxPhotoSizeMB = _options.Value.MaxPhotoSizeBytes / (1024*1024);

        if (!IsValidBase64ContentSize(base64content))
        {
            throw new InvalidPhotoSizeException(minPhotoSizeKB, maxPhotoSizeMB);
        }

        var bytes = Base64ToArrayOfBytes(base64content);
        if (bytes == null)
        {
            throw new FailToConvertBase64StringToArrayOfBytesException();
        }

        if (!IsValidContentSize(bytes))
        {
            throw new InvalidPhotoSizeException(minPhotoSizeKB, maxPhotoSizeMB);
        }

        var extension = GetImageFileFormat(bytes);
        if (string.IsNullOrEmpty(extension))
        {
            throw new InvalidPhotoException();
        }

        return new PhotoServiceProcessOutput(bytes, extension);
    }

    private bool IsValidContentSize(byte[] bytes)
    {
        if (bytes.Length < _options.Value.MinPhotoSizeBytes || bytes.Length > _options.Value.MinPhotoSizeBytes)
        {
            return false;
        }
        return true;
    }

    private bool IsValidBase64ContentSize(string base64Content)
    {
        int minBase64PhotoSizeBytes = (int) Math.Ceiling(1.5 * _options.Value.MinPhotoSizeBytes);
        int maxBase64PhotoSizeBytes = (int) Math.Ceiling(1.5 * _options.Value.MaxPhotoSizeBytes);
        if (base64Content.Length < minBase64PhotoSizeBytes || base64Content.Length > maxBase64PhotoSizeBytes)
        {
            return false;
        }
        return true;
    }

    private static byte[] Base64ToArrayOfBytes(string base64Content)
    {
        // https://stackoverflow.com/questions/51300523/how-to-use-span-in-convert-tryfrombase64string
        byte[] bytes = new byte[((base64Content.Length * 3) + 3) / 4 -
            (base64Content.Length > 0 && base64Content[^1] == '=' ?
                base64Content.Length > 1 && base64Content[^2] == '=' ?
                    2 : 1 : 0)];

        if (!Convert.TryFromBase64String(base64Content, bytes, out _))
        {
            return null;
        }

        return bytes;
    }

    private string GetImageFileFormat(byte[] content)
    {
        // Returns file extension associated with file format
        // if image file format is not known, returns null
        if (content == null) return null;

        bool match = false;
        foreach (var item in _knownFileHeaders)
        {
            for (int i = 0; i < item.Key.Length; i++)
            {
                match = content[i] == item.Key[i];
                if (!match) break;
            }

            if (match)
            {
                return item.Value;
            }
        }
        return null;
    }
}