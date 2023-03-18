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
        
        [Required(ErrorMessage = "user1 is required")]
        public int User1 { get; private set; }

        [Required(ErrorMessage = "user2 is required")]
        public int User2 { get; private set; }

        public Message? LastMessage { get; private set; }

        [Required(ErrorMessage = "created at is required")]
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