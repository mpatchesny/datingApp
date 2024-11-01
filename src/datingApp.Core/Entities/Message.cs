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

    public Message(MessageId id, UserId sendFromId, MessageText text, bool isDisplayed, DateTime createdAt)
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

    public override bool Equals(object obj)
    {
        if (obj is not Message) return false;
        Message message = (Message) obj;
        return message.Id == Id && 
            message.SendFromId == SendFromId &&
            message.IsDisplayed == IsDisplayed &&
            message.Text == Text &&
            message.CreatedAt.Year == CreatedAt.Year &&
            message.CreatedAt.Month == CreatedAt.Month &&
            message.CreatedAt.Day == CreatedAt.Day &&
            message.CreatedAt.Hour == CreatedAt.Hour &&
            message.CreatedAt.Minute == CreatedAt.Minute &&
            message.CreatedAt.Second == CreatedAt.Second &&
            message.CreatedAt.Millisecond == CreatedAt.Millisecond;
    }
}