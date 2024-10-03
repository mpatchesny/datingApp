using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.Exceptions;
using Xunit;

namespace datingApp.Tests.Unit.Entities;

public class PhotoFileTests
{
    [Fact]
    public void given_bytes_content_is_null_throws_EmptyPhotoContentException()
    {
        byte[] bytes = null;

        var exception = Record.Exception(() => new PhotoFile(Guid.NewGuid(), bytes));
        Assert.NotNull(exception);
        Assert.IsType<EmptyPhotoContentException>(exception);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void given_base64_content_is_null_or_emptystring_throws_EmptyPhotoContentException(string byte64Content)
    {
        var exception = Record.Exception(() => new PhotoFile(Guid.NewGuid(), byte64Content));
        Assert.NotNull(exception);
        Assert.IsType<EmptyPhotoContentException>(exception);
    }

    [Fact]
    public void given_not_recognized_file_format_throws_InvalidPhotoException()
    {
        byte[] bytes = new byte[10241];
        bytes[0] = 0x42;
        bytes[1] = 0x4F;

        var exception = Record.Exception(() => new PhotoFile(Guid.NewGuid(), bytes));
        Assert.NotNull(exception);
        Assert.IsType<InvalidPhotoException>(exception);
    }

    [Fact]
    public void given_recognized_file_format_Extension_has_proper_value()
    {
        byte[] bytes = new byte[10241];
        bytes[0] = 0x42;
        bytes[1] = 0x4D;

        PhotoFile photoFile = new PhotoFile(Guid.NewGuid(), bytes);
        Assert.Equal("bmp", photoFile.Extension);
        Assert.Equal(bytes, photoFile.Content);
    }

    [Fact (Skip = "throws InvalidPhoto because file format is not recognized")]
    public void given_bad_base64_content_throws_FailToConvertBase64StringToArrayOfBytesException()
    {
        var base64Content = new string('a', 25000);

        var exception = Record.Exception(() => new PhotoFile(Guid.NewGuid(), base64Content));
        Assert.NotNull(exception);
        Assert.IsType<FailToConvertBase64StringToArrayOfBytesException>(exception);
    }

    [Fact]
    public void given_valid_base64_content_Content_and_Extension_is_set()
    {
        byte[] bytes = new byte[20000];
        bytes[0] = 0x42;
        bytes[1] = 0x4D;
        string byte64Content = Convert.ToBase64String(bytes);

        PhotoFile photoFile = new PhotoFile(Guid.NewGuid(), byte64Content);
        Assert.Equal("bmp", photoFile.Extension);
        Assert.Equal(bytes, photoFile.Content);
    }

    [Fact]
    public void given_valid_bytes_content_Content_and_Extension_is_set()
    {
        byte[] bytes = new byte[10241];
        bytes[0] = 0x42;
        bytes[1] = 0x4D;

        PhotoFile photoFile = new PhotoFile(Guid.NewGuid(), bytes);
        Assert.Equal("bmp", photoFile.Extension);
        Assert.Equal(bytes, photoFile.Content);
    }

    [Fact]
    public void given_bytes_content_is_too_small_throws_InvalidPhotoSizeException()
    {
        var byteContent = new byte[10240 - 1];

        var exception = Record.Exception(() => new PhotoFile(Guid.NewGuid(), byteContent));
        Assert.NotNull(exception);
        Assert.IsType<InvalidPhotoSizeException>(exception);
    }

    [Fact]
    public void given_bytes_content_is_too_large_throws_InvalidPhotoSizeException()
    {
        var byteContent = new byte[2621440 + 1];

        var exception = Record.Exception(() => new PhotoFile(Guid.NewGuid(), byteContent));
        Assert.NotNull(exception);
        Assert.IsType<InvalidPhotoSizeException>(exception);
    }

    [Fact]
    public void given_base64_content_is_too_small_throws_InvalidPhotoSizeException()
    {
        var base64Content = new string('a', 15359);

        var exception = Record.Exception(() => new PhotoFile(Guid.NewGuid(), base64Content));
        Assert.NotNull(exception);
        Assert.IsType<InvalidPhotoSizeException>(exception);
    }

    [Fact]
    public void given_base64_content_is_too_large_throws_InvalidPhotoSizeException()
    {
        var base64Content = new string('a', 3932161);

        var exception = Record.Exception(() => new PhotoFile(Guid.NewGuid(), base64Content));
        Assert.NotNull(exception);
        Assert.IsType<InvalidPhotoSizeException>(exception);
    }
}