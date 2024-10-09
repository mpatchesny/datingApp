using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Infrastructure;
using datingApp.Infrastructure.Exceptions;
using datingApp.Infrastructure.Services;
using Microsoft.Extensions.Options;
using Xunit;

namespace datingApp.Tests.Unit.PhotoManagement;

public class PhotoServiceTests
{
    [Fact]
    public void given_empty_base64_content_ProcessBase64Photo_throws_EmptyBase64StringException()
    {
        var service = new PhotoService(_options);

        var exception = Record.Exception(() => service.ProcessBase64Photo(""));
        Assert.NotNull(exception);
        Assert.IsType<EmptyBase64StringException>(exception);
    }

    [Fact]
    public void given_unknown_file_format_ProcessBase64Photo_throws_InvalidPhotoException()
    {
        _options.Value.MinPhotoSizeBytes = 1;
        _options.Value.MaxPhotoSizeBytes = 999999;
        var service = new PhotoService(_options);
        byte[] photo = new byte[] {0x74, 0x65, 0x73, 0x74};
        string base64content = ToBase64(photo);

        var exception = Record.Exception(() => service.ProcessBase64Photo(base64content));
        Assert.NotNull(exception);
        Assert.IsType<InvalidPhotoException>(exception);
    }

    [Fact]
    public void given_jpg_file_format_ProcessBase64Photo_return_jpg_extension()
    {
        _options.Value.MinPhotoSizeBytes = 1;
        _options.Value.MaxPhotoSizeBytes = 999999;
        var service = new PhotoService(_options);
        byte[] photo = new byte[] {0xFF, 0xD8, 0xFF};
        string base64content = ToBase64(photo);

        var output = service.ProcessBase64Photo(base64content);
        Assert.Equal("jpg", output.Extension);
    }

    [Fact]
    public void given_bmp_file_format_ProcessBase64Photo_return_bmp_extension()
    {
        _options.Value.MinPhotoSizeBytes = 1;
        _options.Value.MaxPhotoSizeBytes = 999999;
        var service = new PhotoService(_options);
        byte[] photo = new byte[] {0x42, 0x4D};
        string base64content = ToBase64(photo);

        var output = service.ProcessBase64Photo(base64content);
        Assert.Equal("bmp", output.Extension);
    }

    [Fact]
    public void given_png_file_format_ProcessBase64Photo_return_png_extension()
    {
        _options.Value.MinPhotoSizeBytes = 1;
        _options.Value.MaxPhotoSizeBytes = 999999;
        var service = new PhotoService(_options);
        byte[] photo = new byte[] {0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A};
        string base64content = ToBase64(photo);

        var output = service.ProcessBase64Photo(base64content);
        Assert.Equal("png", output.Extension);
    }

    [Fact]
    public void given_invalid_base64_encoded_string_ProcessBase64Photo_throws_FailToConvertBase64StringToArrayOfBytesException()
    {
        _options.Value.MinPhotoSizeBytes = 1;
        _options.Value.MaxPhotoSizeBytes = 999999;
        var service = new PhotoService(_options);

        var exception = Record.Exception(() => service.ProcessBase64Photo("ddd"));
        Assert.NotNull(exception);
        Assert.IsType<FailToConvertBase64StringToArrayOfBytesException>(exception);
    }

    [Fact]
    public void given_passed_photo_is_too_small_ProcessBase64Photo_throws_InvalidPhotoSizeException()
    {
        _options.Value.MaxPhotoSizeBytes = 10000;
        _options.Value.MinPhotoSizeBytes = 100;
        var service = new PhotoService(_options);
        byte[] photo = new byte[_options.Value.MinPhotoSizeBytes - 1];
        photo[0] = 0x42;
        photo[1] = 0x4D;
        string base64content = ToBase64(photo);

        var exception = Record.Exception(() => service.ProcessBase64Photo(base64content));
        Assert.NotNull(exception);
        Assert.IsType<InvalidPhotoSizeException>(exception);
    }

    [Fact]
    public void given_passed_photo_is_too_large_ValidatePhoto_throws_InvalidPhotoSizeException()
    {
        _options.Value.MaxPhotoSizeBytes = 10000;
        _options.Value.MinPhotoSizeBytes = 100;
        var service = new PhotoService(_options);
        byte[] photo = new byte[_options.Value.MaxPhotoSizeBytes + 1];
        photo[0] = 0x42;
        photo[1] = 0x4D;
        string base64content = ToBase64(photo);

        var exception = Record.Exception(() => service.ProcessBase64Photo(base64content));
        Assert.NotNull(exception);
        Assert.IsType<InvalidPhotoSizeException>(exception);
    }

    private readonly IOptions<PhotoServiceOptions> _options;
    public PhotoServiceTests()
    {
        _options = Options.Create<PhotoServiceOptions>(new PhotoServiceOptions());
        _options.Value.MinPhotoSizeBytes = 1;
        _options.Value.MaxPhotoSizeBytes = 999999;
    }

    private static string ToBase64(byte[] file)
    {
        return Convert.ToBase64String(file);
    }
}