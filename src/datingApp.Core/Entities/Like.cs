using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Core.Entities
{
    public class Like
    {
        public int Id { get; }
        public int LikedBy { get; private set; }
        public int LikedWho { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public Like(int id, int likedBy, int likedWho, DateTime createdAt)
        {
            Id = id;
            LikedBy = likedBy;
            LikedWho = likedWho;
            CreatedAt = createdAt;
        }
    }
}