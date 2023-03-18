using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace datingApp.Core.Entities
{
    public class Match
    {
        public int Id { get; }
        public int User1 { get; private set; }
        public int User2 { get; private set; }
        public Message? LastMessage { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public Match(int id, int user1, int user2, Message? lastMessage, DateTime createdAt)
        {
            Id = id;
            User1 = user1;
            User2 = user2;
            LastMessage = lastMessage;
            CreatedAt = createdAt;
        }
    }
}