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

    public string GetImageFileFormat(byte[] photo)
    {
        return "jpg";
    }

    public void ValidatePhoto(byte[] photo)
    {
        // do nothing
    }
}