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
        match.UserId1 == entity.UserId1 &&
        match.UserId2 == entity.UserId2 &&
        match.IsDisplayedByUser1 == entity.IsDisplayedByUser1 &&
        match.IsDisplayedByUser2 == entity.IsDisplayedByUser2 &&
        match.CreatedAt.Year == entity.CreatedAt.Year &&
        match.CreatedAt.Month == entity.CreatedAt.Month &&
        match.CreatedAt.Day == entity.CreatedAt.Day &&
        match.CreatedAt.Hour == entity.CreatedAt.Hour &&
        match.CreatedAt.Minute == entity.CreatedAt.Minute &&
        match.CreatedAt.Second == entity.CreatedAt.Second &&
        match.CreatedAt.Millisecond == entity.CreatedAt.Millisecond &&
        match.Messages.Count == entity.Messages.Count;

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
            message.CreatedAt.Year == entity.CreatedAt.Year &&
            message.CreatedAt.Month == entity.CreatedAt.Month &&
            message.CreatedAt.Day == entity.CreatedAt.Day &&
            message.CreatedAt.Hour == entity.CreatedAt.Hour &&
            message.CreatedAt.Minute == entity.CreatedAt.Minute &&
            message.CreatedAt.Second == entity.CreatedAt.Second &&
            message.CreatedAt.Millisecond == entity.CreatedAt.Millisecond;
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
            swipe.CreatedAt.Year == entity.CreatedAt.Year &&
            swipe.CreatedAt.Month == entity.CreatedAt.Month &&
            swipe.CreatedAt.Day == entity.CreatedAt.Day &&
            swipe.CreatedAt.Hour == entity.CreatedAt.Hour &&
            swipe.CreatedAt.Minute == entity.CreatedAt.Minute &&
            swipe.CreatedAt.Second == entity.CreatedAt.Second &&
            swipe.CreatedAt.Millisecond == entity.CreatedAt.Millisecond;
    }
}