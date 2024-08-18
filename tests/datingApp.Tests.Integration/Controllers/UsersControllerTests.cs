using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using datingApp.Application.Commands;
using datingApp.Application.DTO;
using datingApp.Application.Queries;
using datingApp.Core.Entities;
using Newtonsoft.Json;
using Xunit;

namespace datingApp.Tests.Integration.Controllers;

[Collection("Integration tests")]
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
        await CreateUserAsync(email);

        var command = new SignUp(Guid.Empty, "123456789", email, "Janusz", "2000-01-01", 1, 1);
        var response = await Client.PostAsJsonAsync("users", command);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task given_phone_already_exists_sign_up_post_request_returns_400_bad_request()
    {
        var email = "test@test.com";
        var phone = "123456789";
        await CreateUserAsync(email, phone);
        var email2 = "test1@test.com";

        var command = new SignUp(Guid.Empty, phone, email2, "Janusz", "2000-01-01", 1, 1);
        var response = await Client.PostAsJsonAsync("users", command);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task given_email_exists_auth_post_request_returns_200_ok()
    {
        var email = "test@test.com";
        await CreateUserAsync(email);

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
    public async Task given_invalid_access_code_sign_in_post_request_returns_400_bad_request()
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
    public async Task given_expired_access_code_sign_in_post_request_returns_400_bad_request()
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
    public async Task given_missing_token_get_users_returns_401_unauthorized()
    {
        var email = "test@test.com";
        var user = await CreateUserAsync(email);
        var response = await Client.GetAsync($"users/{user.Id}");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task given_invalid_token_get_users_returns_401_unauthorized()
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
    public async Task get_users_returns_200_ok_and_public_user()
    {
        var email = "test@test.com";
        var user = await CreateUserAsync(email);
        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");

        var response = await Client.GetFromJsonAsync<PublicUserDto>($"users/{user.Id}");
        Assert.NotNull(response);
        Assert.Equal(user.Id, response.Id);
    }

    [Fact (Skip = "FIXME")]
    public async Task given_user_with_given_id_not_exists_get_users_returns_404_not_found_and_proper_error_reason()
    {
        var email = "test@test.com";
        var user = await CreateUserAsync(email);
        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");

        var notExistingUserId = Guid.NewGuid();
        var response = await Client.GetAsync($"users/{notExistingUserId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<Error>();
        Assert.Equal($"User with id {notExistingUserId} does not exist.", error.Reason);
    }

    [Fact]
    public async Task get_users_me_returns_200_ok_and_private_user()
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
    public async Task given_user_exists_delete_users_returns_204_no_content()
    {
        var email = "test@test.com";
        var user = await CreateUserAsync(email);
        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");

        var response = await Client.DeleteAsync($"users/{user.Id}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task given_user_not_exists_delete_users_returns_404_not_found_and_proper_error_reason()
    {
        var email = "test@test.com";
        var user = await CreateUserAsync(email);
        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");
        
        var notExistingUserId = Guid.NewGuid();
        var response = await Client.DeleteAsync($"users/{notExistingUserId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<Error>();
        Assert.Equal($"User with id {notExistingUserId} does not exist.", error.Reason);
    }

    [Fact]
    public async Task given_user_was_alread_deleted_delete_users_returns_410_gone()
    {
        var email = "test@test.com";
        var user = await CreateUserAsync(email);
        await DeleteUserAsync(user);

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");
        
        var response = await Client.DeleteAsync($"users/{user.Id}");
        Assert.Equal(HttpStatusCode.Gone, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<Error>();
        Assert.Equal($"User {user.Id} is deleted permanently.", error.Reason);
    }

    [Fact]
    public async Task get_recommendations_returns_200_and_list_of_public_user_dto()
    {
        var email = "test@test.com";
        var user = await CreateUserAsync(email);
        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");
        var response = await Client.GetFromJsonAsync<List<PublicUserDto>>($"users/me/recommendations");
        Assert.NotNull(response);
    }

    [Fact]
    public async Task get_recommendations_returns_max_10_private_user_dtos()
    {
        var email = "test@test.com";
        var user = await CreateUserAsync(email);
        for (int i=0; i<20; i++)
        {
            await CreateUserAsync($"test{i}@test.com");
        }
        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");
        var response = await Client.GetFromJsonAsync<List<PublicUserDto>>($"users/me/recommendations");
        Assert.NotNull(response);
        Assert.Equal(10, response.Count);
    }

    [Fact]
    public async Task get_updates_returns_200_and_list_of_matches_dto()
    {
        var email = "test@test.com";
        var user = await CreateUserAsync(email);
        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");
        var response = await Client.GetFromJsonAsync<List<MatchDto>>("users/me/updates");
        Assert.NotNull(response);
    }

    [Fact]
    public async Task get_updates_without_lastActivityTime_specified_returns_list_of_all_not_displayed_messages_and_matches_as_matches_dto()
    {
        var user = await CreateUserAsync("test@test.com");
        var users = new List<User>();
        for (int i=0; i<100; i++)
        {
            var tempUser = await CreateUserAsync($"test{i}@test.com");
            users.Add(tempUser);
        }

        var matches = new List<Match>();
        var random = new Random();
        for (int i=0; i<50; i++)
        {
            var match = new Match(Guid.Empty, user.Id, users[i].Id, false, false, null, DateTime.UtcNow - TimeSpan.FromMinutes(random.Next(1, 1000001)));
            matches.Add(match);
            _testDb.DbContext.Matches.Add(match);
        }
        await _testDb.DbContext.SaveChangesAsync();
    
        for (int i=50; i<100; i++)
        {
            var match = new Match(Guid.Empty, user.Id, users[i].Id, true, false, null, DateTime.UtcNow - TimeSpan.FromMinutes(random.Next(1, 1000001)));
            matches.Add(match);
            _testDb.DbContext.Matches.Add(match);
        }
        await _testDb.DbContext.SaveChangesAsync();

        for (int i=0; i<50; i++)
        {
            var message = new Message(Guid.Empty, matches[i].Id, matches[i].UserId2, "test", false, DateTime.UtcNow - TimeSpan.FromMinutes(random.Next(1, 1000001)));
            _testDb.DbContext.Messages.Add(message);
        }
        await _testDb.DbContext.SaveChangesAsync();

        for (int i=50; i<75; i++)
        {
            var message = new Message(Guid.Empty, matches[i].Id, matches[i].UserId2, "test", false, DateTime.UtcNow - TimeSpan.FromMinutes(random.Next(1, 1000001)));
            _testDb.DbContext.Messages.Add(message);
        }
        await _testDb.DbContext.SaveChangesAsync();

        for (int i=75; i<100; i++)
        {
            var message = new Message(Guid.Empty, matches[i].Id, matches[i].UserId2, "test", true, DateTime.UtcNow - TimeSpan.FromMinutes(random.Next(1, 1000001)));
            _testDb.DbContext.Messages.Add(message);
        }
        await _testDb.DbContext.SaveChangesAsync();

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");
        var response = await Client.GetFromJsonAsync<List<MatchDto>>("users/me/updates");
        Assert.NotNull(response);
        Assert.Equal(100, response.Count);
    }

    [Fact]
    public async Task get_updates_returns_list_of_matches_dto_since_last_activity_time_parameter()
    {
        var time = DateTime.UtcNow;
        var user = await CreateUserAsync("test@test.com");
        var user2 = await CreateUserAsync("test2@test.com");
        var user3 = await CreateUserAsync("test3@test.com");
        var user4 = await CreateUserAsync("test4@test.com");
        var user5 = await CreateUserAsync("test5@test.com");
        var user6 = await CreateUserAsync("test6@test.com");

        _testDb.DbContext.Matches.Add(new Match(Guid.Empty, user.Id, user2.Id, false, false, null, time - TimeSpan.FromSeconds(1)));
        _testDb.DbContext.Matches.Add(new Match(Guid.Empty, user.Id, user3.Id, false, false, null, time - TimeSpan.FromSeconds(1)));
        _testDb.DbContext.Matches.Add(new Match(Guid.Empty, user.Id, user4.Id, false, false, null, time - TimeSpan.FromSeconds(1)));
        _testDb.DbContext.Matches.Add(new Match(Guid.Parse("00000000-0000-0000-0000-000000000011"), user.Id, user5.Id, false, false, null, time - TimeSpan.FromHours(2)));
        _testDb.DbContext.Matches.Add(new Match(Guid.Parse("00000000-0000-0000-0000-000000000012"), user.Id, user6.Id, false, false, null, time - TimeSpan.FromHours(2)));
        await _testDb.DbContext.SaveChangesAsync();

        _testDb.DbContext.Messages.Add(new Message(Guid.Empty, Guid.Parse("00000000-0000-0000-0000-000000000011"), user5.Id, "test", false, time - TimeSpan.FromSeconds(1)));
        _testDb.DbContext.Messages.Add(new Message(Guid.Empty, Guid.Parse("00000000-0000-0000-0000-000000000012"), user6.Id, "test", false, time - TimeSpan.FromSeconds(1)));
        await _testDb.DbContext.SaveChangesAsync();

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");

        var lastActivityTime = time - TimeSpan.FromHours(1);
        var response = await Client.GetFromJsonAsync<List<MatchDto>>($"users/me/updates?lastActivityTime={lastActivityTime}");
        Assert.NotNull(response);
        Assert.Equal(5, response.Count);
    }

    [Fact]
    public async Task patch_users_with_no_changes_returns_204_no_content()
    {
        var email = "test@test.com";
        var user = await CreateUserAsync(email);
        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");
        var command = new ChangeUser(user.Id);
        var payload = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");
        var response = await Client.PatchAsync($"users/{user.Id}", payload);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task patch_users_with_changes_returns_204_no_content()
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

    [Fact]
    public async Task given_user_not_exists_patch_users_404_not_found()
    {
        var email = "test@test.com";
        var user = await CreateUserAsync(email);
        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");
        var command = new ChangeUser(user.Id, "2001-01-01");
        var content = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");
        var response = await Client.PatchAsync($"users/{Guid.NewGuid()}", content);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<User> CreateUserAsync(string email, string phone = null)
    {
        var userId = Guid.NewGuid();
        var settings = new UserSettings(userId, Sex.MaleAndFemale, 18, 100, 100, 45.5, 45.5);
        Random random = new Random();
        if (phone == null) phone = random.Next(100000000, 999999999).ToString();
        var user = new User(userId, phone, email, "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);
        await _testDb.DbContext.Users.AddAsync(user);
        await _testDb.DbContext.SaveChangesAsync();
        return user;
    }

    private async Task DeleteUserAsync(User user)
    {
        _testDb.DbContext.Users.Remove(user);
        await _testDb.DbContext.DeletedEntities.AddAsync(new DeletedEntityDto() { Id = user.Id });
        await _testDb.DbContext.SaveChangesAsync();
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