using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.ValueObjects;

namespace datingApp.Core.Entities;

public class Message
{
    public Guid Id { get; }
    public MatchId MatchId { get; private set; }
    public UserId SendFromId { get; private set; }
    public string Text { get; private set; }
    public bool IsDisplayed { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Message(Guid id, MatchId matchId, UserId sendFromId, string text, bool isDisplayed, DateTime createdAt)
    {
        Id = id;
        MatchId = matchId;
        SendFromId = sendFromId;
        SetText(text);
        IsDisplayed = isDisplayed;
        CreatedAt = createdAt;
    }

    public void SetDisplayed()
    {
        IsDisplayed = true;
    }

    private void SetText(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            throw new InvalidMessageException("message cannot be empty");
        }
        if (text.Length > 255)
        {
            throw new InvalidMessageException("message too long");
        }
        if (Text == text) return;
        Text = text;
    }
}