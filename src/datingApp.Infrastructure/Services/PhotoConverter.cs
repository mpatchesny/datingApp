using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Services;
using Imageflow.Fluent;
using Microsoft.Extensions.Options;

namespace datingApp.Infrastructure.Services;

internal sealed class PhotoConverter : IPhotoConverter
{
    private readonly int _compressedImageQuality;
    public PhotoConverter(IOptions<PhotoServiceOptions> options)
    {
        _compressedImageQuality = options.Value.CompressedImageQuality;
    }

    public async Task<Stream> ConvertAsync(Stream input, string targetFormat)
    {
        using (var job = new ImageJob())
        {
            var r = await job
                .Decode(content)
                .EncodeToBytes(new MozJpegEncoder(_compressedImageQuality, true))
                .Finish()
                .InProcessAndDisposeAsync();

            return ((ArraySegment<byte>) r.First.TryGetBytes()).Array;
        }
    }
}