using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Services;
using datingApp.Infrastructure.Exceptions;
using Microsoft.Extensions.Logging.TraceSource;
using Microsoft.Extensions.Options;

namespace datingApp.Infrastructure.Services;

public class FormFilePhotoValidator : IPhotoValidator<IFormFile>
{
    private readonly int _minPhotoSizeBytes;
    private readonly int _maxPhotoSizeBytes;
    private readonly string[] _acceptedFileFormats;
    public FormFilePhotoValidator(IOptions<PhotoServiceOptions> options)
    {
        _minPhotoSizeBytes = options.Value.MinPhotoSizeBytes;
        _maxPhotoSizeBytes = options.Value.MaxPhotoSizeBytes;
        _acceptedFileFormats = options.Value.AcceptedFileFormats
            .Split(",")
            .Select(format => format.Trim().ToLowerInvariant())
            .ToArray();
    }

    public void ValidateSize(IFormFile photo)
    {
        if (photo.Length < _minPhotoSizeBytes || photo.Length > _maxPhotoSizeBytes)
        {
            throw new InvalidPhotoSizeException(_minPhotoSizeBytes, _maxPhotoSizeBytes);
        }
    }

    public void ValidateExtension(IFormFile photo, out string extension)
    {
        var ext = Path.GetExtension(photo.FileName).Trim().ToLowerInvariant();
        extension = ext;

        if (!_acceptedFileFormats.Any(format => format == ext))
        {
            throw new InvalidPhotoException();
        }
    }
}