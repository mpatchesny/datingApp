using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.IO;
using datingApp.Application.PhotoManagement;
using datingApp.Application.Services;
using datingApp.Core.Exceptions;
using datingApp.Infrastructure.Exceptions;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Security.Cryptography;
using FluentStorage.Utils.Extensions;

namespace datingApp.Infrastructure.Services;

internal sealed class PhotoService : IPhotoService
{
    private readonly IOptions<PhotoServiceOptions> _options;
    // https://stackoverflow.com/questions/56588900/how-to-validate-uploaded-file-in-asp-net-core
    private static readonly Dictionary<string, List<byte[]>> _fileSignatures = new()
    {
        { ".png", new List<byte[]> { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } } },
        { ".bmp", new List<byte[]> { new byte[] { 0x42, 0x4D } } },
        { ".jpeg", new List<byte[]>
            {
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xEE },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xDB },
            }
        },
        { ".jpeg2000", new List<byte[]> { new byte[] { 0x00, 0x00, 0x00, 0x0C, 0x6A, 0x50, 0x20, 0x20, 0x0D, 0x0A, 0x87, 0x0A } } },
        { ".jpg", new List<byte[]>
            {
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xEE },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xDB },
            }
        }
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
        int minBase64PhotoSizeBytes = (int) Math.Ceiling(1.5 * _options.Value.MinPhotoSizeBytes);
        int maxBase64PhotoSizeBytes = (int) Math.Ceiling(1.5 * _options.Value.MaxPhotoSizeBytes);

        if (!IsValidBase64ContentSize(base64content, minBase64PhotoSizeBytes, maxBase64PhotoSizeBytes))
        {
            throw new InvalidPhotoSizeException(minPhotoSizeKB, maxPhotoSizeMB);
        }

        var stream = Base64ToMemoryStream(base64content);
        if (stream == null)
        {
            throw new FailToConvertBase64StringToArrayOfBytesException();
        }

        if (!IsValidContentSize(stream, _options.Value.MinPhotoSizeBytes, _options.Value.MaxPhotoSizeBytes))
        {
            throw new InvalidPhotoSizeException(minPhotoSizeKB, maxPhotoSizeMB);
        }

        var extension = GetImageFileFormat(stream);
        if (string.IsNullOrEmpty(extension))
        {
            throw new InvalidPhotoException();
        }

        return new PhotoServiceProcessOutput(stream.ToByteArray(), extension);
    }

    private static bool IsValidContentSize(Stream content, long minPhotoSizeBytes, long maxPhotoSizeBytes)
    {
        if (content.Length < minPhotoSizeBytes || content.Length > maxPhotoSizeBytes)
        {
            return false;
        }
        return true;
    }

    private static bool IsValidBase64ContentSize(string base64Content, int minBase64PhotoSizeBytes, int maxBase64PhotoSizeBytes)
    {
        if (base64Content.Length < minBase64PhotoSizeBytes || base64Content.Length > maxBase64PhotoSizeBytes)
        {
            return false;
        }
        return true;
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

    private static string GetImageFileFormat(Stream content)
    {
        // https://stackoverflow.com/questions/56588900/how-to-validate-uploaded-file-in-asp-net-core
        using (var reader = new BinaryReader(content))
        {
            var extensions = _fileSignatures.Keys.ToList();
            var signatures = _fileSignatures.Values.SelectMany(x => x).ToList();

            for (int i = 0; i < signatures.Count; i++)
            {
                var headerBytes = reader.ReadBytes(signatures[i].Length);
                var result = headerBytes.Take(signatures[i].Length).SequenceEqual(signatures[i]);
                if (result)
                {
                    return extensions[i];
                }
            }
        }
        return null;
    }
}