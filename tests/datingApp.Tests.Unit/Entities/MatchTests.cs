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
        var match = new Match(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow);
        match.SetDisplayed(match.UserId1);
        Assert.True(match.IsDisplayedByUser(match.UserId1));
        Assert.False(match.IsDisplayedByUser(match.UserId2));
    }

    [Fact]
    public void set_is_displayed_changes_enitity_state_2()
    {
        var match = new Match(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow);
        match.SetDisplayed(match.UserId2);
        Assert.True(match.IsDisplayedByUser(match.UserId2));
        Assert.False(match.IsDisplayedByUser(match.UserId1));
    }

    [Fact]
    public void set_is_displayed_changes_sets_all_messages_send_by_other_user_as_displayed()
    {
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var messages = new List<Message>() {
            new Message(Guid.NewGuid(), userId1, "text", false, DateTime.UtcNow),
            new Message(Guid.NewGuid(), userId1, "text", true, DateTime.UtcNow),
            new Message(Guid.NewGuid(), userId2, "text", false, DateTime.UtcNow),
            new Message(Guid.NewGuid(), userId2, "text", false, DateTime.UtcNow),
            new Message(Guid.NewGuid(), userId2, "text", true, DateTime.UtcNow),
        };
        var match = new Match(Guid.NewGuid(), userId1, userId2, DateTime.UtcNow, messages: messages);
        match.SetDisplayed(match.UserId1);

        Assert.Empty(match.Messages.Where(msg => msg.SendFromId.Value == userId2 && msg.IsDisplayed == false));
        Assert.NotEmpty(match.Messages.Where(msg => msg.SendFromId.Value == userId1 && msg.IsDisplayed == false));
        Assert.NotEmpty(match.Messages.Where(msg => msg.SendFromId.Value == userId1 && msg.IsDisplayed == true));
    }

    [Fact]
    public void set_is_displayed_by_wrong_user_id_does_not_change_entity_state()
    {
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var messages = new List<Message>() {
            new Message(Guid.NewGuid(), userId1, "text", false, DateTime.UtcNow),
            new Message(Guid.NewGuid(), userId1, "text", false, DateTime.UtcNow),
            new Message(Guid.NewGuid(), userId2, "text", false, DateTime.UtcNow),
            new Message(Guid.NewGuid(), userId2, "text", false, DateTime.UtcNow),
            new Message(Guid.NewGuid(), userId2, "text", false, DateTime.UtcNow),
        };
        var match = new Match(Guid.NewGuid(), userId1, userId2, DateTime.UtcNow, messages: messages);

        match.SetDisplayed(Guid.NewGuid());

        Assert.False(match.IsDisplayedByUser(match.UserId1));
        Assert.False(match.IsDisplayedByUser(match.UserId2));
        Assert.Empty(match.Messages.Where(msg => msg.IsDisplayed == true));
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, true)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public void is_displayed_by_user_returns_proper_value(bool isDisplayedByUser1, bool isDisplayedByUser2)
    {
        var match = new Match(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, isDisplayedByUser1: isDisplayedByUser1, isDisplayedByUser2: isDisplayedByUser2);

        Assert.Equal(isDisplayedByUser1, match.IsDisplayedByUser(match.UserId1));
        Assert.Equal(isDisplayedByUser2, match.IsDisplayedByUser(match.UserId2));
    }

    [Fact]
    public void is_displayed_by_user_not_in_match_returns_false()
    {
        var match = new Match(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, isDisplayedByUser1: true, isDisplayedByUser2: true);

        Assert.False(match.IsDisplayedByUser(Guid.NewGuid()));
    }

    [Fact]
    public void given_message_not_in_Match_AddMessage_adds_message_and_sets_LastActivityTime_to_added_message_CreatedAt()
    {
        var match = new Match(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow);
        var messageCreateTime = DateTime.UtcNow.AddSeconds(10);
        var message = new Message(Guid.NewGuid(), match.UserId1, "abc", false, messageCreateTime);

        Assert.Empty(match.Messages);
        match.AddMessage(message);
        Assert.Single(match.Messages);
        Assert.Equal(messageCreateTime, match.LastActivityTime);
    }

    [Fact]
    public void given_message_in_Match_AddMessage_do_nothing()
    {
        var match = new Match(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow);
        var messageCreateTime = DateTime.UtcNow.AddSeconds(10);
        var message = new Message(Guid.NewGuid(), match.UserId1, "abc", false, messageCreateTime);

        match.AddMessage(message);
        Assert.Single(match.Messages);
        match.AddMessage(message);
        Assert.Single(match.Messages);
        Assert.Equal(messageCreateTime, match.LastActivityTime);
    }

    [Fact]
    public void given_message_SendFrom_not_match_Match_UserId_AddMessage_throw_MessageSenderNotMatchMatchUsers()
    {
        var match = new Match(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow);
        var message = new Message(Guid.NewGuid(), Guid.NewGuid(), "abc", false, DateTime.UtcNow);

        var exception = Record.Exception(() => match.AddMessage(message));
        Assert.NotNull(exception);
        Assert.IsType<MessageSenderNotMatchMatchUsers>(exception);
    }

    [Fact]
    public void given_message_not_in_Match_Messages_RemoveMessage_do_nothing()
    {
        var match = new Match(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow);
        var message = new Message(Guid.NewGuid(), Guid.NewGuid(), "abc", false, DateTime.UtcNow);
        
        Assert.Empty(match.Messages);
        match.RemoveMessage(message.Id);
        Assert.Empty(match.Messages);
    }

    [Fact]
    public void given_message_in_Match_Messages_RemoveMessage_removes_message()
    {
        var message = new Message(Guid.NewGuid(), Guid.NewGuid(), "abc", false, DateTime.UtcNow);
        var messages = new List<Message> { message };
        var match = new Match(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, messages: messages);
        
        Assert.Single(match.Messages);
        match.RemoveMessage(message.Id);
        Assert.Empty(match.Messages);
    }

    [Fact]
    public void given_message_in_Match_Messages_RemoveMessage_changes_LastActivityTime_to_max_CreatedAt_of_remaining_messages_if_any()
    {
        var message1 = new Message(Guid.NewGuid(), Guid.NewGuid(), "abc", false, DateTime.UtcNow.AddSeconds(10));
        var message2 = new Message(Guid.NewGuid(), Guid.NewGuid(), "abc", false, DateTime.UtcNow);
        var messages = new List<Message>() { message1, message2 };
        var match = new Match(Guid.NewGuid(), message1.SendFromId, message2.SendFromId, DateTime.UtcNow, messages: messages);
        
        match.RemoveMessage(message1.Id);
        Assert.Single(match.Messages);
        Assert.Equal(message2.CreatedAt, match.LastActivityTime);
    }

    [Fact]
    public void given_message_in_Match_Messages_RemoveMessage_changes_LastActivityTime_to_match_CreatedAt_if_no_remaining_messages()
    {
        var message = new Message(Guid.NewGuid(), Guid.NewGuid(), "abc", false, DateTime.UtcNow.AddSeconds(10));
        var messages = new List<Message> { message };
        var match = new Match(Guid.NewGuid(), message.SendFromId, Guid.NewGuid(), DateTime.UtcNow, messages: messages);
        
        Assert.Single(match.Messages);
        match.RemoveMessage(message.Id);
        Assert.Equal(match.CreatedAt, match.LastActivityTime);
    }

    [Fact]
    public void given_message_not_in_Match_Messages_SetPreviousMessagesAsDisplayed_do_nothing()
    {
        var matchId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var messages = new List<Message> { 
            new Message(Guid.NewGuid(), userId, "abc", false, DateTime.UtcNow),
            new Message(Guid.NewGuid(), userId, "abc", false, DateTime.UtcNow),
            new Message(Guid.NewGuid(), userId, "abc", false, DateTime.UtcNow),
            new Message(Guid.NewGuid(), userId, "abc", false, DateTime.UtcNow),
            new Message(Guid.NewGuid(), userId, "abc", false, DateTime.UtcNow),
         };
        var match = new Match(matchId, userId, Guid.NewGuid(), DateTime.UtcNow, messages: messages);

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
        var lastMessage = new Message(Guid.NewGuid(), userId1, "abc", false, createdAt);
        var messages = new List<Message> { lastMessage };

        var match = new Match(matchId, userId1, userId2, DateTime.UtcNow, messages: messages);

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
        var lastMessage = new Message(Guid.NewGuid(), userId1, "abc", false, createdAt);
        var messages = new List<Message> { 
            lastMessage,
            new Message(Guid.NewGuid(), userId1, "abc", false, DateTime.UtcNow),
            new Message(Guid.NewGuid(), userId1, "abc", false, createdAt - TimeSpan.FromMinutes(1)),
            new Message(Guid.NewGuid(), userId1, "abc", true, createdAt - TimeSpan.FromHours(1)),
            new Message(Guid.NewGuid(), userId2, "abc", false, createdAt),
            new Message(Guid.NewGuid(), userId2, "abc", false, createdAt),
            new Message(Guid.NewGuid(), userId2, "abc", false, createdAt),
        };
        var match = new Match(matchId, userId1, userId2, DateTime.UtcNow, messages: messages);
        
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

    [Fact]
    public void given_user_id_is_not_in_Match_SetPreviousMessagesAsDisplayed_do_nothing()
    {
        var matchId = Guid.NewGuid();
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var userIdNotInMatch = Guid.NewGuid();

        var createdAt = DateTime.UtcNow - TimeSpan.FromSeconds(1);
        var lastMessage = new Message(Guid.NewGuid(), userId1, "abc", false, createdAt);
        var messages = new List<Message> { 
            lastMessage,
            new Message(Guid.NewGuid(), userId1, "abc", false, DateTime.UtcNow),
            new Message(Guid.NewGuid(), userId1, "abc", false, createdAt - TimeSpan.FromMinutes(1)),
            new Message(Guid.NewGuid(), userId1, "abc", true, createdAt - TimeSpan.FromHours(1)),
            new Message(Guid.NewGuid(), userId2, "abc", false, createdAt),
            new Message(Guid.NewGuid(), userId2, "abc", false, createdAt),
            new Message(Guid.NewGuid(), userId2, "abc", false, createdAt),
        };
        var match = new Match(matchId, userId1, userId2, DateTime.UtcNow, messages: messages);
        
        match.SetPreviousMessagesAsDisplayed(lastMessage.Id, userIdNotInMatch);
        Assert.Collection(match.Messages, 
            m => Assert.False(m.IsDisplayed),
            m => Assert.False(m.IsDisplayed),
            m => Assert.False(m.IsDisplayed),
            m => Assert.True(m.IsDisplayed),
            m => Assert.False(m.IsDisplayed),
            m => Assert.False(m.IsDisplayed),
            m => Assert.False(m.IsDisplayed)
        );
    }
}