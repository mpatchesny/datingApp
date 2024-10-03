using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.Exceptions;
using Xunit;

namespace datingApp.Tests.Unit.Entities;

public class PhotoTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void empty_or_null_photo_url_throws_EmptyPhotoUrlException_1(string url)
    {
        byte[] bytes = new byte[10241];
        bytes[0] = 0x42;
        bytes[1] = 0x4D;
        var photoFile = new PhotoFile(Guid.NewGuid(), bytes);
        var exception = Record.Exception(() =>new Photo(Guid.NewGuid(), Guid.NewGuid(), url, 1, photoFile));
        Assert.NotNull(exception);
        Assert.IsType<EmptyPhotoUrlException>(exception);
    }

    [Fact]
    public void empty_or_null_photo_url_throws_EmptyPhotoUrlException_2()
    {
        byte[] bytes = new byte[10241];
        bytes[0] = 0x42;
        bytes[1] = 0x4D;
        var photoFile = new PhotoFile(Guid.NewGuid(), bytes);
        string url = new string('a', 0);
        var exception = Record.Exception(() =>new Photo(Guid.NewGuid(), Guid.NewGuid(), url, 1, photoFile));
        Assert.NotNull(exception);
        Assert.IsType<EmptyPhotoUrlException>(exception);
    }

    [Fact]
    public void null_photo_file_throws_NullPhotoFileException()
    {
        var exception = Record.Exception(() =>new Photo(Guid.NewGuid(), Guid.NewGuid(), "some url", 1, null));
        Assert.NotNull(exception);
        Assert.IsType<NullPhotoFileException>(exception);
    }
}