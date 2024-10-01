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
    public void empty_or_null_photo_path_throws_PhotoEmptyPathException_1(string path)
    {
        var exception = Record.Exception(() =>new Photo(Guid.NewGuid(), Guid.NewGuid(), path, "abc", 1));
        Assert.NotNull(exception);
        Assert.IsType<PhotoEmptyPathException>(exception);
    }

    [Fact]
    public void empty_or_null_photo_path_throws_PhotoEmptyPathException_2()
    {
        string path = new string('a', 0);
        var exception = Record.Exception(() =>new Photo(Guid.NewGuid(), Guid.NewGuid(), path, "abc", 1));
        Assert.NotNull(exception);
        Assert.IsType<PhotoEmptyPathException>(exception);
    }
}