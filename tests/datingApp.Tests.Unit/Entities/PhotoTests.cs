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
        var exception = Record.Exception(() =>new Photo(Guid.NewGuid(), Guid.NewGuid(), url, 1));
        Assert.NotNull(exception);
        Assert.IsType<EmptyPhotoUrlException>(exception);
    }

    [Fact]
    public void empty_or_null_photo_url_throws_EmptyPhotoUrlException_2()
    {
        string url = new string('a', 0);
        var exception = Record.Exception(() =>new Photo(Guid.NewGuid(), Guid.NewGuid(), url, 1));
        Assert.NotNull(exception);
        Assert.IsType<EmptyPhotoUrlException>(exception);
    }

    [Theory]
    [InlineData(-100)]
    [InlineData(-3)]
    [InlineData(-2)]
    [InlineData(-1)]
    public void photo_oridinal_is_set_to_0_if_negative(int oridinal)
    {
        var photo = new Photo(Guid.NewGuid(), Guid.NewGuid(), "abcdef", oridinal);
        Assert.Equal(0, photo.Oridinal.Value);
    }

    [Fact]
    public void change_oridinal_changes_photo_oridinal()
    {
        var photo = new Photo(Guid.NewGuid(), Guid.NewGuid(), "abcdef", 0);
        Assert.Equal(0, photo.Oridinal.Value);
        photo.ChangeOridinal(1);
        Assert.Equal(1, photo.Oridinal.Value);
    }
}