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
    public void empty_or_null_photo_url_throws_PhotoEmptyPathException_1(string url)
    {
        var exception = Record.Exception(() =>new Photo(Guid.NewGuid(), Guid.NewGuid(), url, 1));
        Assert.NotNull(exception);
        Assert.IsType<PhotoEmptyUrlException>(exception);
    }

    [Fact]
    public void empty_or_null_photo_url_throws_PhotoEmptyPathException_2()
    {
        string url = new string('a', 0);
        var exception = Record.Exception(() =>new Photo(Guid.NewGuid(), Guid.NewGuid(), url, 1));
        Assert.NotNull(exception);
        Assert.IsType<PhotoEmptyUrlException>(exception);
    }
}