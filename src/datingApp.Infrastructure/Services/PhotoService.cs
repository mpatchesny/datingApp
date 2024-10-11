using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.IO;
using Imageflow.Fluent;
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
        uint maxPhotoSizeMB = _options.Value.MaxPhotoSizeBytes / 1048576; // 1024 * 1024
        uint minBase64PhotoSizeBytes = GetBase64Length(_options.Value.MinPhotoSizeBytes);
        uint maxBase64PhotoSizeBytes = GetBase64Length(_options.Value.MaxPhotoSizeBytes);

        if (!IsValidBase64ContentSize(base64content, minBase64PhotoSizeBytes, maxBase64PhotoSizeBytes))
        {
            throw new InvalidPhotoSizeException(minPhotoSizeKB, maxPhotoSizeMB);
        }

        var content = Base64ToByteArray(base64content);
        if (content == null)
        {
            throw new FailToConvertBase64StringToArrayOfBytesException();
        }

        try
        {
            var task = ConvertToJpegAsync(content, _options.Value.ImageQuality);
            task.Wait();
            return new PhotoServiceProcessOutput(task.Result, "jpg");
        }
        catch (Exception)
        {
            // couldn't convert to jpg == bad image
            throw new InvalidPhotoException();
        }
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

    private static byte[] Base64ToByteArray(string base64Content)
    {
        try
        {
            // https://stackoverflow.com/questions/31524343/how-to-convert-base64-value-from-a-database-to-a-stream-with-c-sharp
            return Convert.FromBase64String(base64Content);
        }
        catch (System.Exception)
        {
            return null;
        }
    }

    private static uint GetBase64Length(uint originalLength)
    {
        // https://stackoverflow.com/questions/13378815/base64-length-calculation
        return (uint) Math.Ceiling(originalLength / 3D) * 4;
    }

    private static async Task<byte[]> ConvertToJpegAsync(byte[] content, int quality)
    {
        using (var job = new ImageJob())
        {
            var r = await job
                        .Decode(content)
                        .EncodeToBytes(new MozJpegEncoder(quality, true))
                        .Finish()
                        .InProcessAndDisposeAsync();
            return ((ArraySegment<byte>) r.First.TryGetBytes()).Array;
        }
    }
}