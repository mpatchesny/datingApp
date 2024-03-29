using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using datingApp.Application.Commands;
using datingApp.Application.DTO;
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
        var email = "test@test.com";
        await CreateUserAsync(email);
        var command = new SignUp(Guid.Empty, "123456789", email, "Janusz", "2000-01-01", 1, 1);
        var response = await Client.PostAsJsonAsync("users", command);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task given_email_exists_auth_post_request_should_return_200_ok()
    {
        var email = "test@test.com";
        await CreateUserAsync(email);
        var command = new RequestEmailAccessCode(email);
        var response = await Client.PostAsJsonAsync("users/auth", command);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task given_email_not_exists_auth_post_request_should_return_200_ok()
    {
        var email = "test@test.com";
        var command = new RequestEmailAccessCode(email);
        var response = await Client.PostAsJsonAsync("users/auth", command);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task given_valid_access_code_sign_in_post_request_should_return_200_ok_and_JWT_token()
    {
        var email = "test@test.com";
        await CreateUserAsync(email);
        var accessCode = "12345";
        var code = new AccessCodeDto()
            {
                AccessCode = accessCode,
                EmailOrPhone = email,
                Expiry = TimeSpan.FromMinutes(15),
                ExpirationTime = DateTime.UtcNow + TimeSpan.FromMinutes(15)
            };
        await _testDb.DbContext.AccessCodes.AddAsync(code);
        await _testDb.DbContext.SaveChangesAsync();
        var command = new SignInByEmail(email, accessCode);
        var response = await Client.PostAsJsonAsync("users/sign-in", command);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var token = await response.Content.ReadFromJsonAsync<JwtDto>();
        Assert.NotNull(token);
        Assert.False(string.IsNullOrWhiteSpace(token.AccessToken));
    }

    [Fact]
    public async Task given_invalid_access_code_sign_in_post_request_should_return_400_bad_request()
    {
        var email = "test@test.com";
        await CreateUserAsync(email);
        var accessCode = "12345";
        var code = new AccessCodeDto()
            {
                AccessCode = accessCode,
                EmailOrPhone = email,
                ExpirationTime = DateTime.UtcNow,
                Expiry = TimeSpan.FromMinutes(15)
            };
        await _testDb.DbContext.AccessCodes.AddAsync(code);
        await _testDb.DbContext.SaveChangesAsync();
        var invalidAccessCode = "67890";
        var command = new SignInByEmail(email, invalidAccessCode);
        var response = await Client.PostAsJsonAsync("users/sign-in", command);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task given_missing_JWT_token_get_public_user_should_return_401_unauthorized()
    {
        var email = "test@test.com";
        var user = await CreateUserAsync(email);
        var response = await Client.GetAsync($"users/{user.Id}");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task given_invalid_JWT_token_get_public_user_should_return_401_unauthorized()
    {
        // TODO
        var email = "test@test.com";
        var user = await CreateUserAsync(email);
        var response = await Client.GetAsync($"users/{user.Id}");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task given_valid_JWT_token_get_public_user_should_return_200_ok_and_public_user()
    {
        // TODO
        var email = "test@test.com";
        var user = await CreateUserAsync(email);
        var response = await Client.GetAsync($"users/{user.Id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task given_valid_JWT_token_get_users_me_should_return_200_ok_and_private_user()
    {
        // TODO
        var email = "test@test.com";
        var user = await CreateUserAsync(email);
        var response = await Client.GetAsync($"users/{user.Id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private async Task<User> CreateUserAsync(string email)
    {
        var userId = Guid.NewGuid();
        var settings = new UserSettings(userId, Sex.Female, 18, 20, 50, 45.5, 45.5);
        var user = new User(userId, "123456789", email, "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);
        await _testDb.DbContext.Users.AddAsync(user);
        await _testDb.DbContext.SaveChangesAsync();
        return user;
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