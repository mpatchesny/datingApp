using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Core.Entities;
using Microsoft.EntityFrameworkCore.Storage;
using Xunit.Sdk;

namespace datingApp.Tests.Integration;

internal static class IntegrationTestHelper
{
    internal static async Task<User> CreateUserAsync(TestDatabase database, string email = null, string phone = null)
    {
        Random random = new Random();
        if (email == null) email = "test_" + random.Next(1, 9).ToString() + "@test.com";
        if (phone == null) phone = random.Next(100000000, 999999999).ToString();

        var settings = new UserSettings(Guid.NewGuid(), Sex.MaleAndFemale, 18, 100, 100, 45.5, 45.5);
        var user = new User(settings.UserId, phone, email, "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);

        await database.DbContext.Users.AddAsync(user);
        await database.DbContext.SaveChangesAsync();
        return user;
    }

    internal static async Task<User> CreateUserAsync(TestDatabase database, User user)
    {
        await database.DbContext.Users.AddAsync(user);
        await database.DbContext.SaveChangesAsync();
        return user;
    }

    internal static async Task<Photo> CreatePhotoAsync(TestDatabase database, Guid userId, int oridinal = 1)
    {
        var photo = new Photo(Guid.NewGuid(), userId, "abc", "abc", oridinal);
        await database.DbContext.Photos.AddAsync(photo);
        await database.DbContext.SaveChangesAsync();
        return photo;
    }
   
    internal static async Task<Match> CreateMatchAsync(TestDatabase database, Guid userId1, Guid userId2, bool isDisplayedByUser1 = false, bool isDisplayedByUser2 = false, DateTime? createdAt = null)
    {
        var match = new Match(Guid.NewGuid(), userId1, userId2, isDisplayedByUser1, isDisplayedByUser2, null, createdAt ?? DateTime.UtcNow);
        await database.DbContext.Matches.AddAsync(match);
        await database.DbContext.SaveChangesAsync();
        return match;
    }

    internal static async Task<Message> CreateMessageAsync(TestDatabase database, Guid matchId, Guid sendFromId, DateTime? createdAt = null)
    {
        var message = new Message(Guid.NewGuid(), matchId, sendFromId, "test", false, createdAt ?? DateTime.UtcNow);
        await database.DbContext.Messages.AddAsync(message);
        await database.DbContext.SaveChangesAsync();
        return message;
    }

    internal static async Task<Swipe> CreateSwipeAsync(TestDatabase database, Guid swipedById, Guid swipedWhoId, Like like, DateTime? createdAt = null)
    {
        var swipe = new Swipe(Guid.NewGuid(), swipedById, swipedWhoId, like, createdAt ?? DateTime.UtcNow);
        await database.DbContext.Swipes.AddAsync(swipe);
        await database.DbContext.SaveChangesAsync();
        return swipe;
    }

    internal static async Task DeleteMatchAsync(TestDatabase database, Match match)
    {
        database.DbContext.Matches.Remove(match);
        await database.DbContext.DeletedEntities.AddAsync(new DeletedEntityDto() { Id = match.Id });
        await database.DbContext.SaveChangesAsync();
    }

    internal static async Task DeleteUserAsync(TestDatabase database, User user)
    {
        database.DbContext.Users.Remove(user);
        await database.DbContext.DeletedEntities.AddAsync(new DeletedEntityDto() { Id = user.Id });
        await database.DbContext.SaveChangesAsync();
    }

    internal static async Task DeletePhotoAsync(TestDatabase database, Photo photo)
    {
        database.DbContext.Photos.Remove(photo);
        await database.DbContext.DeletedEntities.AddAsync(new DeletedEntityDto() { Id = photo.Id });
        await database.DbContext.SaveChangesAsync();
    }
}