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


public class GetIsLikedByOtherUserHandlerTests : IDisposable
{
    [Fact]
    public async void given_other_user_like_user_GetIsLikedByOtherUser_should_return_non_empty_is_liked_by_other_user_dto_with_true_is_liked_value()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        _ = await IntegrationTestHelper.CreateSwipeAsync(_testDb, user1.Id, user2.Id, Like.Like);

        var query = new GetIsLikedByOtherUser { SwipedById = user1.Id, SwipedWhoId = user2.Id };
        var match = await _handler.HandleAsync(query);
        Assert.NotNull(match);
        Assert.IsType<IsLikedByOtherUserDto>(match);
        Assert.True(match.IsLikedByOtherUser);
    }

    [Fact]
    public async void given_other_user_not_like_user_GetIsLikedByOtherUser_should_return_non_empty_is_liked_by_other_user_dto_with_false_is_liked_value()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        _ = await IntegrationTestHelper.CreateSwipeAsync(_testDb, user1.Id, user2.Id, Like.Pass);

        var query = new GetIsLikedByOtherUser { SwipedById = user1.Id, SwipedWhoId = user2.Id };
        var match = await _handler.HandleAsync(query);
        Assert.NotNull(match);
        Assert.IsType<IsLikedByOtherUserDto>(match);
        Assert.True(match.IsLikedByOtherUser);
    }

    [Fact]
    public async void given_no_swipes_exists_GetIsLikedByOtherUser_should_return_non_empty_is_liked_by_other_user_dto_with_false_is_liked_value()
    {
        var query = new GetIsLikedByOtherUser { SwipedById = Guid.NewGuid(), SwipedWhoId = Guid.NewGuid() };
        var match = await _handler.HandleAsync(query);
        Assert.NotNull(match);
        Assert.IsType<IsLikedByOtherUserDto>(match);
        Assert.False(match.IsLikedByOtherUser);
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly GetIsLikedByOtherUserHandler _handler;
    public GetIsLikedByOtherUserHandlerTests()
    {
        _testDb = new TestDatabase();
        // var settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000001"), Sex.Female, 18, 21, 20, 45.5, 45.5);
        // var user = new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "111111111", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);
        // var settings2 = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000002"), Sex.Male, 18, 21, 20, 45.5, 45.5);
        // var user2 = new User(Guid.Parse("00000000-0000-0000-0000-000000000002"), "222222222", "test2@test.com", "Karyna", new DateOnly(2000,1,1), Sex.Female, null, settings2);
        // _testDb.DbContext.Users.Add(user);
        // _testDb.DbContext.Users.Add(user2);
        // _testDb.DbContext.SaveChanges();
        _handler = new GetIsLikedByOtherUserHandler(_testDb.DbContext);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}