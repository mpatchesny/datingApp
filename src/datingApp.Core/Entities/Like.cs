using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace datingApp.Core.Entities
{
    public class Like
    {
        public int Id { get; }
        
        [Required(ErrorMessage = "liked by is required")]
        public User LikedBy { get; private set; }

        [Required(ErrorMessage = "liked who is required")]
        public User LikedWho { get; private set; }

        [Required(ErrorMessage = "created at is required")]
        public DateTime CreatedAt { get; private set; }

        public Like(int id, User likedBy, User likedWho, DateTime createdAt)
        {
            Id = id;
            LikedBy = likedBy;
            LikedWho = likedWho;
            CreatedAt = createdAt;
        }
    }
}