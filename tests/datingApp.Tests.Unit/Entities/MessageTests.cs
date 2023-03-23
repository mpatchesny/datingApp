using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;

namespace datingApp.Tests.Unit.Entities;

public class MessageTests
{
    [Fact]
    public void message_should_not_be_emptystring()
    {
        var exception = Record.Exception(() =>new Message(1, 1, 1, 1, "", false, DateTime.UtcNow));
        Assert.NotNull(exception);
        Assert.IsType<InvalidMessageException>(exception);
    }

    [Fact]
    public void message_length_should_not_be_above_255()
    {
        string message = "";
        for (int i=1; i <= 256; i++)
        {
            message += "a";
        }
        var exception = Record.Exception(() =>new Message(1, 1, 1, 1, message, false, DateTime.UtcNow));
        Assert.NotNull(exception);
        Assert.IsType<InvalidMessageException>(exception);
    }

    [Fact]
    public void message_set_displayed_should_change_displayed_to_true()
    {
        var message = new Message(1, 1, 1, 1, "test", false, DateTime.UtcNow);
        message.SetDisplayed();
        Assert.Equal(true, message.IsDisplayed);
    }
}