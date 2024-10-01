using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using datingApp.Application.PhotoManagement;
using datingApp.Infrastructure.Exceptions;
using Microsoft.Extensions.Options;

namespace datingApp.Infrastructure.PhotoManagement;

internal sealed class PhotoService : IPhotoService
{
    private readonly IOptions<PhotoServiceOptions> _options;
    private string _base64Bytes;
    private byte[] _bytes;
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

    public void SetBase64Photo(string base64content)
    {
        if (string.IsNullOrEmpty(base64content))
        {
            throw new EmptyBase64StringException();
        }
        _base64Bytes = base64content;
    }

    public byte[] GetArrayOfBytes()
    {
        if (string.IsNullOrEmpty(_base64Bytes))
        {
            throw new EmptyBase64StringException();
        }

        if (_bytes == null || _bytes.Length == 0)
        {
            // https://stackoverflow.com/questions/51300523/how-to-use-span-in-convert-tryfrombase64string
            byte[] bytes = new byte[((_base64Bytes.Length * 3) + 3) / 4 -
                (_base64Bytes.Length > 0 && _base64Bytes[^1] == '=' ?
                    _base64Bytes.Length > 1 && _base64Bytes[^2] == '=' ?
                        2 : 1 : 0)];
            if (!Convert.TryFromBase64String(_base64Bytes, bytes, out _))
            {
                throw new FailToConvertBase64StringToArrayOfBytes();
            }
            _bytes = bytes;
        }

        return _bytes;
    }

    public string GetImageFileFormat()
    {
        // Returns file extension associated with file format
        // if image file format is not known, returns null
        if (string.IsNullOrEmpty(_base64Bytes))
        {
            throw new EmptyBase64StringException();
        }
        GetArrayOfBytes();

        // FIXME: jakaś gotowa biblioteka do tego
        bool match = false;
        foreach (var item in knownFileHeaders)
        {
            for (int i = 0; i < item.Key.Length; i++)
            {
                match = _bytes[i] == item.Key[i];
                if (!match) break;
            }

            if (match)
            {
                return item.Value;
            }
        }
        return null;
    }

    public void ValidatePhoto()
    {
        if (string.IsNullOrEmpty(_base64Bytes))
        {
            throw new EmptyBase64StringException();
        }

        int maxPhotoSizeMB = _options.Value.MaxPhotoSizeBytes / (1024*1024);
        int minPhotoSizeKB = _options.Value.MinPhotoSizeBytes / 1024;
        int maxPhotoSizeMBBytes64EncodedStringApprox = (int) Math.Ceiling(1.5 * maxPhotoSizeMB);
        int minPhotoSizeKBBytes64EncodedStringApprox = (int) Math.Ceiling(1.5 * maxPhotoSizeMB);

        // Initial photo validation: is base64 string length within range
        if (_base64Bytes.Length < maxPhotoSizeMBBytes64EncodedStringApprox)
        {
            throw new InvalidPhotoSizeException(minPhotoSizeKB, maxPhotoSizeMB);
        }
        if (_base64Bytes.Length > minPhotoSizeKBBytes64EncodedStringApprox)
        {
            throw new InvalidPhotoSizeException(minPhotoSizeKB, maxPhotoSizeMB);
        }

        GetArrayOfBytes();

        // More precise photo validation based on bytes array and image 
        // format recognition
        if (_bytes.Length > _options.Value.MaxPhotoSizeBytes)
        {
            throw new InvalidPhotoSizeException(minPhotoSizeKB, maxPhotoSizeMB);
        }
        if (_bytes.Length < _options.Value.MinPhotoSizeBytes)
        {
            throw new InvalidPhotoSizeException(minPhotoSizeKB, maxPhotoSizeMB);
        }
        if (string.IsNullOrWhiteSpace(GetImageFileFormat()))
        {
            throw new InvalidPhotoException();
        }
    }
}