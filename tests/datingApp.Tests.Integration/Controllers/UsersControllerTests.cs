using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using datingApp.Application.Commands;
using datingApp.Core.Entities;
using Xunit;

namespace datingApp.Tests.Integration.Controllers;

[Collection("Integration tests")]
public class UsersControllerTests : ControllerTestBase, IDisposable
{
    [Fact]
    public async Task given_valid_sign_up_post_request_should_return_201_created()
    {
        var command = new SignUp(Guid.Empty, "123456789", "test@test.com", "Janusz", "2000-01-01", 1, 1);
        var response = await Client.PostAsJsonAsync("users", command);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers);
    }

    [Fact]
    public async Task given_email_already_exists_sign_up_post_request_should_return_400_bad_request()
    {
        var settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000002"), Sex.Female, 18, 20, 50, 45.5, 45.5);
        var user = new User(Guid.NewGuid(), "123456789", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);
        _testDb.DbContext.Users.Add(user);
        await _testDb.DbContext.SaveChangesAsync();
        var command = new SignUp(Guid.Empty, "123456789", "test@test.com", "Janusz", "2000-01-01", 1, 1);
        var response = await Client.PostAsJsonAsync("users", command);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task get_public_user_when_not_logged_in_should_return_401_unauthorized()
    {
        var guid = new Guid("00000000-0000-0000-0000-000000000001");
        var response = await Client.GetAsync($"users/{guid}");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private readonly TestDatabase _testDb;

    public UsersControllerTests(OptionsProvider optionsProvider) : base(optionsProvider)
    {
        _testDb = new TestDatabase();
    }

    public void Dispose()
    {
        _testDb?.Dispose();
    }
}