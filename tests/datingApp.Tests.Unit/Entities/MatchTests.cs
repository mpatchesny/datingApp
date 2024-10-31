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

    [Fact]
    public void given_message_not_in_Match_SetPreviousMessagesAsDisplayed_do_nothing()
    {
        var matchId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var messages = new List<Message> { 
            new Message(Guid.NewGuid(), matchId, userId, "abc", false, DateTime.UtcNow),
            new Message(Guid.NewGuid(), matchId, userId, "abc", false, DateTime.UtcNow),
            new Message(Guid.NewGuid(), matchId, userId, "abc", false, DateTime.UtcNow),
            new Message(Guid.NewGuid(), matchId, userId, "abc", false, DateTime.UtcNow),
            new Message(Guid.NewGuid(), matchId, userId, "abc", false, DateTime.UtcNow),
         };
        var match = new Match(matchId, userId, Guid.NewGuid(), false, false, messages, DateTime.UtcNow);

        var exception = Record.Exception(() => match.SetPreviousMessagesAsDisplayed(Guid.NewGuid(), match.UserId1));
        Assert.Null(exception);
        Assert.Collection(match.Messages, 
            m => Assert.False(m.IsDisplayed),
            m => Assert.False(m.IsDisplayed),
            m => Assert.False(m.IsDisplayed),
            m => Assert.False(m.IsDisplayed),
            m => Assert.False(m.IsDisplayed)
        );
    }

    [Fact]
    public void SetPreviousMessagesAsDisplayed_sets_last_message_as_displayed()
    {
        var matchId = Guid.NewGuid();
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();

        var createdAt = DateTime.UtcNow - TimeSpan.FromSeconds(1);
        var lastMessage = new Message(Guid.NewGuid(), matchId, userId1, "abc", false, createdAt);
        var messages = new List<Message> { lastMessage };

        var match = new Match(matchId, userId1, userId2, false, false, messages, DateTime.UtcNow);

        match.SetPreviousMessagesAsDisplayed(lastMessage.Id, userId2);
        Assert.Collection(match.Messages, 
            m => Assert.True(m.IsDisplayed)
        );
    }

    [Fact]
    public void SetPreviousMessagesAsDisplayed_sets_messages_as_displayed_if_message_is_created_before_last_message_and_send_from_is_different_than_displayedUser()
    {
        var matchId = Guid.NewGuid();
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();

        var createdAt = DateTime.UtcNow - TimeSpan.FromSeconds(1);
        var lastMessage = new Message(Guid.NewGuid(), matchId, userId1, "abc", false, createdAt);
        var messages = new List<Message> { 
            lastMessage,
            new Message(Guid.NewGuid(), matchId, userId1, "abc", false, DateTime.UtcNow),
            new Message(Guid.NewGuid(), matchId, userId1, "abc", false, createdAt - TimeSpan.FromMinutes(1)),
            new Message(Guid.NewGuid(), matchId, userId1, "abc", true, createdAt - TimeSpan.FromHours(1)),
            new Message(Guid.NewGuid(), matchId, userId2, "abc", false, createdAt),
            new Message(Guid.NewGuid(), matchId, userId2, "abc", false, createdAt),
            new Message(Guid.NewGuid(), matchId, userId2, "abc", false, createdAt),
        };
        var match = new Match(matchId, userId1, userId2, false, false, messages, DateTime.UtcNow);
        
        match.SetPreviousMessagesAsDisplayed(lastMessage.Id, userId2);
        Assert.Collection(match.Messages, 
            m => Assert.True(m.IsDisplayed),
            m => Assert.False(m.IsDisplayed),
            m => Assert.True(m.IsDisplayed),
            m => Assert.True(m.IsDisplayed),
            m => Assert.False(m.IsDisplayed),
            m => Assert.False(m.IsDisplayed),
            m => Assert.False(m.IsDisplayed)
        );
    }
}