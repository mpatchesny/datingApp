using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Core.Entities;

public class Swipe
{
    public Guid Id { get; }
    public Guid SwippedById { get; private set; }
    public Guid SwippedWhoId { get; private set; }
    public Like Like { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Swipe(Guid id, Guid swipedById, Guid swipedWhoId, Like like, DateTime createdAt)
    {
        Id = id;
        if (swipedById == swipedWhoId) throw new InvalidSwipeException();
        SwippedById = swipedById;
        SwippedWhoId = swipedWhoId;
        if (!Like.IsDefined(like)) throw new LikeValueNotDefinedException((int) like);
        Like = like;
        CreatedAt = createdAt;
    }
}