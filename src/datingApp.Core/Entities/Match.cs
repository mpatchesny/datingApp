using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;
using datingApp.Core.ValueObjects;

namespace datingApp.Core.Entities;

public class Match
{
    public MatchId Id { get; }
    public UserId UserId1 { get; private set; }
    public UserId UserId2 { get; private set; }
    public bool IsDisplayedByUser1 { get; private set; }
    public bool IsDisplayedByUser2 { get; private set; }
    public IEnumerable<Message> Messages => _messages;
    public DateTime CreatedAt { get; private set; }

    private readonly List<Message> _messages = new();

    private Match()
    {
        // EF
    }

    public Match(MatchId id, UserId userId1, UserId userId2, bool isDisplayedByUser1, bool isDisplayedByUser2, List<Message> messages, DateTime createdAt)
    {
        Id = id;
        UserId1 = userId1;
        UserId2 = userId2;
        IsDisplayedByUser1 = isDisplayedByUser1;
        IsDisplayedByUser2 = isDisplayedByUser2;
        _messages = messages ?? new List<Message>();
        CreatedAt = createdAt;
    }

    public bool IsDisplayedByUser(UserId userId)
    {
        if (UserId1.Equals(userId))
        {
            return IsDisplayedByUser1;
        }
        else if (UserId2.Equals(userId))
        {
            return IsDisplayedByUser2;
        }
        else
        {
            return false;
        }
    }

    public void SetDisplayed(UserId userId)
    {
        if (UserId1.Equals(userId))
        {
            IsDisplayedByUser1 = true;
        }
        else if (UserId2.Equals(userId))
        {
            IsDisplayedByUser2 = true;
        }
    }

    public void AddMessage(Message message)
    {
        if (Messages.Any(m => m.Id == message.Id)) return;

        if (message.SendFromId != UserId1 && message.SendFromId != UserId2)
        {
            throw new MessageSenderNotMatchMatchUsers();
        }

        _messages.Add(message);
    }

    public void RemoveMessage(MessageId messageId)
    {
        var message = Messages.FirstOrDefault(m => m.Id == messageId);
        if (message != null) 
        {
            _messages.Remove(message);
        }
    }

    public void SetPreviousMessagesAsDisplayed(MessageId lastMessageId, UserId displayedByUserId)
    {
        var lastMessage = Messages.FirstOrDefault(m => m.Id == lastMessageId);
        if (lastMessage == null) return;

        foreach (var message in Messages)
        {
            if (message.CreatedAt <= lastMessage.CreatedAt && message.SendFromId != displayedByUserId)
            {
                message.SetDisplayed();
            }
        }
    }
}