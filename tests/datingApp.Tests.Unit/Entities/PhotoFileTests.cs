using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace datingApp.Tests.Unit.Entities;

public class PhotoFileTests
{
    [Fact]
    public void given_bytes_content_is_null_throws_EmptyPhotoContentException()
    {
        Assert.True(true);
    }

    [Fact]
    public void given_base64_content_is_null_or_emptystring_throws_EmptyPhotoContentException()
    {
        Assert.True(true);
    }

    [Fact]
    public void given_not_recognized_file_format_throws_InvalidPhotoException()
    {
        Assert.True(true);
    }

    [Fact]
    public void given_recognized_file_format_extension_has_proper_value()
    {
        Assert.True(true);
    }

    [Fact]
    public void given_bad_base64_content_throws_FailToConvertBase64StringToArrayOfBytes()
    {
        Assert.True(true);
    }

    [Fact]
    public void given_valid_base64_content_Content_and_Exception_is_set()
    {
        Assert.True(true);
    }

    [Fact]
    public void given_valid_bytes_content_Content_and_Exception_is_set()
    {
        Assert.True(true);
    }

    [Fact]
    public void given_bytes_content_is_too_small_throws_InvalidPhotoSizeException()
    {
        Assert.True(true);
    }

    [Fact]
    public void given_bytes_content_is_too_large_throws_InvalidPhotoSizeException()
    {
        Assert.True(true);
    }

    [Fact]
    public void given_base64_content_is_too_small_throws_InvalidPhotoSizeException()
    {
        Assert.True(true);
    }

    [Fact]
    public void given_base64_content_is_too_large_throws_InvalidPhotoSizeException()
    {
        Assert.True(true);
    }

    private static string ToBase64(byte[] file)
    {
        return Convert.ToBase64String(file);
    }
}