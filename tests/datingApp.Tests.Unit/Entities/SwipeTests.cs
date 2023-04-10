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
        var exception = Record.Exception(() => new Swipe(0, Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Like.Like, DateTime.UtcNow));
        Assert.NotNull(exception);
        Assert.IsType<InvalidSwipeException>(exception);
    }
}