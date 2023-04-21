using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Infrastructure.Exceptions;
using datingApp.Infrastructure.PhotoManagement;
using Microsoft.Extensions.Options;
using Xunit;

namespace datingApp.Tests.Unit.PhotoManagement;

public class PhotoServiceTests
{
    [Fact]
    public void get_image_file_format_returns_null_if_unkown_format()
    {
        IOptions<PhotoServiceOptions> options = Options.Create<PhotoServiceOptions>(new PhotoServiceOptions());
        var service = new PhotoService(options);
        byte[] photo = new byte[] {0x74, 0x65, 0x73, 0x74};
        var ext = service.GetImageFileFormat(photo);
        Assert.Null(ext);
    }

    [Fact]
    public void given_invalid_unknown_file_format_validate_should_throw_exception()
    {
        IOptions<PhotoServiceOptions> options = Options.Create<PhotoServiceOptions>(new PhotoServiceOptions());
        var service = new PhotoService(options);
        byte[] photo = new byte[] {0x74, 0x65, 0x73, 0x74};
        options.Value.MaxPhotoSizeBytes=10000;
        options.Value.MinPhotoSizeBytes=1;
        var exception = Record.Exception(() => service.ValidatePhoto(photo));
        Assert.NotNull(exception);
        Assert.IsType<InvalidPhotoException>(exception);
    }

    [Fact]
    public void get_image_file_format_returns_jpg_if_given_jpg_file_header()
    {
        IOptions<PhotoServiceOptions> options = Options.Create<PhotoServiceOptions>(new PhotoServiceOptions());
        var service = new PhotoService(options);
        byte[] photo = new byte[] {0xFF, 0xD8, 0xFF};
        var ext = service.GetImageFileFormat(photo);
        Assert.Equal("jpg", ext);
    }

    [Fact]
    public void get_image_file_format_returns_bmp_if_given_bmp_file_header()
    {
        IOptions<PhotoServiceOptions> options = Options.Create<PhotoServiceOptions>(new PhotoServiceOptions());
        var service = new PhotoService(options);
        options.Value.MaxPhotoSizeBytes=10000;
        options.Value.MinPhotoSizeBytes=100;
        byte[] photo = new byte[] {0x42, 0x4D};
        var ext = service.GetImageFileFormat(photo);
        Assert.Equal("bmp", ext);
    }

    [Fact]
    public void get_image_file_format_returns_bmp_if_given_png_file_header()
    {
        IOptions<PhotoServiceOptions> options = Options.Create<PhotoServiceOptions>(new PhotoServiceOptions());
        var service = new PhotoService(options);
        byte[] photo = new byte[] {0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A};
        var ext = service.GetImageFileFormat(photo);
        Assert.Equal("png", ext);
    }

    [Fact]
    public void given_proper_base64_encoded_string_convert_to_array_of_bytes_should_succeed()
    {
        IOptions<PhotoServiceOptions> options = Options.Create<PhotoServiceOptions>(new PhotoServiceOptions());
        var service = new PhotoService(options);
        string Base64Bytes = "dGVzdA==";
        var exception = Record.Exception(() => service.ConvertToArrayOfBytes(Base64Bytes));
        Assert.Null(exception);
    }

    [Fact]
    public void given_invalid_base64_encoded_string_convert_to_array_of_bytes_should_throw_exception()
    {
        IOptions<PhotoServiceOptions> options = Options.Create<PhotoServiceOptions>(new PhotoServiceOptions());
        var service = new PhotoService(options);
        string Base64Bytes = "sd";
        var exception = Record.Exception(() => service.ConvertToArrayOfBytes(Base64Bytes));
        Assert.NotNull(exception);
        Assert.IsType<FailToConvertBase64StringToArrayOfBytes>(exception);
    }

    [Fact]
    public void too_small_photo_should_throw_exception()
    {
        IOptions<PhotoServiceOptions> options = Options.Create<PhotoServiceOptions>(new PhotoServiceOptions());
        options.Value.MaxPhotoSizeBytes=10000;
        options.Value.MinPhotoSizeBytes=100;
        var service = new PhotoService(options);
        byte[] photo = new byte[99];
        var exception = Record.Exception(() => service.ValidatePhoto(photo));
        Assert.NotNull(exception);
        Assert.IsType<InvalidPhotoSizeException>(exception);
    }

    [Fact]
    public void too_big_photo_should_throw_exception()
    {
        IOptions<PhotoServiceOptions> options = Options.Create<PhotoServiceOptions>(new PhotoServiceOptions());
        options.Value.MaxPhotoSizeBytes=100;
        options.Value.MinPhotoSizeBytes=99;
        var service = new PhotoService(options);
        byte[] photo = new byte[101];
        var exception = Record.Exception(() => service.ValidatePhoto(photo));
        Assert.NotNull(exception);
        Assert.IsType<InvalidPhotoSizeException>(exception);
    }

    [Fact]
    public void file_is_saved_to_storage_path()
    {
        IOptions<PhotoServiceOptions> options = Options.Create<PhotoServiceOptions>(new PhotoServiceOptions());
        options.Value.StoragePath = "./";
        var service = new PhotoService(options);
        byte[] photo = new byte[] {0x74, 0x65, 0x73, 0x74};
        var path = service.SavePhoto(photo, "txt");
        var fileExists = System.IO.File.Exists(path);
        Assert.True(fileExists);
        System.IO.File.Delete(path);
    }

    [Fact]
    public void directory_is_created_if_needed_upon_save_photo()
    {
        IOptions<PhotoServiceOptions> options = Options.Create<PhotoServiceOptions>(new PhotoServiceOptions());
        options.Value.StoragePath = testDirectoryPath;
        var service = new PhotoService(options);
        byte[] photo = new byte[] {0x74, 0x65, 0x73, 0x74};
        var path = service.SavePhoto(photo, "txt");
        var folderExists = System.IO.Directory.Exists(testDirectoryPath);
        Assert.True(folderExists);
        System.IO.File.Delete(path);
        System.IO.Directory.Delete(testDirectoryPath);
    }

    private string testDirectoryPath = "./test/";
    public PhotoServiceTests()
    {
        if (System.IO.Directory.Exists(testDirectoryPath))
        {
            System.IO.Directory.Delete(testDirectoryPath);
        }
    }
}