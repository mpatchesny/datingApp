using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Services;
using datingApp.Infrastructure.Exceptions;
using Imageflow.Fluent;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;

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
        var ext = "";

        try
        {
            IAsyncMemorySource memorySource = BufferedStreamSource.BorrowEntireStream(content);
            var info = ImageJob.GetImageInfoAsync(memorySource, SourceLifetime.TransferOwnership);
            info.AsTask().Wait();
            ext = info.Result.PreferredExtension.ToLowerInvariant();
        }
        catch (System.Exception)
        {
            // pass
        }

        if (!_acceptedFileFormats.Any(format => format == ext))
        {
            throw new InvalidPhotoException();
        }
        extension = ext;
    }

    public void ValidateSize(Stream content)
    {
        if (content.Length < _minPhotoSizeBytes || content.Length > _maxPhotoSizeBytes)
        {
            throw new InvalidPhotoSizeException(_minPhotoSizeBytes, _maxPhotoSizeBytes);
        }
    }
}