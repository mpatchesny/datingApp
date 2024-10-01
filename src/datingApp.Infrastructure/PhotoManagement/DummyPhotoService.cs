using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.PhotoManagement;

internal sealed class DummyPhotoService : IPhotoService
{
    public byte[] GetArrayOfBytes()
    {
        throw new NotImplementedException();
    }

    public string GetImageFileFormat()
    {
        return "jpg";
    }

    public void SetBase64Photo(string base64content)
    {
        // do nothing
    }

    public void ValidatePhoto()
    {
        // do nothing
    }
}