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
    public void when_swiped_by_equals_swiped_who_swipe_should_throw_exception()
    {
        var exception = Record.Exception(() => new Swipe(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Like.Like, DateTime.UtcNow));
        Assert.NotNull(exception);
        Assert.IsType<InvalidSwipeException>(exception);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(7)]
    public void swipe_with_invalid_like_value_should_throw_exception(int like)
    {
        var exception = Record.Exception(() => new Swipe(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000002"), (Like) like, DateTime.UtcNow));
        Assert.NotNull(exception);
        Assert.IsType<LikeValueNotDefinedException>(exception);
    }
}