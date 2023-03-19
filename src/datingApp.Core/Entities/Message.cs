using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Core.Entities;

public class Message
{
    public int Id { get; }
    public int SendFromId { get; private set; }
    public int SendToId { get; private set; }
    public string Text { get; private set; }
    public bool IsDisplayed { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Message(int id, int sendFromId, int sendToId, string text, bool isDisplayed, DateTime createdAt)
    {
        Id = id;
        SendFromId = sendFromId;
        SendToId = sendToId;
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
            throw new InvalidMessageException("message cannot exceed 255 characters in length");
        }
        if (Text == text) return;
        Text = text;
    }
}