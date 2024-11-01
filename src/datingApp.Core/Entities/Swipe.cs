using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Consts;
using datingApp.Core.Exceptions;
using datingApp.Core.ValueObjects;

namespace datingApp.Core.Entities;

public class Swipe
{
    public UserId SwipedById { get; private set; }
    public UserId SwipedWhoId { get; private set; }
    public Like Like { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Swipe()
    {
        // EF
    }

    public Swipe(UserId swipedById, UserId swipedWhoId, Like like, DateTime createdAt)
    {
        if (swipedById == swipedWhoId) throw new InvalidSwipeException();
        if (!Enum.IsDefined(typeof(Like), like)) throw new LikeValueNotDefinedException((int) like);

        SwipedById = swipedById;
        SwipedWhoId = swipedWhoId;
        Like = like;
        CreatedAt = createdAt;
    }

    public override bool Equals(object obj)
    {
        if (obj is not Swipe) return false;
        Swipe swipe = (Swipe) obj;
        return swipe.SwipedById == SwipedById && 
            swipe.SwipedWhoId == SwipedWhoId &&
            swipe.Like == Like &&
            swipe.CreatedAt.Year == CreatedAt.Year &&
            swipe.CreatedAt.Month == CreatedAt.Month &&
            swipe.CreatedAt.Day == CreatedAt.Day &&
            swipe.CreatedAt.Hour == CreatedAt.Hour &&
            swipe.CreatedAt.Minute == CreatedAt.Minute &&
            swipe.CreatedAt.Second == CreatedAt.Second &&
            swipe.CreatedAt.Millisecond == CreatedAt.Millisecond;
    }
}