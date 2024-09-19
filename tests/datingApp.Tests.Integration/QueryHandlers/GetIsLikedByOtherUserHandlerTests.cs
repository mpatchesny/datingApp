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
        _handler = new GetIsLikedByOtherUserHandler(_testDb.DbContext);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}