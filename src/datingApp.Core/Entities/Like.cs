using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace datingApp.Core.Entities
{
    public class Like
    {
        public long Id { get; }
        
        [Required]
        public User LikedBy { get; private set; }

        [Required]
        public User LikedWho { get; private set; }

        [Required]
        public DateTime CreatedAt { get; private set; }

        public Like(User likedBy, User likedWho, DateTime createdAt)
        {
            LikedBy = likedBy;
            LikedWho = likedWho;
            CreatedAt = createdAt;
        }
    }
}