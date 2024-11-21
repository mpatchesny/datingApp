using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Services;
using Microsoft.Extensions.Logging.TraceSource;
using Microsoft.Extensions.Options;

namespace datingApp.Infrastructure.Services;

public class FormFilePhotoService : IPhotoValidator<IFormFile>
{
    private readonly IOptions<PhotoServiceOptions> _options;
    public FormFilePhotoService(IOptions<PhotoServiceOptions> options)
    {
        _options = options;
    }

    public bool ValidateSize(IFormFile photo)
    {
        if (photo.Length < _options.Value.MinPhotoSizeBytes)
        {
            return false;
        }
        if (photo.Length > _options.Value.MaxPhotoSizeBytes)
        {
            return false;
        }
        return true;
    }

    public bool ValidateExtension(IFormFile photo, out string extension)
    {
        var ext = Path.GetExtension(photo.FileName).Trim().ToLowerInvariant();
        extension = ext;

        var acceptedFileFormats = _options.Value.AcceptedFileFormats.Split(",");
        if (!acceptedFileFormats.Any(format => format.Trim().ToLowerInvariant() == ext))
        {
            return false;
        }
        return true;
    }
}