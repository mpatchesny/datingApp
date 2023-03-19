using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Core.Entities
{
    public class Swap
    {
        public int Id { get; }
        public int SwappedById { get; private set; }
        public int SwappedWhoId { get; private set; }
        public Like Like { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public Swap(int id, int swappedById, int swappedWhoId, Like like, DateTime createdAt)
        {
            Id = id;
            SwappedById = swappedById;
            SwappedWhoId = swappedWhoId;
            Like = like;
            CreatedAt = createdAt;
        }
    }
}