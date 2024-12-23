using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;

namespace datingApp.Core.Entities;

public static class Extensions
{
    public static bool IsEqualTo(this User entity, object obj)
    {
        if (obj is not User) return false;
        User user = (User) obj;
        var equals = user.Id == entity.Id && 
            user.Phone == entity.Phone &&
            user.Email == entity.Email &&
            user.Name == entity.Name &&
            user.Sex == entity.Sex &&
            user.DateOfBirth == entity.DateOfBirth &&
            user.Job == entity.Job &&
            user.Bio == entity.Bio &&
            user.Photos.Count == entity.Photos.Count;
        
        if (user.Settings != null && entity.Settings !=null)
        {
            equals = equals && user.Settings.IsEqualTo(entity.Settings);
        }
        else if (!(user.Settings == null && entity.Settings ==null))
        {
            equals = false;
        }


        if (equals)
        {
            var photos1 = user.Photos.OrderBy(p => p.Id.Value).ToList();
            var photos2 = entity.Photos.OrderBy(p => p.Id.Value).ToList();
            for (int i = 0; i < photos1.Count; i++)
            {
                equals = equals && photos1[i].IsEqualTo(photos2[i]);
            }
        }

        return equals;
    }

    public static bool IsEqualTo(this Match entity, object obj)
    {
        if (obj is not Match) return false;
        Match match = (Match) obj;
        var equals = match.Id == entity.Id && 
        (match.UserId1 == entity.UserId1 || match.UserId1 == entity.UserId2) &&
        (match.UserId2 == entity.UserId1 || match.UserId2 == entity.UserId2) &&
        match.IsDisplayedByUser(match.UserId1) == entity.IsDisplayedByUser(match.UserId1) &&
        match.IsDisplayedByUser(match.UserId2) == entity.IsDisplayedByUser(match.UserId2) &&
        match.CreatedAt.IsEqualTo(entity.CreatedAt) &&
        match.LastActivityTime.IsEqualTo(entity.LastActivityTime);

        if (equals)
        {
            var messages1 = match.Messages.OrderBy(m => m.Id.Value).ToList();
            var messages2 = entity.Messages.OrderBy(m => m.Id.Value).ToList();
            for (int i = 0; i < messages1.Count; i++)
            {
                equals = equals && messages1[i].IsEqualTo(messages2[i]);
            }
        }

        return equals;
    }

    public static bool IsEqualTo(this UserSettings entity, object obj)
    {
        if (obj is not UserSettings) return false;
        UserSettings userSettings = (UserSettings) obj;
        return userSettings.UserId == entity.UserId && 
            userSettings.PreferredSex == entity.PreferredSex &&
            userSettings.PreferredAge == entity.PreferredAge &&
            userSettings.PreferredMaxDistance == entity.PreferredMaxDistance &&
            userSettings.Location == entity.Location;
    }

    public static bool IsEqualTo(this Message entity, object obj)
    {
        if (obj is not Message) return false;
        Message message = (Message) obj;
        return message.Id == entity.Id && 
            message.SendFromId == entity.SendFromId &&
            message.IsDisplayed == entity.IsDisplayed &&
            message.Text == entity.Text &&
            message.CreatedAt.IsEqualTo(entity.CreatedAt);
    }

    public static bool IsEqualTo(this Photo entity, object obj)
    {
        if (obj is not Photo) return false;
        Photo photo = (Photo) obj;
        return photo.Id == entity.Id && 
            photo.Url == entity.Url &&
            photo.Extension == entity.Extension;
    }

    public static bool IsEqualTo(this Swipe entity, object obj)
    {
        if (obj is not Swipe) return false;
        Swipe swipe = (Swipe) obj;
        return swipe.SwipedById == entity.SwipedById && 
            swipe.SwipedWhoId == entity.SwipedWhoId &&
            swipe.Like == entity.Like &&
            swipe.CreatedAt.IsEqualTo(entity.CreatedAt);
    }

    private static bool IsEqualTo(this DateTime dateTime, DateTime otherDatetime)
    {
        return dateTime.Year == otherDatetime.Year &&
            dateTime.Month == otherDatetime.Month &&
            dateTime.Day == otherDatetime.Day &&
            dateTime.Hour == otherDatetime.Hour &&
            dateTime.Minute == otherDatetime.Minute &&
            dateTime.Second == otherDatetime.Second &&
            dateTime.Millisecond == otherDatetime.Millisecond;
    }
}