using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Core.Entities;

public class Message
{
    public int Id { get; }
    public int MatchId { get; private set; }
    public int SendFromId { get; private set; }
    public string Text { get; private set; }
    public bool IsDisplayed { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Message(int id, int matchId, int sendFromId, string text, bool isDisplayed, DateTime createdAt)
    {
        Id = id;
        MatchId = matchId;
        SendFromId = sendFromId;
        SetText(text);
        IsDisplayed = IsDisplayed;
        CreatedAt = createdAt;
    }

    public void SetDisplayed()
    {
        IsDisplayed = true;
    }

    private void SetText(string text)
    {
        if (text.Length == 0)
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