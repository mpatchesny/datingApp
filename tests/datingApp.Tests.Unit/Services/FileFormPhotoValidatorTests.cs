using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using datingApp.Infrastructure.Exceptions;
using datingApp.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Xunit;

namespace datingApp.Tests.Unit.Services;

public class FileFormPhotoValidatorTests
{
    [Fact]
    public void given_valid_file_which_is_not_image_ValidateExtension_throws_InvalidPhotoException()
    {
        var stream = ImageHelper.Base64ToMemoryStream(ImageHelper.Base64DocxSample);
        var formFile = new FormFile(stream, 0, stream.Length, "fileContent", "file.docx")
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document/msword/docx"
        };

        var validator = new FormFilePhotoValidator(_options);
        var exception = Record.Exception(() => validator.Validate(formFile, out var extension));
        Assert.NotNull(exception);
        Assert.IsType<InvalidPhotoException>(exception);
    }

    [Fact]
    public void given_valid_unsupported_image_file_ValidateExtension_throws_InvalidPhotoException()
    {
        var stream = ImageHelper.Base64ToMemoryStream(ImageHelper.Base64GifSample);
        var formFile = new FormFile(stream, 0, stream.Length, "fileContent", "file.gif")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/gif"
        };

        var validator = new FormFilePhotoValidator(_options);
        var exception = Record.Exception(() => validator.Validate(formFile, out var extension));
        Assert.NotNull(exception);
        Assert.IsType<InvalidPhotoException>(exception);
    }

    [Fact]
    public void given_valid_bmp_file_ValidateExtension_throws_InvalidPhotoException()
    {
        var stream = ImageHelper.Base64ToMemoryStream(ImageHelper.Base64BmpSample);
        var formFile = new FormFile(stream, 0, stream.Length, "fileContent", "file.bmp")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/bmp"
        };

        var validator = new FormFilePhotoValidator(_options);
        var exception = Record.Exception(() => validator.Validate(formFile, out var extension));
        Assert.NotNull(exception);
        Assert.IsType<InvalidPhotoException>(exception);
    }

    [Fact]
    public void given_valid_jpg_file_ValidateExtension_return_jpg_extension()
    {
        var stream = ImageHelper.Base64ToMemoryStream(ImageHelper.Base64JpgSample);
        var formFile = new FormFile(stream, 0, stream.Length, "fileContent", "file.jpg")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/jpg"
        };
        var validator = new FormFilePhotoValidator(_options);

        validator.Validate(formFile, out var extension);
        Assert.Equal("jpg", extension);
    }

    [Fact]
    public void given_valid_png_file_ValidateExtension_return_png_extension()
    {
        var stream = ImageHelper.Base64ToMemoryStream(ImageHelper.Base64PngSample);
        var formFile = new FormFile(stream, 0, stream.Length, "fileContent", "file.png")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/png"
        };
        var validator = new FormFilePhotoValidator(_options);

        validator.Validate(formFile, out var extension);
        Assert.Equal("png", extension);
    }

    [Fact]
    public void given_valid_webp_file_ValidateExtension_return_webp_extension()
    {
        var stream = ImageHelper.Base64ToMemoryStream(ImageHelper.Base64WebpSample);
        var formFile = new FormFile(stream, 0, stream.Length, "fileContent", "file.webp")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/webp"
        };
        var validator = new FormFilePhotoValidator(_options);

        validator.Validate(formFile, out var extension);
        Assert.Equal("webp", extension);
    }

    [Fact]
    public void given_passed_valid_photo_is_too_small_ValidateSize_throws_InvalidPhotoSizeException()
    {
        var stream = ImageHelper.Base64ToMemoryStream(ImageHelper.Base64PngSample);
        var formFile = new FormFile(stream, 0, stream.Length, "fileContent", "file.webp")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/webp"
        };
        _options.Value.MinPhotoSizeBytes = 380;
        _options.Value.MaxPhotoSizeBytes = 390;
        var validator = new FormFilePhotoValidator(_options);

        var exception = Record.Exception(() => validator.Validate(formFile, out var extension));
        Assert.NotNull(exception);
        Assert.IsType<InvalidPhotoSizeException>(exception);
    }

    [Fact]
    public void given_passed_valid_photo_is_too_large_ValidateSize_throws_InvalidPhotoSizeException()
    {
        var stream = ImageHelper.Base64ToMemoryStream(ImageHelper.Base64PngSample);
        var formFile = new FormFile(stream, 0, stream.Length, "fileContent", "file.webp")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/webp"
        };
        _options.Value.MinPhotoSizeBytes = 1;
        _options.Value.MaxPhotoSizeBytes = 378;
        var validator = new FormFilePhotoValidator(_options);

        var exception = Record.Exception(() => validator.Validate(formFile, out var extension));
        Assert.NotNull(exception);
        Assert.IsType<InvalidPhotoSizeException>(exception);
    }
   
    private readonly IOptions<PhotoServiceOptions> _options;
    public FileFormPhotoValidatorTests()
    {
        _options = Options.Create<PhotoServiceOptions>(new PhotoServiceOptions());
        _options.Value.MinPhotoSizeBytes = 1;
        _options.Value.MaxPhotoSizeBytes = 999999;
        _options.Value.AcceptedFileFormats = "png,webp,jpg";
    }
}