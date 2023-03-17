using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace datingApp.Core.Entities
{
    public class Match
    {
        public long Id { get; }
        
        [Required]
        public User User1 { get; private set; }

        [Required]
        public User User2 { get; private set; }

        [Required]
        public DateTime CreatedAt { get; private set; }

        public Match(User user1, User user2, DateTime createdAt)
        {
            User1 = user1;
            User2 = user2;
            CreatedAt = createdAt;
        }
    }
}