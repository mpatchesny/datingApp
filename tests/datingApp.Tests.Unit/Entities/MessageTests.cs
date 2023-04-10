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
        var exception = Record.Exception(() =>new Message(1, 1, Guid.Parse("00000000-0000-0000-0000-000000000001"), "", false, DateTime.UtcNow));
        Assert.NotNull(exception);
        Assert.IsType<InvalidMessageException>(exception);
    }

    [Fact]
    public void message_length_above_255_should_throw_exception()
    {
        string message = "";
        for (int i=1; i <= 256; i++)
        {
            message += "a";
        }
        var exception = Record.Exception(() =>new Message(1, 1, Guid.Parse("00000000-0000-0000-0000-000000000001"), message, false, DateTime.UtcNow));
        Assert.NotNull(exception);
        Assert.IsType<InvalidMessageException>(exception);
    }

    [Fact]
    public void message_set_displayed_should_change_displayed_to_true()
    {
        var message = new Message(1, 1, Guid.Parse("00000000-0000-0000-0000-000000000001"), "test", false, DateTime.UtcNow);
        message.SetDisplayed();
        Assert.Equal(true, message.IsDisplayed);
    }
}