using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Core.Consts;
using datingApp.Core.Entities;
using datingApp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace datingApp.Tests.Integration.Controllers;

[Collection("Controller tests")]
public class LikeControllerTests : ControllerTestBase, IDisposable
{
    [Fact]
    public async Task given_other_user_not_swiped_put_like_returns_200_status_code_and_false_content()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user1.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var response = await Client.PutAsync($"like/{user2.Id.Value}", null);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var isLikedByOtherUser = await response.Content.ReadFromJsonAsync<IsLikedByOtherUserDto>();
        Assert.False(isLikedByOtherUser.IsLikedByOtherUser);
    }

    [Fact]
    public async Task given_other_user_not_liked_put_like_returns_200_status_code_and_false_content()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _ = await IntegrationTestHelper.CreateSwipeAsync(_dbContext, user1.Id, user2.Id, Like.Pass);
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user1.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var response = await Client.PutAsync($"like/{user2.Id.Value}", null);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var isLikedByOtherUser = await response.Content.ReadFromJsonAsync<IsLikedByOtherUserDto>();
        Assert.False(isLikedByOtherUser.IsLikedByOtherUser);
    }

    [Fact]
    public async Task given_other_user_not_exists_put_like_returns_404_status_code()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user1.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var response = await Client.PutAsync($"like/{Guid.NewGuid}", null);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task given_other_user_liked_put_like_returns_200_status_code_and_true_content()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _ = await IntegrationTestHelper.CreateSwipeAsync(_dbContext, user2.Id, user1.Id, Like.Like);
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user1.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var response = await Client.PutAsync($"like/{user2.Id.Value}", null);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var isLikedByOtherUser = await response.Content.ReadFromJsonAsync<IsLikedByOtherUserDto>();
        Assert.True(isLikedByOtherUser.IsLikedByOtherUser);
    }

    private readonly TestDatabase _testDb;
    private readonly DatingAppDbContext _dbContext;

    public LikeControllerTests(OptionsProvider optionsProvider) : base(optionsProvider)
    {
        _testDb = new TestDatabase(false);
        _dbContext = _testDb.DbContext;
    }

    public void Dispose()
    {
        _testDb?.Dispose();
    }
}