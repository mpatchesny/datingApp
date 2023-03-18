using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Core.Entities
{
    public class Like
    {
        public int Id { get; }
        public int LikedById { get; private set; }
        public int LikedWhoId { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public Like(int id, int likedById, int likedWhoId, DateTime createdAt)
        {
            Id = id;
            LikedById = likedById;
            LikedWhoId = likedWhoId;
            CreatedAt = createdAt;
        }
    }
}