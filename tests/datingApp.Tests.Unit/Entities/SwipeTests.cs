using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.Exceptions;
using Xunit;

namespace datingApp.Tests.Unit.Entities;

public class SwipeTests
{
    [Fact]
    public void when_swipped_by_equals_swipped_who_swipe_should_throw_exception()
    {
        var exception = Record.Exception(() => new Swipe(0, 1, 1, Like.Like, DateTime.UtcNow));
        Assert.NotNull(exception);
        Assert.IsType<InvalidSwipeException>(exception);
    }
}