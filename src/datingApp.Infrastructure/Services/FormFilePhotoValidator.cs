using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Services;
using datingApp.Infrastructure.Exceptions;
using Microsoft.Extensions.Logging.TraceSource;
using Microsoft.Extensions.Options;

namespace datingApp.Infrastructure.Services;

internal sealed class FormFilePhotoValidator : IPhotoValidator<IFormFile>
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

    public void Validate(IFormFile content, out string extension)
    {
        ValidateSize(content);
        ValidateExtension(content, out extension);
    }

    private void ValidateSize(IFormFile photo)
    {
        if (photo == null)
        {
            throw new EmptyFormFileContentException();
        }

        if (photo.Length < _minPhotoSizeBytes || photo.Length > _maxPhotoSizeBytes)
        {
            throw new InvalidPhotoSizeException(_minPhotoSizeBytes, _maxPhotoSizeBytes);
        }
    }

    private void ValidateExtension(IFormFile photo, out string extension)
    {
        if (photo == null)
        {
            throw new EmptyFormFileContentException();
        }

        var ext = Path.GetExtension(photo.FileName).Trim().Trim('.').ToLowerInvariant();
        extension = ext;

        if (!_acceptedFileFormats.Any(format => format == ext))
        {
            throw new InvalidPhotoException();
        }
    }


}