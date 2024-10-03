using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.Extensions.Logging.TraceSource;

namespace datingApp.Core.Entities;

public class PhotoFile
{
    public Guid PhotoId { get; }
    public string Extension { get; private set; }
    public byte[] Content { get; private set; }
    private const int MinPhotoSizeBytes = 10240;
    private const int MaxPhotoSizeBytes = 2621440;
    private readonly IDictionary<byte[], string> _knownFileHeaders = new Dictionary<byte[], string>()
    {
        {new byte[] {0x42, 0x4D}, "bmp"},
        {new byte[] {0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A}, "png"},
        {new byte[] {0xFF, 0xD8, 0xFF}, "jpg"},
    };

    private PhotoFile()
    {
        // EF
    }

    public PhotoFile(Guid photoId, byte[] content)
    {
        PhotoId = photoId;
        SetContent(content);
    }

    public PhotoFile(Guid photoId, string base64Content)
    {
        PhotoId = photoId;
        SetContent(base64Content);
    }

    private void SetContent(byte[] content)
    {
        if (content == null)
        {
            throw new EmptyPhotoContentException();
        }
        if (Content == content) return;

        int minPhotoSizeKB = MinPhotoSizeBytes / 1024;
        int maxPhotoSizeMB = MaxPhotoSizeBytes / (1024*1024);

        if (!IsValidContentSize(content))
        {
            throw new InvalidPhotoSizeException(minPhotoSizeKB, maxPhotoSizeMB);
        }

        var extension = GetImageFileFormat(content);
        if (string.IsNullOrEmpty(extension))
        {
            throw new InvalidPhotoException();
        }

        Extension = extension;
        Content = content;
    }

    private void SetContent(string base64Content)
    {
        if (string.IsNullOrEmpty(base64Content))
        {
            throw new EmptyPhotoContentException();
        }

        int minPhotoSizeKB = MinPhotoSizeBytes / 1024;
        int maxPhotoSizeMB = MaxPhotoSizeBytes / (1024*1024);

        if (!IsValidBase64ContentSize(base64Content))
        {
            throw new InvalidPhotoSizeException(minPhotoSizeKB, maxPhotoSizeMB);
        }

        byte[] bytes = Base64ToArrayOfBytes(base64Content);
        if (bytes == null)
        {
            throw new FailToConvertBase64StringToArrayOfBytesException();
        }
        SetContent(bytes);
    }

    private static bool IsValidContentSize(byte[] content)
    {
        if (content.Length > MaxPhotoSizeBytes || content.Length < MinPhotoSizeBytes)
        {
            return false;
        }
        return true;
    }

    private static bool IsValidBase64ContentSize(string base64Content)
    {
        int minBase64PhotoSizeBytes = (int) Math.Ceiling(1.5 * MinPhotoSizeBytes);
        int maxBase64PhotoSizeBytes = (int) Math.Ceiling(1.5 * MaxPhotoSizeBytes);

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