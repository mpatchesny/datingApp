using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Core.Entities;

public class Swipe
{
    public int Id { get; }
    public Guid SwippedById { get; private set; }
    public Guid SwippedWhoId { get; private set; }
    public Like Like { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Swipe(int id, Guid swippedById, Guid swippedWhoId, Like like, DateTime createdAt)
    {
        Id = id;
        if (swippedById == swippedWhoId) throw new InvalidSwipeException();
        SwippedById = swippedById;
        SwippedWhoId = swippedWhoId;
        if (!Like.IsDefined(like)) throw new LikeValueNotDefinedException((int) like);
        Like = like;
        CreatedAt = createdAt;
    }
}