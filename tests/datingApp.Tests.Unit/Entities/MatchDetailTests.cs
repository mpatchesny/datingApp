using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using Xunit;

namespace datingApp.Tests.Unit.Entities;

public class MatchDetailTests
{
    [Fact]
    public void SetDisplayed_changes_enitity_state()
    {
        var matchDetail = new MatchDetail(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        matchDetail.SetDisplayed();
        Assert.True(matchDetail.IsDisplayed);
    }

    [Fact]
    public void AddMessage_adds_message()
    {
        var matchDetail = new MatchDetail(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var message = new Message(Guid.NewGuid(), matchDetail.UserId, "abc", false, DateTime.UtcNow);
        
        Assert.Empty(matchDetail.Messages);
        matchDetail.AddMessage(message);
        Assert.Single(matchDetail.Messages);
    }

    [Fact]
    public void given_message_already_in_Match_Messages_AddMessage_adds_message()
    {
        var userId = Guid.NewGuid();
        var messages = new List<Message>(){
            new Message(Guid.NewGuid(), userId, "abc", false, DateTime.UtcNow)
        };
        var matchDetail = new MatchDetail(Guid.NewGuid(), Guid.NewGuid(), userId, messages: messages);
        
        Assert.Single(matchDetail.Messages);
        matchDetail.AddMessage(messages[0]);
        Assert.Equal(2, matchDetail.Messages.Count());
    }

    [Fact]
    public void given_message_in_MatchDetail_Messages_RemoveMessage_removes_message()
    {
        var userId = Guid.NewGuid();
        var messages = new List<Message>(){
            new Message(Guid.NewGuid(), userId, "abc", false, DateTime.UtcNow)
        };
        var matchDetail = new MatchDetail(Guid.NewGuid(), Guid.NewGuid(), userId, messages: messages);
        
        Assert.Single(matchDetail.Messages);
        matchDetail.RemoveMessage(messages[0].Id);
        Assert.Empty(matchDetail.Messages);
    }

    [Fact]
    public void given_message_not_in_MatchDetail_Messages_RemoveMessage_do_nothing()
    {
        var userId = Guid.NewGuid();
        var messages = new List<Message>(){
            new Message(Guid.NewGuid(), userId, "abc", false, DateTime.UtcNow)
        };
        var matchDetail = new MatchDetail(Guid.NewGuid(), Guid.NewGuid(), userId, messages: messages);
        
        Assert.Single(matchDetail.Messages);
        matchDetail.RemoveMessage(Guid.NewGuid());
        Assert.Single(matchDetail.Messages);
    }

    [Fact]
    public void given_message_not_in_MatchDetail_Messages_SetPreviousMessagesAsDisplayed_do_nothing()
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
        var matchDetail = new MatchDetail(Guid.NewGuid(), Guid.NewGuid(), userId, messages: messages);

        var exception = Record.Exception(() => matchDetail.SetPreviousMessagesAsDisplayed(Guid.NewGuid(), matchDetail.UserId));
        Assert.Null(exception);
        Assert.Collection(matchDetail.Messages, 
            m => Assert.False(m.IsDisplayed),
            m => Assert.False(m.IsDisplayed),
            m => Assert.False(m.IsDisplayed),
            m => Assert.False(m.IsDisplayed),
            m => Assert.False(m.IsDisplayed)
        );
    }

    [Fact]
    public void SetPreviousMessagesAsDisplayed_sets_messages_as_displayed_if_message_is_created_before_last_message_and_send_from_is_different_than_displayedUser()
    {
        var matchId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var createdAt = DateTime.UtcNow - TimeSpan.FromSeconds(1);
        var lastMessage = new Message(Guid.NewGuid(), userId, "abc", false, createdAt);
        var messages = new List<Message> { lastMessage };
        var matchDetail = new MatchDetail(Guid.NewGuid(), Guid.NewGuid(), userId, messages: messages);

        matchDetail.SetPreviousMessagesAsDisplayed(lastMessage.Id, Guid.NewGuid());
        Assert.Collection(matchDetail.Messages, 
            m => Assert.True(m.IsDisplayed)
        );
    }
}