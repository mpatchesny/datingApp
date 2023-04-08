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
    public void empty_or_null_photo_path_should_throw_exception(string path)
    {
        var exception = Record.Exception(() =>new Photo(1, 1, path, 1));
        Assert.NotNull(exception);
        Assert.IsType<PhotoEmptyPathException>(exception);
    }
}