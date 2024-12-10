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
        var imageStream = ImageTestsHelper.Base64ToMemoryStream(ImageTestsHelper.Base64WebpSample);
        var exception = await Record.ExceptionAsync(() => _converter.ConvertAsync(imageStream));
        Assert.Null(exception);
    }

    [Fact]
    public async void given_valid_image_file_convert_to_jpeg_should_success_and_converted_image_smaller_or_equal_to_original()
    {
        var imageStream = ImageTestsHelper.Base64ToMemoryStream(ImageTestsHelper.Base64JpgSample);
        var options = Options.Create<PhotoServiceOptions>(new PhotoServiceOptions());
        options.Value.CompressedImageQuality = 10;
        var converter = new JpegPhotoConverter(options);

        var convertedImage = await converter.ConvertAsync(imageStream);
        Assert.True(imageStream.Length >= convertedImage.Length);
    }

    [Fact]
    public async void given_valid_jpeg_file_convert_to_jpeg_should_success()
    {
        var imageStream = ImageTestsHelper.Base64ToMemoryStream(ImageTestsHelper.Base64JpgSample);
        var exception = await Record.ExceptionAsync(() => _converter.ConvertAsync(imageStream));
        Assert.Null(exception);
    }

    [Fact]
    public async void given_valid_png_file_convert_to_jpeg_should_success()
    {
        var imageStream = ImageTestsHelper.Base64ToMemoryStream(ImageTestsHelper.Base64PngSample);
        var exception = await Record.ExceptionAsync(() => _converter.ConvertAsync(imageStream));
        Assert.Null(exception);
    }

    [Fact]
    public async Task given_not_an_image_file_convert_to_jpeg_throws_exception()
    {
        var imageStream = ImageTestsHelper.Base64ToMemoryStream(ImageTestsHelper.Base64DocxSample);
        var exception = await Record.ExceptionAsync(() => _converter.ConvertAsync(imageStream));
        Assert.NotNull(exception);
    }

    [Theory]
    [InlineData(-5)]
    [InlineData(-1)]
    [InlineData(101)]
    public async Task given_image_quality_is_negative_or_above_100_convert_to_jpeg_throws_exception(int imageQuality)
    {
        var options = Options.Create<PhotoServiceOptions>(new PhotoServiceOptions());
        options.Value.CompressedImageQuality = imageQuality;
        var converter = new JpegPhotoConverter(options);
        var imageStream = ImageTestsHelper.Base64ToMemoryStream(ImageTestsHelper.Base64PngSample);

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
}