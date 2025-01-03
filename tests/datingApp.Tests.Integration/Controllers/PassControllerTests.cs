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
using Xunit;

namespace datingApp.Tests.Integration.Controllers;

[Collection("Controller tests")]
public class PassControllerTests : ControllerTestBase, IDisposable
{
    [Fact]
    public async Task given_other_user_not_liked_put_pass_returns_200_status_code_and_false_content()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _ = await IntegrationTestHelper.CreateSwipeAsync(_dbContext, user2.Id, user1.Id, Like.Pass);
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user1.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var response = await Client.PutAsync($"pass/{user2.Id.Value}", null);
        var isLikedByOtherUser = await response.Content.ReadFromJsonAsync<IsLikedByOtherUserDto>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.False(isLikedByOtherUser.IsLikedByOtherUser);
    }

    [Fact]
    public async Task given_other_user_not_swiped_put_pass_returns_200_status_code_and_false_content()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user1.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var response = await Client.PutAsync($"pass/{user2.Id.Value}", null);
        var isLikedByOtherUser = await response.Content.ReadFromJsonAsync<IsLikedByOtherUserDto>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.False(isLikedByOtherUser.IsLikedByOtherUser);
    }

    [Fact]
    public async Task given_other_user_not_exists_put_pass_returns_200_status_code_and_false_content()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user1.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var response = await Client.PutAsync($"pass/{Guid.NewGuid()}", null);
        var isLikedByOtherUser = await response.Content.ReadFromJsonAsync<IsLikedByOtherUserDto>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.False(isLikedByOtherUser.IsLikedByOtherUser);
    }

    [Fact]
    public async Task given_other_user_liked_put_pass_returns_200_status_code_and_true_content()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);;
        _ = await IntegrationTestHelper.CreateSwipeAsync(_dbContext, user2.Id, user1.Id, Like.Like);
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user1.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var response = await Client.PutAsync($"pass/{user2.Id.Value}", null);
        var isLikedByOtherUser = await response.Content.ReadFromJsonAsync<IsLikedByOtherUserDto>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(isLikedByOtherUser.IsLikedByOtherUser);
    }

    private readonly TestDatabase _testDb;
    private readonly DatingAppDbContext _dbContext;

    public PassControllerTests(OptionsProvider optionsProvider) : base(optionsProvider)
    {
        _testDb = new TestDatabase(false);
        _dbContext = _testDb.DbContext;
    }

    public void Dispose()
    {
        _testDb.Dispose();
    }
}