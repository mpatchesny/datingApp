using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Infrastructure.Services;
using Microsoft.Extensions.Options;
using Xunit;

namespace datingApp.Tests.Unit.Services;

public class JpegPhotoConverterTests
{
    [Fact]
    public async void given_valid_webp_file_convert_to_jpeg_should_success()
    {
        var imageStream = Base64ToMemoryStream(ImageSamples.Base64WebpSample);
        var exception = await Record.ExceptionAsync(() => _converter.ConvertAsync(imageStream));
        Assert.Null(exception);
    }

    [Fact]
    public async void given_valid_jpeg_file_convert_to_jpeg_should_success()
    {
        var imageStream = Base64ToMemoryStream(ImageSamples.Base64JpgSample);
        var exception = await Record.ExceptionAsync(() => _converter.ConvertAsync(imageStream));
        Assert.Null(exception);
    }

    [Fact]
    public async void given_valid_png_file_convert_to_jpeg_should_success()
    {
        var imageStream = Base64ToMemoryStream(ImageSamples.Base64PngSample);
        var exception = await Record.ExceptionAsync(() => _converter.ConvertAsync(imageStream));
        Assert.Null(exception);
    }

    [Fact]
    public async Task given_invalid_image_file_convert_to_jpeg_throws_exception()
    {
        var imageStream = Base64ToMemoryStream(ImageSamples.Base64DocxSample);
        var exception = await Record.ExceptionAsync(() => _converter.ConvertAsync(imageStream));
        Assert.NotNull(exception);
    }

    [Theory]
    [InlineData(-5)]
    [InlineData(-1)]
    [InlineData(101)]
    public async Task given_invalid_image_quality_convert_to_jpeg_throws_exception(int imageQuality)
    {
        var options = Options.Create<PhotoServiceOptions>(new PhotoServiceOptions());
        options.Value.CompressedImageQuality = imageQuality;
        var converter = new JpegPhotoConverter(options);
        var imageStream = Base64ToMemoryStream(ImageSamples.Base64PngSample);

        var exception = await Record.ExceptionAsync(() => converter.ConvertAsync(imageStream));
        Assert.NotNull(exception);
    }

    private readonly IOptions<PhotoServiceOptions> _options;
    private readonly JpegPhotoConverter _converter;
    public JpegPhotoConverterTests()
    {
        _options = Options.Create<PhotoServiceOptions>(new PhotoServiceOptions());
        _converter = new JpegPhotoConverter(_options);
    }

    private static MemoryStream Base64ToMemoryStream(string base64)
    {
        var bytes = Convert.FromBase64String(base64);
        return new MemoryStream(bytes);
    }
}