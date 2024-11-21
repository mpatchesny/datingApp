using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Services;
using datingApp.Infrastructure.Exceptions;
using Imageflow.Fluent;
using Microsoft.Extensions.Options;

namespace datingApp.Infrastructure.Services;

internal sealed class StreamPhotoValidator : IPhotoValidator<Stream>
{
    private readonly int _minPhotoSizeBytes;
    private readonly int _maxPhotoSizeBytes;
    private readonly string[] _acceptedFileFormats;
    public StreamPhotoValidator(IOptions<PhotoServiceOptions> options)
    {
        _minPhotoSizeBytes = options.Value.MinPhotoSizeBytes;
        _maxPhotoSizeBytes = options.Value.MaxPhotoSizeBytes;
        _acceptedFileFormats = options.Value.AcceptedFileFormats
            .Split(",")
            .Select(format => format.Trim().ToLowerInvariant())
            .ToArray();
    }

    public void ValidateExtension(Stream content, out string extension)
    {
        var info = ImageJob.GetImageInfoAsync(stream, SourceLifetime.TransferOwnership);
        info.AsTask().Wait();
        var ext = info.Result.PreferredExtension.ToLowerInvariant();
        extension = ext;

        if (!_acceptedFileFormats.Any(format => format == ext))
        {
            throw new InvalidPhotoException();
        }
    }

    public void ValidateSize(Stream content)
    {
        if (content.Length < _minPhotoSizeBytes || content.Length > _maxPhotoSizeBytes)
        {
            throw new InvalidPhotoSizeException(_minPhotoSizeBytes, _maxPhotoSizeBytes);
        }
    }
}