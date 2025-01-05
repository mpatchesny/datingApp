using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.ValueObjects;

namespace datingApp.Core.Entities;

public class MatchDetail
{
    public MatchDetailId Id { get; }
    public MatchId MatchId { get; private set; }
    public UserId UserId { get; private set; }
    public bool IsDisplayed { get; private set; }
    public IEnumerable<Message> Messages => _messages;

    private readonly List<Message> _messages = new();

    private MatchDetail()
    {
    }

    public MatchDetail(MatchDetailId id, MatchId matchId, UserId userId, bool isDisplayed=false, List<Message> messages=null)
    {
        Id = id;
        MatchId = matchId;
        UserId = userId;
        IsDisplayed = isDisplayed;
        _messages = messages ?? new List<Message>();
    }

    internal void SetDisplayed()
    {
        IsDisplayed = true;
    }

    internal void AddMessage(Message message)
    {
        _messages.Add(message);
    }

    internal void RemoveMessage(MessageId messageId)
    {
        var message = _messages.FirstOrDefault(m => m.Id == messageId);
        if (message != null) _messages.Remove(message);
    }

    internal void SetPreviousMessagesAsDisplayed(MessageId lastMessageId, UserId displayedByUserId)
    {
        var lastMessage = _messages.FirstOrDefault(m => m.Id == lastMessageId);
        if (lastMessage == null) return;

        foreach (var message in Messages)
        {
            if (message.CreatedAt <= lastMessage.CreatedAt && 
                message.SendFromId != displayedByUserId &&
                !message.IsDisplayed)
            {
                message.SetDisplayed();
            }
        }
    }
}