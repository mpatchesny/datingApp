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
    [Fact]
    public void photo_path_should_not_be_emptystring()
    {
        var exception = Record.Exception(() =>new Photo(1, 1, "", 1));
        Assert.NotNull(exception);
        Assert.IsType<PhotoEmptyPathException>(exception);
    }

    [Fact]
    public void photo_path_should_not_be_null()
    {
        var exception = Record.Exception(() =>new Photo(1, 1, "", 1));
        Assert.NotNull(exception);
        Assert.IsType<PhotoEmptyPathException>(exception);
    }
}