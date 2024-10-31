using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.ValueObjects;
using Microsoft.Extensions.WebEncoders.Testing;

namespace datingApp.Core.Entities;

public class Message
{
    public MessageId Id { get; }
    public UserId SendFromId { get; private set; }
    public MessageText Text { get; private set; }
    public bool IsDisplayed { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Message()
    {
        // EF
    }

    public Message(MessageId id, MatchId matchId, UserId sendFromId, MessageText text, bool isDisplayed, DateTime createdAt)
    {
        Id = id;
        SendFromId = sendFromId;
        Text = text;
        IsDisplayed = isDisplayed;
        CreatedAt = createdAt;
    }

    public void SetDisplayed()
    {
        IsDisplayed = true;
    }
}