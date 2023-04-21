using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.PhotoManagement;

internal sealed class DummyPhotoService : IPhotoService
{
    public string SavePhoto(byte[] photo, string extension)
    {
        return $"{Guid.NewGuid().ToString()}.{extension}";
    }
}