using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using datingApp.Application.Commands;
using datingApp.Application.DTO;
using datingApp.Core.Entities;
using Newtonsoft.Json;
using Xunit;

namespace datingApp.Tests.Integration.Controllers;

[Collection("Integration tests")]
public class UsersControllerTests : ControllerTestBase, IDisposable
{
    [Fact]
    public async Task given_valid_sign_up_post_request_should_return_201_created_and_private_user_dto()
    {
        var email = "test@test.com";
        var command = new SignUp(Guid.Empty, "123456789", email, "Janusz", "2000-01-01", 1, 1);
        var response = await Client.PostAsJsonAsync("users", command);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers);
        var dto = await response.Content.ReadFromJsonAsync<PrivateUserDto>();
        Assert.NotNull(dto);
        Assert.Equal(dto.Email, email);
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
                ExpirationTime = DateTime.UtcNow + TimeSpan.FromMinutes(15),
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
    public async Task given_expired_access_code_sign_in_post_request_should_return_400_bad_request()
    {
        var email = "test@test.com";
        await CreateUserAsync(email);
        var accessCode = "12345";
        var code = new AccessCodeDto()
            {
                AccessCode = accessCode,
                EmailOrPhone = email,
                ExpirationTime = DateTime.UtcNow,
                Expiry = TimeSpan.FromMilliseconds(1)
            };
        await _testDb.DbContext.AccessCodes.AddAsync(code);
        await _testDb.DbContext.SaveChangesAsync();
        var invalidAccessCode = "12345";
        var command = new SignInByEmail(email, invalidAccessCode);
        var response = await Client.PostAsJsonAsync("users/sign-in", command);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task given_missing_token_get_users_should_return_401_unauthorized()
    {
        var email = "test@test.com";
        var user = await CreateUserAsync(email);
        var response = await Client.GetAsync($"users/{user.Id}");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task given_invalid_token_get_users_should_return_401_unauthorized()
    {
        var email = "test@test.com";
        var user = await CreateUserAsync(email);
        var token = Authorize(user.Id);
        var badToken = token.AccessToken + "x";
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {badToken}");
        var response = await Client.GetAsync($"users/{user.Id}");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task get_users_should_return_200_ok_and_public_user()
    {
        var email = "test@test.com";
        var user = await CreateUserAsync(email);
        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");
        var response = await Client.GetFromJsonAsync<PublicUserDto>($"users/{user.Id}");
        Assert.NotNull(response);
        Assert.Equal(user.Id, response.Id);
    }

    [Fact]
    public async Task get_users_me_should_return_200_ok_and_private_user()
    {
        var email = "test@test.com";
        var user = await CreateUserAsync(email);
        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");
        var response = await Client.GetFromJsonAsync<PrivateUserDto>($"users/me");
        Assert.NotNull(response);
        Assert.Equal(user.Id, response.Id);
    }

    [Fact]
    public async Task delete_users_should_return_201_no_content()
    {
        var email = "test@test.com";
        var user = await CreateUserAsync(email);
        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");
        var response = await Client.DeleteAsync($"users/{user.Id}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task patch_users_with_no_changes_should_return_201_no_content()
    {
        var email = "test@test.com";
        var user = await CreateUserAsync(email);
        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");
        var command = new ChangeUser(user.Id);
        var content = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");
        var response = await Client.PatchAsync($"users/{user.Id}", content);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task patch_users_with_changes_should_return_201_no_content()
    {
        var email = "test@test.com";
        var user = await CreateUserAsync(email);
        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");
        var command = new ChangeUser(user.Id, "2001-01-01");
        var content = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");
        var response = await Client.PatchAsync($"users/{user.Id}", content);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
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