using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using datingApp.Application.Commands;
using datingApp.Application.DTO;
using datingApp.Application.Queries;
using datingApp.Core.Entities;
using Newtonsoft.Json;
using Xunit;

namespace datingApp.Tests.Integration.Controllers;

[Collection("Controller tests")]
public class UsersControllerTests : ControllerTestBase, IDisposable
{
    [Fact]
    public async Task given_valid_sign_up_post_request_returns_201_created_and_private_user_dto()
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
    public async Task given_email_already_exists_sign_up_post_request_returns_400_bad_request()
    {
        var email = "test@test.com";
        await IntegrationTestHelper.CreateUserAsync(_testDb, email);

        var command = new SignUp(Guid.Empty, "123456789", email, "Janusz", "2000-01-01", 1, 1);
        var response = await Client.PostAsJsonAsync("users", command);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task given_phone_already_exists_sign_up_post_request_returns_400_bad_request()
    {
        var phone = "123456789";
        await IntegrationTestHelper.CreateUserAsync(_testDb, phone : phone);

        var command = new SignUp(Guid.Empty, phone, "test@test.com", "Janusz", "2000-01-01", 1, 1);
        var response = await Client.PostAsJsonAsync("users", command);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task given_email_exists_auth_post_request_returns_200_ok()
    {
        var email = "test@test.com";
        await IntegrationTestHelper.CreateUserAsync(_testDb, email);

        var command = new RequestEmailAccessCode(email);
        var response = await Client.PostAsJsonAsync("users/auth", command);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task given_email_not_exists_auth_post_request_returns_200_ok()
    {
        var email = "test@test.com";

        var command = new RequestEmailAccessCode(email);
        var response = await Client.PostAsJsonAsync("users/auth", command);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task given_valid_access_code_sign_in_post_request_returns_200_ok_and_token()
    {
        var email = "test@test.com";
        await IntegrationTestHelper.CreateUserAsync(_testDb, email);
        var accessCode = "12345";
        await CreateAccessCode(email, accessCode);

        var command = new SignInByEmail(email, accessCode);
        var response = await Client.PostAsJsonAsync("users/sign-in", command);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var token = await response.Content.ReadFromJsonAsync<JwtDto>();
        Assert.NotNull(token);
        Assert.False(string.IsNullOrWhiteSpace(token.AccessToken.Token));
    }

    [Fact]
    public async Task given_invalid_access_code_sign_in_post_request_returns_400_bad_request()
    {
        var email = "test@test.com";
        await IntegrationTestHelper.CreateUserAsync(_testDb, email);
        var accessCode = "12345";
        await CreateAccessCode(email, accessCode);

        var invalidAccessCode = "67890";
        var command = new SignInByEmail(email, invalidAccessCode);
        var response = await Client.PostAsJsonAsync("users/sign-in", command);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task given_expired_access_code_sign_in_post_request_returns_400_bad_request()
    {
        var email = "test@test.com";
        await IntegrationTestHelper.CreateUserAsync(_testDb, email);
        var expiredAccessCode = "12345";
        await CreateAccessCode(email, expiredAccessCode, TimeSpan.FromMilliseconds(1));

        var command = new SignInByEmail(email, expiredAccessCode);
        var response = await Client.PostAsJsonAsync("users/sign-in", command);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task given_missing_token_get_users_returns_401_unauthorized()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);

        var response = await Client.GetAsync($"users/{user.Id}");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task given_invalid_token_get_users_returns_401_unauthorized()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var token = Authorize(user.Id);
        var badToken = token.AccessToken.Token + "x";

        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {badToken}");
        var response = await Client.GetAsync($"users/{user.Id}");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task given_valid_refresh_token_used_to_authorize_get_me_returns_401_unauthorized()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var tokens = Authorize(user.Id);
        var refreshToken = tokens.RefreshToken.Token;

        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {refreshToken}");
        var response = await Client.GetAsync($"users/me");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task given_invalid_refresh_token_auth_refresh_returns_401_unauthorized()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var badToken = Authorize(user.Id).AccessToken.Token;

        var command = new RefreshJWT(badToken);
        var response = await Client.PostAsJsonAsync($"users/auth/refresh", command);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task given_valid_refresh_token_auth_refresh_returns_200_with_new_access_and_refresh_tokens()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var tokens = Authorize(user.Id);
        var accessToken = tokens.AccessToken.Token;
        var refreshToken = tokens.RefreshToken.Token;

        // workaround: sleep 1000 milliseconds so that newly generated token
        // is not the same as the old token
        Thread.Sleep(1000);

        var command = new RefreshJWT(refreshToken);
        var response = await Client.PostAsJsonAsync($"users/auth/refresh", command);
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    
        var responseJson = await response.Content.ReadFromJsonAsync<JwtDto>();
        Assert.NotNull(responseJson.AccessToken);
        Assert.NotNull(responseJson.RefreshToken);
        Assert.NotEqual(responseJson.AccessToken.Token, accessToken);
        Assert.NotEqual(responseJson.RefreshToken.Token, refreshToken);
    }

    [Fact]
    public async Task given_valid_refresh_token_used_more_than_once_auth_refresh_returns_401_unauthorized()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var tokens = Authorize(user.Id);
        var refreshToken = tokens.RefreshToken.Token;

        var command = new RefreshJWT(refreshToken);
        var response = await Client.PostAsJsonAsync($"users/auth/refresh", command);
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var secondResponse = await Client.PostAsJsonAsync($"users/auth/refresh", command);
        Assert.NotNull(secondResponse);
        Assert.Equal(HttpStatusCode.Unauthorized, secondResponse.StatusCode);
    }

    [Fact]
    public async Task given_expired_refresh_token_auth_refresh_returns_401_unauthorized()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var tokenExpirtaionTimeInMilliseconds = 1000;
        var tokens = Authorize(user.Id, refreshTokenExpirtyTime: TimeSpan.FromMilliseconds(tokenExpirtaionTimeInMilliseconds));
        var refreshToken = tokens.RefreshToken.Token;

        // more time due to jwt's time precision
        var sleepTimeInMilliseconds = tokenExpirtaionTimeInMilliseconds + 1000;
        Thread.Sleep(sleepTimeInMilliseconds);

        var command = new RefreshJWT(refreshToken);
        var response = await Client.PostAsJsonAsync($"users/auth/refresh", command);
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task given_two_users_have_match_get_users_returns_200_ok_and_public_user()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        _ = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);

        var token = Authorize(user1.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var response = await Client.GetFromJsonAsync<PublicUserDto>($"users/{user2.Id}");
        Assert.NotNull(response);
        Assert.Equal(user2.Id, response.Id);
    }

    [Fact]
    public async Task given_two_users_dont_have_match_get_users_returns_403_forbidden()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);

        var token = Authorize(user1.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var response =  await Client.GetAsync($"users/{user2.Id}");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task given_user_get_himself_get_users_returns_403_forbidden()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);

        var token = Authorize(user1.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var response = await Client.GetAsync($"users/{user1.Id}");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task given_requested_user_not_exists_get_users_returns_no_content()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var notExistingUserId = Guid.NewGuid();
        var response = await Client.GetAsync($"users/{notExistingUserId}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task get_users_me_returns_200_ok_and_private_user()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var response = await Client.GetFromJsonAsync<PrivateUserDto>($"users/me");
        Assert.NotNull(response);
        Assert.Equal(user.Id, response.Id);
    }

    [Fact]
    public async Task given_user_exists_delete_users_returns_204_no_content()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var response = await Client.DeleteAsync($"users/{user.Id}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task given_user_not_exists_delete_users_returns_404_not_found_and_proper_error_reason()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");
        
        var notExistingUserId = Guid.NewGuid();
        var response = await Client.DeleteAsync($"users/{notExistingUserId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<Error>();
        Assert.Equal($"User with id {notExistingUserId} does not exist.", error.Reason);
    }

    [Fact]
    public async Task given_user_was_already_deleted_delete_users_returns_410_gone()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        await IntegrationTestHelper.DeleteUserAsync(_testDb, user);

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");
        
        var response = await Client.DeleteAsync($"users/{user.Id}");
        Assert.Equal(HttpStatusCode.Gone, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<Error>();
        Assert.Equal($"User {user.Id} is deleted permanently.", error.Reason);
    }

    [Fact]
    public async Task get_recommendations_returns_200_and_list_of_public_user_dto()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");
        var response = await Client.GetFromJsonAsync<List<PublicUserDto>>($"users/me/recommendations");
        Assert.NotNull(response);
    }

    [Fact]
    public async Task get_recommendations_returns_max_10_private_user_dtos()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        for (int i=0; i<20; i++)
        {
            await IntegrationTestHelper.CreateUserAsync(_testDb, $"test{i}@test.com");
        }

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");
        var response = await Client.GetFromJsonAsync<List<PublicUserDto>>($"users/me/recommendations");
        Assert.NotNull(response);
        Assert.Equal(10, response.Count);
    }

    [Fact]
    public async Task get_updates_returns_200_and_list_of_matches_dto()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");
        var response = await Client.GetFromJsonAsync<List<MatchDto>>("users/me/updates");
        Assert.NotNull(response);
    }

    [Fact]
    public async Task get_updates_without_lastActivityTime_specified_returns_list_of_all_not_displayed_messages_and_matches_as_matches_dto()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var users = new List<User>();
        for (int i=0; i<100; i++)
        {
            var tempUser = await IntegrationTestHelper.CreateUserAsync(_testDb);
            users.Add(tempUser);
        }

        var matches = new List<Match>();
        var random = new Random();
        for (int i=0; i<50; i++)
        {
            var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user.Id, users[i].Id, createdAt : DateTime.UtcNow - TimeSpan.FromMinutes(random.Next(1, 1000001)));
            matches.Add(match);
        }
    
        for (int i=50; i<100; i++)
        {
            var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user.Id, users[i].Id, createdAt : DateTime.UtcNow - TimeSpan.FromMinutes(random.Next(1, 1000001)));
            matches.Add(match);
        }

        for (int i=0; i<50; i++)
        {
            _ = await IntegrationTestHelper.CreateMessageAsync(_testDb, matches[i].Id, matches[i].UserId2, createdAt: DateTime.UtcNow - TimeSpan.FromMinutes(random.Next(1, 1000001)));
        }

        for (int i=50; i<75; i++)
        {
            _ = await IntegrationTestHelper.CreateMessageAsync(_testDb, matches[i].Id, matches[i].UserId2, createdAt: DateTime.UtcNow - TimeSpan.FromMinutes(random.Next(1, 1000001)));
        }

        for (int i=75; i<100; i++)
        {
            _ = await IntegrationTestHelper.CreateMessageAsync(_testDb, matches[i].Id, matches[i].UserId2, createdAt: DateTime.UtcNow - TimeSpan.FromMinutes(random.Next(1, 1000001)));
        }

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");
        var response = await Client.GetFromJsonAsync<List<MatchDto>>("users/me/updates");
        Assert.NotNull(response);
        Assert.Equal(100, response.Count);
    }

    [Fact]
    public async Task get_updates_returns_list_of_matches_dto_since_last_activity_time_parameter()
    {
        var time = DateTime.UtcNow;
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user3 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user4 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user5 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user6 = await IntegrationTestHelper.CreateUserAsync(_testDb);

        _ = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id, createdAt: time - TimeSpan.FromSeconds(1));
        _ = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user3.Id, createdAt: time - TimeSpan.FromSeconds(1));
        _ = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user4.Id, createdAt: time - TimeSpan.FromSeconds(1));
        var match1 = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user5.Id, createdAt: time - TimeSpan.FromHours(2));
        var match2 = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user6.Id, createdAt: time - TimeSpan.FromHours(2));

        _ = await IntegrationTestHelper.CreateMessageAsync(_testDb, match1.Id, user5.Id, createdAt: time - TimeSpan.FromSeconds(1));
        _ = await IntegrationTestHelper.CreateMessageAsync(_testDb, match2.Id, user6.Id, createdAt: time - TimeSpan.FromSeconds(1));

        var token = Authorize(user1.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var lastActivityTime = time - TimeSpan.FromHours(1);
        var response = await Client.GetFromJsonAsync<List<MatchDto>>($"users/me/updates?lastActivityTime={lastActivityTime}");
        Assert.NotNull(response);
        Assert.Equal(5, response.Count);
    }

    [Fact]
    public async Task patch_users_with_no_changes_returns_204_no_content()
    {
        var email = "test@test.com";
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb, email);

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var command = new ChangeUser(user.Id);
        var payload = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");
        var response = await Client.PatchAsync($"users/{user.Id}", payload);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task patch_users_with_changes_returns_204_no_content()
    {
        var email = "test@test.com";
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb, email);

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var command = new ChangeUser(user.Id, "2001-01-01");
        var content = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");
        var response = await Client.PatchAsync($"users/{user.Id}", content);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task given_user_not_exists_patch_users_returns_404_not_found()
    {
        var email = "test@test.com";
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb, email);

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var command = new ChangeUser(user.Id, "2001-01-01");
        var content = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");
        var response = await Client.PatchAsync($"users/{Guid.NewGuid()}", content);
        Debug.Print($"users/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<AccessCodeDto> CreateAccessCode(string email, string accessCode = "123456", TimeSpan? expirationTime = null)
    {
        var code = new AccessCodeDto()
            {
                AccessCode = accessCode,
                EmailOrPhone = email,
                ExpirationTime = DateTime.UtcNow + (expirationTime ?? TimeSpan.FromMinutes(15)),
                Expiry = expirationTime ?? TimeSpan.FromMinutes(15)
            };
        await _testDb.DbContext.AccessCodes.AddAsync(code);
        await _testDb.DbContext.SaveChangesAsync();
        return code;
    }

    private readonly TestDatabase _testDb;
    public UsersControllerTests(OptionsProvider optionsProvider) : base(optionsProvider)
    {
        _testDb = new TestDatabase(false);
    }

    public void Dispose()
    {
        _testDb?.Dispose();
    }
}