using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Services;

namespace datingApp.Application.PhotoManagement;

internal sealed class DummyPhotoService : IPhotoService
{
    public PhotoServiceProcessOutput ProcessBase64Photo(string base64Bytes)
    {
        return new PhotoServiceProcessOutput(Array.Empty<byte>(), "jpg");
    }
}