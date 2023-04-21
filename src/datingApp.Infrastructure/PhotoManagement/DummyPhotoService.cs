using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.PhotoManagement;

internal sealed class DummyPhotoService : IPhotoService
{
    public string SavePhoto(byte[] photo)
    {
        return Guid.NewGuid().ToString();
    }
}