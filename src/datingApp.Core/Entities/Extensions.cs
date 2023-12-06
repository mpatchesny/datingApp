using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Core.Entities;

public static class Extensions
{
    public static bool IsOwner(this User entity, Guid userId)
    {
        return entity.Id == userId;
    }
    public static bool IsOwner(this UserSettings entity, Guid userId)
    {
        return entity.UserId == userId;
    }
    public static bool IsOwner(this Match entity, Guid userId)
    {
        return entity.UserId1 == userId || entity.UserId2 == userId;
    }
    public static bool IsOwner(this Message entity, Guid userId)
    {
        return entity.SendFromId == userId;
    }
    public static bool IsOwner(this Photo entity, Guid userId)
    {
        return entity.UserId == userId;
    }
    public static bool IsOwner(this Swipe entity, Guid userId)
    {
        return entity.SwipedById == userId;
    }
}