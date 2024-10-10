using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using datingApp.Application.PhotoManagement;
using datingApp.Application.Services;
using datingApp.Core.Exceptions;
using datingApp.Infrastructure.Exceptions;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Security.Cryptography;

namespace datingApp.Infrastructure.Services;

internal sealed class PhotoService : IPhotoService
{
    private readonly IOptions<PhotoServiceOptions> _options;

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
        if (bytes.Length < _options.Value.MinPhotoSizeBytes || bytes.Length > _options.Value.MaxPhotoSizeBytes)
        {
            return false;
        }
        return true;
    }

    private bool IsValidContentSize(Stream stream)
    {
        if (stream.Length < _options.Value.MinPhotoSizeBytes || stream.Length > _options.Value.MaxPhotoSizeBytes)
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

    private static Stream Base64ToMemoryStream(string base64Content)
    {
        try
        {
            // https://stackoverflow.com/questions/31524343/how-to-convert-base64-value-from-a-database-to-a-stream-with-c-sharp
            return new MemoryStream(Convert.FromBase64String(base64Content));
        }
        catch (System.Exception)
        {
            return null;
        }
    }

    private string GetImageFileFormat(byte[] content)
    {
        try
        {
            var image = Image.FromStream(new MemoryStream(content));
            switch (image.RawFormat)
            {
                case ImageFormat.Bmp:
                    return "bmp";
                    break;
                case ImageFormat.Jpeg:
                    return "jpg";
                    break;
                case ImageFormat.Png:
                    return "png";
                    break;
                default:
                    return null;
            }
        }
        catch (System.Exception)
        {
            return null;
        }
    }
}