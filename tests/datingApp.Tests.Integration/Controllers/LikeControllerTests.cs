using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Core.Consts;
using datingApp.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace datingApp.Tests.Integration.Controllers;

[Collection("Integration tests")]
public class LikeControllerTests : ControllerTestBase, IDisposable
{
    [Fact]
    public async Task given_other_user_not_swiped_put_like_returns_200_status_code_and_false_content()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);

        var token = Authorize(user1.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var response = await Client.PutAsync($"like/{user2.Id}", null);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var isLikedByOtherUser = await response.Content.ReadFromJsonAsync<IsLikedByOtherUserDto>();
        Assert.False(isLikedByOtherUser.IsLikedByOtherUser);
    }

    [Fact]
    public async Task given_other_user_not_liked_put_like_returns_200_status_code_and_false_content()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        _ = await IntegrationTestHelper.CreateSwipeAsync(_testDb, user1.Id, user2.Id, Like.Pass);

        var token = Authorize(user1.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var response = await Client.PutAsync($"like/{user2.Id}", null);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var isLikedByOtherUser = await response.Content.ReadFromJsonAsync<IsLikedByOtherUserDto>();
        Assert.False(isLikedByOtherUser.IsLikedByOtherUser);
    }

    [Fact]
    public async Task given_other_user_not_exists_put_like_returns_404_status_code()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);

        var token = Authorize(user1.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var response = await Client.PutAsync($"like/{Guid.NewGuid}", null);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task given_other_user_liked_put_like_returns_200_status_code_and_true_content()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        _ = await IntegrationTestHelper.CreateSwipeAsync(_testDb, user2.Id, user1.Id, Like.Like);

        var token = Authorize(user1.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var response = await Client.PutAsync($"like/{user2.Id}", null);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var isLikedByOtherUser = await response.Content.ReadFromJsonAsync<IsLikedByOtherUserDto>();
        Assert.True(isLikedByOtherUser.IsLikedByOtherUser);
    }

    private readonly TestDatabase _testDb;
    public LikeControllerTests(OptionsProvider optionsProvider) : base(optionsProvider)
    {
        _testDb = new TestDatabase(false);
    }

    public void Dispose()
    {
        _testDb?.Dispose();
    }
}