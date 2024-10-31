using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.Exceptions;
using Xunit;

namespace datingApp.Tests.Unit.Entities;

public class MatchTests
{
    [Fact]
    public void set_is_displayed_changes_enitity_state_1()
    {
        var match = new Match(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), false, false, null, DateTime.UtcNow);
        match.SetDisplayed(match.UserId1);
        Assert.True(match.IsDisplayedByUser1);
        Assert.False(match.IsDisplayedByUser2);
    }

    [Fact]
    public void set_is_displayed_changes_enitity_state_2()
    {
        var match = new Match(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), false, false, null, DateTime.UtcNow);
        match.SetDisplayed(match.UserId2);
        Assert.True(match.IsDisplayedByUser2);
        Assert.False(match.IsDisplayedByUser1);
    }

    [Fact]
    public void set_is_displayed_by_wrong_user_id_does_not_change_entity_state()
    {
        var match = new Match(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), false, false, null, DateTime.UtcNow);
        match.SetDisplayed(Guid.NewGuid());
        Assert.False(match.IsDisplayedByUser2);
        Assert.False(match.IsDisplayedByUser1);
    }

    [Fact]
    public void given_message_not_in_Match_AddMessage_adds_message()
    {
        var match = new Match(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), false, false, null, DateTime.UtcNow);
        var message = new Message(Guid.NewGuid(), match.Id, match.UserId1, "abc", false, DateTime.UtcNow);
        
        Assert.Empty(match.Messages);
        match.AddMessage(message);
        Assert.Single(match.Messages);
    }

    [Fact]
    public void given_message_in_Match_AddMessage_do_nothing()
    {
        var message = new Message(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "abc", false, DateTime.UtcNow);
        var messages = new List<Message> { message };
        var match = new Match(message.MatchId, message.SendFromId, Guid.NewGuid(), false, false, messages, DateTime.UtcNow);

        Assert.Single(match.Messages);
        match.AddMessage(message);
        Assert.Single(match.Messages);
    }

    [Fact]
    public void given_message_SendFrom_not_match_Match_UserId_AddMessage_throw_MessageSenderNotMatchMatchUsers_1()
    {
        var match = new Match(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), false, false, null, DateTime.UtcNow);
        var message = new Message(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "abc", false, DateTime.UtcNow);

        var exception = Record.Exception(() => match.AddMessage(message));
        Assert.NotNull(exception);
        Assert.IsType<MessageSenderNotMatchMatchUsers>(exception);
    }

    [Fact]
    public void given_message_not_in_Match_RemoveMessage_do_nothing()
    {
        var match = new Match(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), false, false, null, DateTime.UtcNow);
        var message = new Message(Guid.NewGuid(), match.Id, Guid.NewGuid(), "abc", false, DateTime.UtcNow);
        
        Assert.Empty(match.Messages);
        match.RemoveMessage(message.Id);
        Assert.Empty(match.Messages);
    }

    [Fact]
    public void given_message_in_Match_RemoveMessage_removes_message()
    {
        var message = new Message(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "abc", false, DateTime.UtcNow);
        var messages = new List<Message> { message };
        var match = new Match(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), false, false, messages, DateTime.UtcNow);
        
        Assert.Single(match.Messages);
        match.RemoveMessage(message.Id);
        Assert.Empty(match.Messages);
    }
}