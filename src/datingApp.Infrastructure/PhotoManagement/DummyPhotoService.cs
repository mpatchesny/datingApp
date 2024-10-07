using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.PhotoManagement;

internal sealed class DummyPhotoService : IPhotoService
{
    public PhotoServiceProcessOutput ProcessBase64Photo(string base64Bytes)
    {
        return new PhotoServiceProcessOutput(Array.Empty<byte>(), "jpg");
    }
}