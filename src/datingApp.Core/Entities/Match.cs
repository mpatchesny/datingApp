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
    public bool IsDisplayedByUser1 => MatchDetails.ElementAt(0).IsDisplayed;
    public bool IsDisplayedByUser2 => MatchDetails.ElementAt(1).IsDisplayed;
    public IEnumerable<Message> Messages => _messages;
    public IEnumerable<User> Users => new List<User>();
    private IEnumerable<MatchDetail> MatchDetails => _matchDetails;
    public DateTime CreatedAt { get; private set; }

    private readonly List<Message> _messages = new();
    private readonly List<MatchDetail> _matchDetails = new();

    private Match()
    {
        // EF
    }

    public Match(MatchId id, UserId userId1, UserId userId2, DateTime createdAt, bool isDisplayedByUser1=false, bool isDisplayedByUser2=false, List<Message> messages=null)
    {
        Id = id;
        UserId1 = userId1;
        UserId2 = userId2;
        _matchDetails.Add(new MatchDetail(id, userId1, isDisplayedByUser1, messages));
        _matchDetails.Add(new MatchDetail(id, userId2, isDisplayedByUser2, messages));
        _messages = messages ?? new List<Message>();
        CreatedAt = createdAt;
    }

    public bool IsDisplayedByUser(UserId userId)
    {
        var detail = MatchDetails.FirstOrDefault(md => md.UserId == userId);
        if (detail != null)
        {
            return detail.IsDisplayed;
        }
        else
        {
            return false;
        }
    }

    public void SetDisplayed(UserId userId)
    {
        var detail = _matchDetails.FirstOrDefault(md => md.UserId == userId);
        if (detail != null)
        {
            detail.SetDisplayed();
        }
    }

    public void AddMessage(Message message)
    {
        if (_messages.Any(m => m.Id == message.Id)) return;

        if (message.SendFromId != UserId1 && message.SendFromId != UserId2)
        {
            throw new MessageSenderNotMatchMatchUsers();
        }

        var detail = _matchDetails.FirstOrDefault(md => md.UserId == message.SendFromId);
        if (detail != null) detail.AddMessage(message);
        _messages.Add(message);
    }

    public void RemoveMessage(MessageId messageId)
    {
        var detail = _matchDetails
            .Where(md => md.Messages.Any(message => message.Id == messageId))
            .FirstOrDefault();
        if (detail != null) detail.RemoveMessage(messageId);

        var message = _messages.FirstOrDefault(m => m.Id == messageId);
        if (message != null) _messages.Remove(message);
    }

    public void SetPreviousMessagesAsDisplayed(MessageId lastMessageId, UserId displayedByUserId)
    {
        var lastMessage = _messages.FirstOrDefault(m => m.Id == lastMessageId);
        if (lastMessage == null) return;

        foreach (var message in _messages)
        {
            if (message.CreatedAt <= lastMessage.CreatedAt && message.SendFromId != displayedByUserId)
            {
                message.SetDisplayed();
            }
        }

        var detail = _matchDetails
            .Where(md => md.UserId != lastMessage.SendFromId)
            .FirstOrDefault();
        if (detail != null) detail.SetPreviousMessagesAsDisplayed(lastMessageId, displayedByUserId);
    }
}