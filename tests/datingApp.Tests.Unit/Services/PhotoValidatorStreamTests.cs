using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Infrastructure;
using datingApp.Infrastructure.Exceptions;
using datingApp.Infrastructure.Services;
using Microsoft.Extensions.Options;
using Xunit;

namespace datingApp.Tests.Unit.Services;

public class PhotoValidatorStreamTests
{
    [Fact]
    public void given_valid_file_which_is_not_image_ValidateExtension_throws_InvalidPhotoException()
    {
        var validator = new PhotoValidator(_options);
        var imageStream = ImageTestsHelper.Base64ToMemoryStream(ImageTestsHelper.Base64DocxSample);

        var exception = Record.Exception(() => validator.Validate(imageStream, out var extension));
        Assert.NotNull(exception);
        Assert.IsType<InvalidPhotoException>(exception);
    }

    [Fact]
    public void given_valid_unsupported_image_file_ValidateExtension_throws_InvalidPhotoException()
    {
        var validator = new PhotoValidator(_options);
        var imageStream = ImageTestsHelper.Base64ToMemoryStream(ImageTestsHelper.Base64GifSample);

        var exception = Record.Exception(() => validator.Validate(imageStream, out var extension));
        Assert.NotNull(exception);
        Assert.IsType<InvalidPhotoException>(exception);
    }

    [Fact]
    public void given_valid_bmp_file_ValidateExtension_throws_InvalidPhotoException()
    {
        var validator = new PhotoValidator(_options);
        var imageStream = ImageTestsHelper.Base64ToMemoryStream(ImageTestsHelper.Base64BmpSample);

        var exception = Record.Exception(() => validator.Validate(imageStream, out var extension));
        Assert.NotNull(exception);
        Assert.IsType<InvalidPhotoException>(exception);
    }

    [Fact]
    public void given_valid_jpg_file_ValidateExtension_return_jpg_extension()
    {
        var validator = new PhotoValidator(_options);
        var imageStream = ImageTestsHelper.Base64ToMemoryStream(ImageTestsHelper.Base64JpgSample);

        validator.Validate(imageStream, out var extension);
        Assert.Equal("jpg", extension);
    }

    [Fact]
    public void given_valid_png_file_ValidateExtension_return_png_extension()
    {
        var validator = new PhotoValidator(_options);
        var imageStream = ImageTestsHelper.Base64ToMemoryStream(ImageTestsHelper.Base64PngSample);

        validator.Validate(imageStream, out var extension);
        Assert.Equal("png", extension);
    }

    [Fact]
    public void given_valid_webp_file_ValidateExtension_return_webp_extension()
    {
        var validator = new PhotoValidator(_options);
        var imageStream = ImageTestsHelper.Base64ToMemoryStream(ImageTestsHelper.Base64WebpSample);

        validator.Validate(imageStream, out var extension);
        Assert.Equal("webp", extension);
    }

    [Fact]
    public void given_valid_photo_is_too_small_ValidateSize_throws_InvalidPhotoSizeException()
    {
        _options.Value.MinPhotoSizeBytes = 380;
        _options.Value.MaxPhotoSizeBytes = 390;
        var validator = new PhotoValidator(_options);
        var imageStream = ImageTestsHelper.Base64ToMemoryStream(ImageTestsHelper.Base64PngSample);

        var exception = Record.Exception(() => validator.Validate(imageStream, out var extension));
        Assert.NotNull(exception);
        Assert.IsType<InvalidPhotoSizeException>(exception);
    }

    [Fact]
    public void given_valid_photo_is_too_large_ValidateSize_throws_InvalidPhotoSizeException()
    {
        _options.Value.MinPhotoSizeBytes = 1;
        _options.Value.MaxPhotoSizeBytes = 378;
        var validator = new PhotoValidator(_options);
        var imageStream = ImageTestsHelper.Base64ToMemoryStream(ImageTestsHelper.Base64PngSample);

        var exception = Record.Exception(() => validator.Validate(imageStream, out var extension));
        Assert.NotNull(exception);
        Assert.IsType<InvalidPhotoSizeException>(exception);
    }
   
    private readonly IOptions<PhotoServiceOptions> _options;
    public PhotoValidatorStreamTests()
    {
        _options = Options.Create<PhotoServiceOptions>(new PhotoServiceOptions());
        _options.Value.MinPhotoSizeBytes = 1;
        _options.Value.MaxPhotoSizeBytes = 999999;
        _options.Value.AcceptedFileFormats = "png,webp,jpg";
    }
}