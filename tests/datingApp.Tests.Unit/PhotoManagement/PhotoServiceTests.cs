using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Infrastructure;
using datingApp.Infrastructure.Exceptions;
using datingApp.Infrastructure.PhotoManagement;
using Microsoft.Extensions.Options;
using Xunit;

namespace datingApp.Tests.Unit.PhotoManagement;

public class PhotoServiceTests
{
    [Fact]
    public void given_base64_photo_not_set_GetArrayOfBytes_throws_EmptyBase64StringException()
    {
        var service = new PhotoService(_options);

        var exception = Record.Exception(() => service.GetArrayOfBytes());
        Assert.NotNull(exception);
        Assert.IsType<EmptyBase64StringException>(exception);
    }

    [Fact]
    public void given_base64_photo_not_set_GetImageFileFormat_throws_EmptyBase64StringException()
    {
        var service = new PhotoService(_options);

        var exception = Record.Exception(() => service.GetImageFileFormat());
        Assert.NotNull(exception);
        Assert.IsType<EmptyBase64StringException>(exception);
    }

    [Fact]
    public void given_base64_photo_not_set_ValidatePhoto_throws_EmptyBase64StringException()
    {
        var service = new PhotoService(_options);

        var exception = Record.Exception(() => service.ValidatePhoto());
        Assert.NotNull(exception);
        Assert.IsType<EmptyBase64StringException>(exception);
    }

    [Fact]
    public void given_unknown_file_format_get_image_file_format_returns_null()
    {
        var service = new PhotoService(_options);
        byte[] photo = new byte[] {0x74, 0x65, 0x73, 0x74};
        string base64content = ToBase64(photo);
        service.SetBase64Photo(base64content);

        var ext = service.GetImageFileFormat();
        Assert.Null(ext);
    }

    [Fact]
    public void given_invalid_or_unknown_file_format_validate_throws_InvalidPhotoException()
    {
        var service = new PhotoService(_options);
        byte[] photo = new byte[] {0x74, 0x65, 0x73, 0x74};
        string base64content = ToBase64(photo);
        service.SetBase64Photo(base64content);

        var exception = Record.Exception(() => service.ValidatePhoto());
        Assert.NotNull(exception);
        Assert.IsType<InvalidPhotoException>(exception);
    }

    [Fact]
    public void get_image_file_format_returns_jpg_if_given_jpg_file_header()
    {
        var service = new PhotoService(_options);
        byte[] photo = new byte[] {0xFF, 0xD8, 0xFF};
        string base64content = ToBase64(photo);
        service.SetBase64Photo(base64content);

        var ext = service.GetImageFileFormat();
        Assert.Equal("jpg", ext);
    }

    [Fact]
    public void get_image_file_format_returns_bmp_if_given_bmp_file_header()
    {
        var service = new PhotoService(_options);
        byte[] photo = new byte[] {0x42, 0x4D};
        string base64content = ToBase64(photo);
        service.SetBase64Photo(base64content);

        var ext = service.GetImageFileFormat();
        Assert.Equal("bmp", ext);
    }

    [Fact]
    public void get_image_file_format_returns_bmp_if_given_png_file_header()
    {
        var service = new PhotoService(_options);
        byte[] photo = new byte[] {0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A};
        string base64content = ToBase64(photo);
        service.SetBase64Photo(base64content);

        var ext = service.GetImageFileFormat();
        Assert.Equal("png", ext);
    }

    [Fact]
    public void given_proper_base64_encoded_string_get_array_of_bytes_should_succeed()
    {
        var options = Options.Create<PhotoServiceOptions>(new PhotoServiceOptions());
        var service = new PhotoService(options);
        string base64content = "dGVzdA==";
        service.SetBase64Photo(base64content);

        var exception = Record.Exception(() => service.GetArrayOfBytes());
        Assert.Null(exception);
    }

    [Fact]
    public void given_invalid_base64_encoded_string_get_array_of_bytes_throws_FailToConvertBase64StringToArrayOfBytes()
    {
        var service = new PhotoService(_options);
        string base64content = "dd";
        service.SetBase64Photo(base64content);

        var exception = Record.Exception(() => service.GetArrayOfBytes());
        Assert.NotNull(exception);
        Assert.IsType<FailToConvertBase64StringToArrayOfBytes>(exception);
    }

    [Fact]
    public void given_passed_photo_is_too_small_ValidatePhoto_throws_InvalidPhotoSizeException()
    {
        _options.Value.MaxPhotoSizeBytes = 10000;
        _options.Value.MinPhotoSizeBytes = 100;
        var service = new PhotoService(_options);
        byte[] photo = new byte[_options.Value.MinPhotoSizeBytes - 1];
        string base64content = ToBase64(photo);
        service.SetBase64Photo(base64content);

        var exception = Record.Exception(() => service.ValidatePhoto());
        Assert.NotNull(exception);
        Assert.IsType<InvalidPhotoSizeException>(exception);
    }

    [Fact]
    public void given_passed_photo_is_too_large_ValidatePhoto_throws_InvalidPhotoSizeException()
    {
        _options.Value.MaxPhotoSizeBytes = 100;
        _options.Value.MinPhotoSizeBytes = 99;
        var service = new PhotoService(_options);
        byte[] photo = new byte[_options.Value.MaxPhotoSizeBytes + 1];
        string base64content = ToBase64(photo);
        service.SetBase64Photo(base64content);

        var exception = Record.Exception(() => service.ValidatePhoto());
        Assert.NotNull(exception);
        Assert.IsType<InvalidPhotoSizeException>(exception);
    }

    [Fact]
    public void given_passed_base64_string_is_too_small_ValidatePhoto_throws_InvalidPhotoSizeException()
    {
        _options.Value.MaxPhotoSizeBytes = 10000;
        _options.Value.MinPhotoSizeBytes = 100;
        var service = new PhotoService(_options);
        int base64len = (int) Math.Ceiling(_options.Value.MinPhotoSizeBytes * 1.5) - 1;
        string base64content = "x".PadRight(base64len);
        service.SetBase64Photo(base64content);

        var exception = Record.Exception(() => service.ValidatePhoto());
        Assert.NotNull(exception);
        Assert.IsType<InvalidPhotoSizeException>(exception);
    }

    [Fact]
    public void given_passed_based64_string_is_too_large_ValidatePhoto_throws_InvalidPhotoSizeException()
    {
        _options.Value.MaxPhotoSizeBytes = 100;
        _options.Value.MinPhotoSizeBytes = 99;
        var service = new PhotoService(_options);
        int base64len = (int) Math.Ceiling(_options.Value.MaxPhotoSizeBytes * 1.5) + 1;
        string base64content = "x".PadRight(base64len);
        service.SetBase64Photo(base64content);

        var exception = Record.Exception(() => service.ValidatePhoto());
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