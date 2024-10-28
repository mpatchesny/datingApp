using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.ValueObjects;

namespace datingApp.Core.Entities;

public class Match
{
    public Guid Id { get; }
    public UserId UserId1 { get; private set; }
    public UserId UserId2 { get; private set; }
    public bool IsDisplayedByUser1 { get; private set; }
    public bool IsDisplayedByUser2 { get; private set; }
    public IEnumerable<Message> Messages { get; private set; } = new List<Message>();
    public DateTime CreatedAt { get; private set; }

    private Match()
    {
        // EF
    }

    public Match(Guid id, UserId userId1, UserId userId2, bool isDisplayedByUser1, bool isDisplayedByUser2, IEnumerable<Message> messages, DateTime createdAt)
    {
        Id = id;
        UserId1 = userId1;
        UserId2 = userId2;
        IsDisplayedByUser1 = isDisplayedByUser1;
        IsDisplayedByUser2 = isDisplayedByUser2;
        Messages = messages;
        CreatedAt = createdAt;
    }

    public void SetDisplayed(Guid userId)
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
}