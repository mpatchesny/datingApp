using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Infrastructure;
using datingApp.Infrastructure.Exceptions;
using datingApp.Infrastructure.Services;
using Microsoft.Extensions.Options;
using Xunit;

namespace datingApp.Tests.Unit.Services;

public class StreamPhotoValidatorTests
{
    [Fact]
    public void given_empty_base64_content_ProcessBase64Photo_throws_EmptyBase64StringException()
    {
        // var service = new PhotoService(_options);

        // var exception = Record.Exception(() => service.ProcessBase64Photo(""));
        // Assert.NotNull(exception);
        // Assert.IsType<EmptyBase64StringException>(exception);
    }

    [Fact]
    public void given_valid_file_which_is_not_image_ProcessBase64Photo_throws_InvalidPhotoException()
    {
        // _options.Value.MinPhotoSizeBytes = 1;
        // _options.Value.MaxPhotoSizeBytes = 999999;
        // var service = new PhotoService(_options);

        // var exception = Record.Exception(() => service.ProcessBase64Photo(_base64DocxSample));
        // Assert.NotNull(exception);
        // Assert.IsType<InvalidPhotoException>(exception);
    }

    [Fact]
    public void given_valid_unsupported_image_file_ProcessBase64Photo_throws_InvalidPhotoException()
    {
        // _options.Value.MinPhotoSizeBytes = 1;
        // _options.Value.MaxPhotoSizeBytes = 999999;
        // var service = new PhotoService(_options);

        // var exception = Record.Exception(() => service.ProcessBase64Photo(_base64GifSample));
        // Assert.NotNull(exception);
        // Assert.IsType<InvalidPhotoException>(exception);
    }

    [Fact]
    public void given_valid_bmp_file_ProcessBase64Photo_throws_InvalidPhotoException()
    {
        // _options.Value.MinPhotoSizeBytes = 1;
        // _options.Value.MaxPhotoSizeBytes = 999999;
        // var service = new PhotoService(_options);

        // var exception = Record.Exception(() => service.ProcessBase64Photo(_base64GifSample));
        // Assert.NotNull(exception);
        // Assert.IsType<InvalidPhotoException>(exception);
    }

    [Fact]
    public void given_valid_jpg_file_ProcessBase64Photo_return_jpg_extension()
    {
        // _options.Value.MinPhotoSizeBytes = 1;
        // _options.Value.MaxPhotoSizeBytes = 999999;
        // var service = new PhotoService(_options);

        // var output = service.ProcessBase64Photo(_base64JpgSample);
        // Assert.Equal("jpg", output.Extension);
    }

    [Fact]
    public void given_valid_png_file_ProcessBase64Photo_return_jpg_extension()
    {
        // _options.Value.MinPhotoSizeBytes = 1;
        // _options.Value.MaxPhotoSizeBytes = 999999;
        // var service = new PhotoService(_options);

        // var output = service.ProcessBase64Photo(_base64PngSample);
        // Assert.Equal("jpg", output.Extension);
    }

    [Fact]
    public void given_valid_webp_file_ProcessBase64Photo_return_jpg_extension()
    {
        // _options.Value.MinPhotoSizeBytes = 1;
        // _options.Value.MaxPhotoSizeBytes = 999999;
        // var service = new PhotoService(_options);

        // var output = service.ProcessBase64Photo(_base64WebpSample);
        // Assert.Equal("jpg", output.Extension);
    }

    [Fact]
    public void given_invalid_base64_encoded_string_ProcessBase64Photo_throws_FailToConvertBase64StringToArrayOfBytesException()
    {
        // _options.Value.MinPhotoSizeBytes = 0;
        // _options.Value.MaxPhotoSizeBytes = 999999;
        // var service = new PhotoService(_options);

        // var exception = Record.Exception(() => service.ProcessBase64Photo("ddd"));
        // Assert.NotNull(exception);
        // Assert.IsType<FailToConvertBase64StringToArrayOfBytesException>(exception);
    }

    [Fact]
    public void given_passed_valid_photo_is_too_small_ProcessBase64Photo_throws_InvalidPhotoSizeException()
    {
        // _options.Value.MinPhotoSizeBytes = 380;
        // _options.Value.MaxPhotoSizeBytes = 390;
        // var service = new PhotoService(_options);

        // var exception = Record.Exception(() => service.ProcessBase64Photo(_base64PngSample));
        // Assert.NotNull(exception);
        // Assert.IsType<InvalidPhotoSizeException>(exception);
    }

    [Fact]
    public void given_passed_valid_photo_is_too_large_ValidatePhoto_throws_InvalidPhotoSizeException()
    {
        // _options.Value.MinPhotoSizeBytes = 1;
        // _options.Value.MaxPhotoSizeBytes = 378;
        // var service = new PhotoService(_options);

        // var exception = Record.Exception(() => service.ProcessBase64Photo(_base64PngSample));
        // Assert.NotNull(exception);
        // Assert.IsType<InvalidPhotoSizeException>(exception);
    }
   
    private readonly IOptions<PhotoServiceOptions> _options;
    public StreamPhotoValidatorTests()
    {
        _options = Options.Create<PhotoServiceOptions>(new PhotoServiceOptions());
        _options.Value.MinPhotoSizeBytes = 1;
        _options.Value.MaxPhotoSizeBytes = 999999;
    }
}