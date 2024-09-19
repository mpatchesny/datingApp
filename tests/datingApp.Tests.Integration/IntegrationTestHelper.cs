using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace datingApp.Tests.Integration;

internal static class IntegrationTestHelper
{
    public static async Task<User> CreateUserAsync(TestDatabase database, string email = null, string phone = null)
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

    internal static async Task<Photo> CreatePhotoAsync(TestDatabase database, Guid userId)
    {
        var photo = new Photo(Guid.NewGuid(), userId, "abc", "abc", 1);
        await database.DbContext.Photos.AddAsync(photo);
        await database.DbContext.SaveChangesAsync();
        return photo;
    }
}