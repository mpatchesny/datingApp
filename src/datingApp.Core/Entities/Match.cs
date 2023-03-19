using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Core.Entities;

public class Match
{
    public int Id { get; }
    public int UserId1 { get; private set; }
    public int UserId2 { get; private set; }
    public Message? LastMessage { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Match(int id, int userId1, int userId2, Message? lastMessage, DateTime createdAt)
    {
        Id = id;
        UserId1 = userId1;
        UserId2 = userId2;
        LastMessage = lastMessage;
        CreatedAt = createdAt;
    }
}