using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Core.Entities;

public class Swipe
{
    public int Id { get; }
    public int SwippedById { get; private set; }
    public int SwippedWhoId { get; private set; }
    public Like Like { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Swipe(int id, int swippedById, int swippedWhoId, Like like, DateTime createdAt)
    {
        Id = id;
        SwippedById = swippedById;
        SwippedWhoId = swippedWhoId;
        Like = like;
        CreatedAt = createdAt;
    }
}