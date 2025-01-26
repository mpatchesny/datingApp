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
    public string SurrogateId { get; }
    public UserId UserId1 => _matchDetails[0].UserId;
    public UserId UserId2 => _matchDetails[1].UserId;
    public IEnumerable<User> Users { get; private set; } = new List<User>();
    public IEnumerable<Message> Messages => _messages;
    public IEnumerable<MatchDetail> MatchDetails => _matchDetails;
    public DateTime CreatedAt { get; private set; }
    public DateTime LastActivityTime { get; private set; }

    private readonly List<Message> _messages = new();
    private readonly List<MatchDetail> _matchDetails = new();

    private Match()
    {
        // EF
    }

    public Match(MatchId id, UserId userId1, UserId userId2, DateTime createdAt, bool isDisplayedByUser1=false, bool isDisplayedByUser2=false, List<Message> messages=null)
    {
        Id = id;

        SurrogateId =  new[] { userId1.Value, userId2.Value }
            .OrderBy(id => id)
            .Aggregate("", (acc, guid) => acc + guid.ToString("N"));

        _matchDetails.Add(new MatchDetail(Guid.NewGuid(), id, userId1, isDisplayedByUser1, messages));
        _matchDetails.Add(new MatchDetail(Guid.NewGuid(), id, userId2, isDisplayedByUser2, messages));
        _messages = messages ?? new List<Message>();
        CreatedAt = createdAt;
        LastActivityTime = _messages.Any() ? _messages.Max(msg => msg.CreatedAt) : CreatedAt;
    }

    public bool IsDisplayedByUser(UserId userId)
    {
        var detail = MatchDetails.FirstOrDefault(md => md.UserId == userId);
        return detail != null ? detail.IsDisplayed : false;
    }

    public void SetDisplayed(UserId userId)
    {
        var detail = _matchDetails.FirstOrDefault(md => md.UserId == userId);
        if (detail != null)
        {
            detail.SetDisplayed();
            foreach (var message in detail.Messages)
            {
                message.SetDisplayed();
            }
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
        LastActivityTime = message.CreatedAt;
    }

    public void RemoveMessage(MessageId messageId)
    {
        var detail = _matchDetails
            .Where(md => md.Messages.Any(message => message.Id == messageId))
            .FirstOrDefault();
        if (detail != null) detail.RemoveMessage(messageId);

        var message = _messages.FirstOrDefault(m => m.Id == messageId);
        if (message != null) _messages.Remove(message);

        LastActivityTime = CreatedAt;
        if (_messages.Any()) LastActivityTime = _messages.Max(msg => msg.CreatedAt);
    }

    public void SetPreviousMessagesAsDisplayed(MessageId lastMessageId, UserId displayedByUserId)
    {
        var userExists = _matchDetails.Any(md => md.UserId == displayedByUserId);
        if (!userExists) return;

        var lastMessage = _messages.FirstOrDefault(m => m.Id == lastMessageId);
        if (lastMessage == null) return;

        var detail = _matchDetails
            .Where(md => md.UserId != lastMessage.SendFromId)
            .FirstOrDefault();
        if (detail != null) detail.SetPreviousMessagesAsDisplayed(lastMessageId, displayedByUserId);
    }
}