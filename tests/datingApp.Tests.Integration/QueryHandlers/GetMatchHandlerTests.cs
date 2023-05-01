using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Queries;
using datingApp.Core.Entities;
using datingApp.Infrastructure.DAL.Handlers;
using Xunit;

namespace datingApp.Tests.Integration.QueryHandlers;

[Collection("Integration tests")]
public class GetMatchHandlerTests : IDisposable
{
    [Fact]
    public async void given_two_users_liked_each_other_get_match_should_return_non_empty_match_dto_with_true_match_value()
    {
        var swipes = new List<Swipe>
        {
            new Core.Entities.Swipe(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000002"), Like.Like, DateTime.UtcNow),
            new Core.Entities.Swipe(Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Like.Like, DateTime.UtcNow)
        };
        await _testDb.DbContext.Swipes.AddRangeAsync(swipes);
        await _testDb.DbContext.SaveChangesAsync();

        var query = new GetMatch { SwipedById =  Guid.Parse("00000000-0000-0000-0000-000000000001"), SwipedWhoId = Guid.Parse("00000000-0000-0000-0000-000000000002") };
        var match = await _handler.HandleAsync(query);
        Assert.NotNull(match);
        Assert.IsType<IsLikedByOtherUserDto>(match);
        Assert.Equal(true, match.IsLikedByOtherUser);
    }

    [Fact]
    public async void given_only_one_user_liked_other_get_match_should_return_non_empty_match_dto_with_false_Match_value()
    {
        var swipes = new List<Swipe>
        {
            new Core.Entities.Swipe(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000002"), Like.Like, DateTime.UtcNow),
            new Core.Entities.Swipe(Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Like.Pass, DateTime.UtcNow)
        };
        await _testDb.DbContext.Swipes.AddRangeAsync(swipes);
        await _testDb.DbContext.SaveChangesAsync();
        
        var query = new GetMatch { SwipedById =  Guid.Parse("00000000-0000-0000-0000-000000000001"), SwipedWhoId = Guid.Parse("00000000-0000-0000-0000-000000000002") };
        var match = await _handler.HandleAsync(query);
        Assert.NotNull(match);
        Assert.IsType<IsLikedByOtherUserDto>(match);
        Assert.Equal(false, match.IsLikedByOtherUser);
    }

    [Fact]
    public async void given_no_swipes_exists_get_match_should_return_non_empty_match_dto_with_false_Match_value()
    {
        var query = new GetMatch { SwipedById =  Guid.Parse("00000000-0000-0000-0000-000000000001"), SwipedWhoId = Guid.Parse("00000000-0000-0000-0000-000000000002") };
        var match = await _handler.HandleAsync(query);
        Assert.NotNull(match);
        Assert.IsType<IsLikedByOtherUserDto>(match);
        Assert.Equal(false, match.IsLikedByOtherUser);
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly GetMatchHandler _handler;
    public GetMatchHandlerTests()
    {
        var settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000001"), Sex.Female, 18, 21, 20, 45.5, 45.5);
        var user = new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "111111111", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);
        var settings2 = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000002"), Sex.Male, 18, 21, 20, 45.5, 45.5);
        var user2 = new User(Guid.Parse("00000000-0000-0000-0000-000000000002"), "222222222", "test2@test.com", "Karyna", new DateOnly(2000,1,1), Sex.Female, null, settings);
        
        _testDb = new TestDatabase();
        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.Users.Add(user2);
        _testDb.DbContext.SaveChanges();
        _handler = new GetMatchHandler(_testDb.DbContext);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}