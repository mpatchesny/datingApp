using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using Xunit;

namespace datingApp.Tests.Unit.Entities;

public class ExtensionsTests
{
    [Fact]
    public void user_is_owner_should_return_true_when_user_id_match()
    {
        var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var userSettings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000001"), Sex.Female, 18, 20, 20, 45.5, 45.5);
        var user = new User(userId, "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), Sex.Female, null, userSettings);
        Assert.True(user.IsOwner(userId));
    }

    [Fact]
    public void user_is_owner_should_return_false_when_user_id_dont_match()
    {
        var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var userId2 = Guid.Parse("00000000-0000-0000-0000-000000000002");
        var userSettings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000001"), Sex.Female, 18, 20, 20, 45.5, 45.5);
        var user = new User(userId, "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), Sex.Female, null, userSettings);
        Assert.False(user.IsOwner(userId2));
    }

    [Fact]
    public void user_settings_is_owner_should_return_true_when_user_id_match()
    {
        var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var userSettings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000001"), Sex.Female, 18, 20, 20, 45.5, 45.5);
        Assert.True(userSettings.IsOwner(userId));
    }

    [Fact]
    public void user_settings_is_owner_should_return_false_when_user_id_dont_match()
    {
        var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var userId2 = Guid.Parse("00000000-0000-0000-0000-000000000002");
        var userSettings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000001"), Sex.Female, 18, 20, 20, 45.5, 45.5);
        Assert.False(userSettings.IsOwner(userId2));
    }

    [Fact]
    public void match_is_owner_should_return_true_when_user_id_match_userid1_or_userid2()
    {
        var userId1 = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var userId2 = Guid.Parse("00000000-0000-0000-0000-000000000002");
        var match = new Match(Guid.Parse("00000000-0000-0000-0000-000000000001"), userId1, userId2, false, false, null, DateTime.UtcNow);
        Assert.True(match.IsOwner(userId1));
        Assert.True(match.IsOwner(userId2));
    }

    [Fact]
    public void match_is_owner_should_return_false_when_user_id_dont_match()
    {
        var userId1 = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var userId2 = Guid.Parse("00000000-0000-0000-0000-000000000002");
        var userId3 = Guid.Parse("00000000-0000-0000-0000-000000000003");
        var match = new Match(Guid.Parse("00000000-0000-0000-0000-000000000001"), userId1, userId2, false, false, null, DateTime.UtcNow);
        Assert.False(match.IsOwner(userId3));
    }

    [Fact]
    public void swipe_is_owner_should_return_true_when_user_id_match_swiped_by_user_id()
    {
        var userId1 = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var userId2 = Guid.Parse("00000000-0000-0000-0000-000000000002");
        var swipe = new Swipe(Guid.Parse("00000000-0000-0000-0000-000000000001"), userId1, userId2, Like.Like, DateTime.UtcNow);
        Assert.True(swipe.IsOwner(userId1));
    }

    [Fact]
    public void swipe_is_owner_should_return_true_when_user_id_dont_match_swiped_by_user_id()
    {
        var userId1 = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var userId2 = Guid.Parse("00000000-0000-0000-0000-000000000002");
        var swipe = new Swipe(Guid.Parse("00000000-0000-0000-0000-000000000001"), userId1, userId2, Like.Like, DateTime.UtcNow);
        Assert.False(swipe.IsOwner(userId2));
    }

    [Fact]
    public void message_is_owner_should_return_true_when_send_from_id_match()
    {
        var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var message = new Message(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), userId, "test", false, DateTime.UtcNow);
        Assert.True(message.IsOwner(userId));
    }

    [Fact]
    public void message_is_owner_should_return_false_when_send_from_id_dont_match()
    {
        var userId1 = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var userId2 = Guid.Parse("00000000-0000-0000-0000-000000000002");
        var message = new Message(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), userId1, "test", false, DateTime.UtcNow);
        Assert.False(message.IsOwner(userId2));
    }

    [Fact]
    public void photo_is_owner_should_return_true_when_user_id_match()
    {
        var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var photo = new Photo(Guid.Parse("00000000-0000-0000-0000-000000000001"), userId, "test", "test", 0);
        Assert.True(photo.IsOwner(userId));
    }

    [Fact]
    public void photo_is_owner_should_return_false_when_user_id_dont_match()
    {
        var userId1 = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var userId2 = Guid.Parse("00000000-0000-0000-0000-000000000003");
        var photo = new Photo(Guid.Parse("00000000-0000-0000-0000-000000000001"), userId1, "test", "test", 0);
        Assert.False(photo.IsOwner(userId2));
    }
}