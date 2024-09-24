using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using Xunit;

namespace datingApp.Tests.Unit.Entities;

public class MessageTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void empty_message_string_should_throw_exception(string message)
    {
        var exception = Record.Exception(() =>new Message(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), message, false, DateTime.UtcNow));
        Assert.NotNull(exception);
        Assert.IsType<InvalidMessageException>(exception);
    }

    [Fact]
    public void empty_message_string_should_throw_exception2()
    {
        string message = new string('a', 0);
        var exception = Record.Exception(() =>new Message(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), message, false, DateTime.UtcNow));
        Assert.NotNull(exception);
        Assert.IsType<InvalidMessageException>(exception);
    }

    [Fact]
    public void message_length_above_255_should_throw_exception()
    {
        string message = new string('a', 256);
        var exception = Record.Exception(() =>new Message(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), message, false, DateTime.UtcNow));
        Assert.NotNull(exception);
        Assert.IsType<InvalidMessageException>(exception);
    }

    [Fact]
    public void message_set_displayed_should_change_displayed_to_true()
    {
        var message = new Message(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "test", false, DateTime.UtcNow);
        message.SetDisplayed();
        Assert.True(message.IsDisplayed);
    }
}