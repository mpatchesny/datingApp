using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Core.Consts;
using datingApp.Core.Entities;
using Xunit;

namespace datingApp.Tests.Integration.Controllers;

[Collection("Integration tests")]
public class PassControllerTests : ControllerTestBase, IDisposable
{
    [Fact]
    public async Task given_other_user_not_liked_put_pass_returns_200_status_code_and_false_content()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);;

        var token = Authorize(user1.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var response = await Client.PutAsync($"pass/{user2.Id}", null);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var isLikedByOtherUser = await response.Content.ReadFromJsonAsync<IsLikedByOtherUserDto>();
        Assert.False(isLikedByOtherUser.IsLikedByOtherUser);
    }

    [Fact]
    public async Task given_other_user_not_exists_put_pass_returns_404_status_code()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);

        var token = Authorize(user1.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var response = await Client.PutAsync($"pass/{Guid.NewGuid}", null);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task given_other_user_liked_put_pass_returns_200_status_code_and_true_content()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);;

        var swipe = new Swipe(user2.Id, user1.Id, Like.Like, DateTime.UtcNow);
        _testDb.DbContext.Swipes.Add(swipe);
        await _testDb.DbContext.SaveChangesAsync();

        var token = Authorize(user1.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var response = await Client.PutAsync($"pass/{user2.Id}", null);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var isLikedByOtherUser = await response.Content.ReadFromJsonAsync<IsLikedByOtherUserDto>();
        Assert.True(isLikedByOtherUser.IsLikedByOtherUser);
    }

    private readonly TestDatabase _testDb;
    public PassControllerTests(OptionsProvider optionsProvider) : base(optionsProvider)
    {
        _testDb = new TestDatabase(false);
    }

    public void Dispose()
    {
        _testDb?.Dispose();
    }
}