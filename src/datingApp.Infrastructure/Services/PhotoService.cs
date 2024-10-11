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

        uint minPhotoSizeKB = _options.Value.MinPhotoSizeBytes / 1024;
        uint maxPhotoSizeMB = _options.Value.MaxPhotoSizeBytes / (1024*1024);
        uint minBase64PhotoSizeBytes = Base64Length(_options.Value.MinPhotoSizeBytes);
        uint maxBase64PhotoSizeBytes = Base64Length(_options.Value.MaxPhotoSizeBytes);

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

    private static bool IsValidContentSize(Stream content, uint minPhotoSizeBytes, uint maxPhotoSizeBytes)
    {
        if (content.Length < minPhotoSizeBytes || content.Length > maxPhotoSizeBytes)
        {
            return false;
        }
        return true;
    }

    private static bool IsValidBase64ContentSize(string base64Content, uint minBase64PhotoSizeBytes, uint maxBase64PhotoSizeBytes)
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

    private static uint Base64Length(uint originalLength)
    {
        // https://stackoverflow.com/questions/13378815/base64-length-calculation
        return ((originalLength + 3 - 1) / 3) * 4;
    }
    private static string GetImageFileFormat(Stream content)
    {
        // FIXME: use ImageFlow
        return null;
    }
}