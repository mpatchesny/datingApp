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
        var exception = Record.Exception(() =>new Photo(1, Guid.Parse("00000000-0000-0000-0000-000000000001"), path, 1));
        Assert.NotNull(exception);
        Assert.IsType<PhotoEmptyPathException>(exception);
    }
}