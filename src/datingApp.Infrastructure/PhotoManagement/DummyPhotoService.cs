using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.PhotoManagement;

internal sealed class DummyPhotoService : IPhotoService
{
    public byte[] ConvertToArrayOfBytes(string base64content)
    {
        byte[] bytes = new byte[10];
        return bytes;
    }

    public void DeletePhoto(string path)
    {
        // do nothing
    }

    public string GetImageFileFormat(byte[] photo)
    {
        return "jpg";
    }

    public string SavePhoto(byte[] photo, string filename, string extension)
    {
        return $"{filename}.{extension}";
    }

    public void ValidatePhoto(byte[] photo)
    {
        // do nothing
    }
}