using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Services;

namespace datingApp.Infrastructure.Services;

internal sealed class PhotoUrlProvider : IPhotoUrlProvider
{
    public string GetPhotoUrl(string name, string extension)
    {
        return $"~/storage/{name}.{extension}";
    }
}