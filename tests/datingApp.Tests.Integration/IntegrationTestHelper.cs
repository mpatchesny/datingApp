using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Core.Consts;
using datingApp.Core.Entities;
using datingApp.Core.ValueObjects;
using datingApp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Xunit.Sdk;

namespace datingApp.Tests.Integration;

internal static class IntegrationTestHelper
{
    #region User
    internal static User CreateUser(ICollection<Photo> photos = null, string email = null, string phone = null)
    {
        Random random = new Random();
        if (email == null) email = "test_" + Guid.NewGuid().ToString().Replace("-", "") + "@test.com";
        if (phone == null) phone = random.Next(100000000, 999999999).ToString();
        var settings = new UserSettings(Guid.NewGuid(), PreferredSex.MaleAndFemale, new PreferredAge(18, 100), 100, new Location(45.5, 45.5));
        var user = new User(settings.UserId, phone, email, "Janusz", new DateOnly(2000,1,1), UserSex.Male, settings, DateTime.UtcNow, photos: photos);
        return user;
    }

    internal static async Task<User> CreateUserAsync(DatingAppDbContext dbContext, ICollection<Photo> photos = null, string email = null, string phone = null)
    {
        var user = CreateUser(photos, email, phone);
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();
        return user;
    }

    internal static async Task<User> CreateUserAsync(DatingAppDbContext dbContext, User user)
    {
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();
        return user;
    }

    internal static async Task DeleteUserAsync(DatingAppDbContext dbContext, User user)
    {
        dbContext.Users.Remove(user);
        await dbContext.DeletedEntities.AddAsync(new DeletedEntityDto() { Id = user.Id, EntityType = "user", DeletedAt = DateTime.UtcNow });
        await dbContext.SaveChangesAsync();
    }

    internal static Photo CreatePhoto(int oridinal = 0, string checksum = "checksum")
    {
        var photo = new Photo(Guid.NewGuid(), "url", checksum, oridinal);
        return photo;
    }

    internal static async Task DeletePhotoAsync(DatingAppDbContext dbContext, Photo photo)
    {
        dbContext.Photos.Remove(photo);
        await dbContext.DeletedEntities.AddAsync(new DeletedEntityDto() { Id = photo.Id, EntityType = "photo", DeletedAt = DateTime.UtcNow });
        await dbContext.SaveChangesAsync();
    }
    #endregion

    #region Match
    internal static Message CreateMessage(Guid sendFromId, bool isDisplayed = false, DateTime? createdAt = null, string text="test")
    {
        return new Message(Guid.NewGuid(), sendFromId, text, isDisplayed, createdAt ?? DateTime.UtcNow);
    }

    internal static async Task<Match> CreateMatchAsync(DatingAppDbContext dbContext, Guid userId1, Guid userId2, List<Message> messages = null, bool isDisplayedByUser1 = false, bool isDisplayedByUser2 = false, DateTime? createdAt = null)
    {
        var match = new Match(Guid.NewGuid(), userId1, userId2, createdAt ?? DateTime.UtcNow, isDisplayedByUser1, isDisplayedByUser2, messages);
        await dbContext.Matches.AddAsync(match);
        await dbContext.SaveChangesAsync();
        return match;
    }

    internal static async Task DeleteMatchAsync(DatingAppDbContext dbContext, Match match)
    {
        dbContext.Matches.Remove(match);
        await dbContext.DeletedEntities.AddAsync(new DeletedEntityDto() { Id = match.Id, EntityType = "match", DeletedAt = DateTime.Now });
        await dbContext.SaveChangesAsync();
    }
    #endregion

    internal static Swipe CreateSwipe(Guid swipedById, Guid swipedWhoId, Like like = Like.Like, DateTime? createdAt = null)
    {
        return new Swipe(swipedById, swipedWhoId, like, createdAt ?? DateTime.UtcNow);
    }

    internal static async Task<Swipe> CreateSwipeAsync(DatingAppDbContext dbContext, Guid swipedById, Guid swipedWhoId, Like like, DateTime? createdAt = null)
    {
        var swipe = new Swipe(swipedById, swipedWhoId, like, createdAt ?? DateTime.UtcNow);
        await dbContext.Swipes.AddAsync(swipe);
        await dbContext.SaveChangesAsync();
        return swipe;
    }

    internal static MemoryStream SamplePhotoStream()
    {
        var base64 = "iVBORw0KGgoAAAANSUhEUgAAAAoAAAAKCAIAAAACUFjqAAABhmlDQ1BJQ0MgcHJvZmlsZQAAKJF9kT1Iw0AcxV9TpVIrDnYQcchQBcGCqIjgIlUsgoXSVmjVweTSD6FJQ5Li4ii4Fhz8WKw6uDjr6uAqCIIfIM4OToouUuL/kkKLGA+O+/Hu3uPuHSDUy0w1O8YAVbOMVDwmZnMrYuAVQXQjjBHMSMzUE+mFDDzH1z18fL2L8izvc3+OHiVvMsAnEs8y3bCI14mnNi2d8z5xmJUkhficeNSgCxI/cl12+Y1z0WGBZ4aNTGqOOEwsFttYbmNWMlTiSeKIomqUL2RdVjhvcVbLVda8J39hKK8tp7lOcxBxLCKBJETIqGIDZViI0qqRYiJF+zEP/4DjT5JLJtcGGDnmUYEKyfGD/8Hvbs3CxLibFIoBnS+2/TEEBHaBRs22v49tu3EC+J+BK63lr9SB6U/Say0tcgT0bgMX1y1N3gMud4D+J10yJEfy0xQKBeD9jL4pB/TdAsFVt7fmPk4fgAx1tXQDHBwCw0XKXvN4d1d7b/+eafb3A/RZcttv9j1bAAAACXBIWXMAAC4jAAAuIwF4pT92AAAAB3RJTUUH6AoLFCsTQpMb9AAAABl0RVh0Q29tbWVudABDcmVhdGVkIHdpdGggR0lNUFeBDhcAAAAVSURBVBjTY/z37x8DbsDEgBeMVGkAokcDDrmMttcAAAAASUVORK5CYII=";
        var bytes = Convert.FromBase64String(base64);
        return new MemoryStream(bytes);
    }
}