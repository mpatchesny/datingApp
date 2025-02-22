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
using datingApp.Infrastructure;
using datingApp.Infrastructure.Exceptions;
using FluentStorage.Utils.Extensions;
using MailKit;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Prng;
using Xunit;

namespace datingApp.Tests.Integration.Controllers;

[Collection("Controller tests")]
public class UsersControllerTests : ControllerTestBase, IDisposable
{
    #region SignUp
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
        await IntegrationTestHelper.CreateUserAsync(_dbContext, email: email);
        _dbContext.ChangeTracker.Clear();

        var command = new SignUp(Guid.Empty, "123456789", email, "Janusz", "2000-01-01", 1, 1);
        var response = await Client.PostAsJsonAsync("users", command);
        var error = await response.Content.ReadFromJsonAsync<Error>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal($"Email {email} is already in use.", error.Reason);
    }

    [Fact]
    public async Task given_phone_already_exists_sign_up_post_request_returns_400_bad_request()
    {
        var phone = "123456789";
        await IntegrationTestHelper.CreateUserAsync(_dbContext, phone : phone);
        _dbContext.ChangeTracker.Clear();

        var command = new SignUp(Guid.Empty, phone, "test@test.com", "Janusz", "2000-01-01", 1, 1);
        var response = await Client.PostAsJsonAsync("users", command);
        var error = await response.Content.ReadFromJsonAsync<Error>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal($"Phone {phone} is already in use.", error.Reason);
    }
    #endregion

    #region Auth
    [Fact]
    public async Task given_email_exists_auth_post_request_returns_200_ok()
    {
        var email = "test@test.com";
        await IntegrationTestHelper.CreateUserAsync(_dbContext, email: email);
        _dbContext.ChangeTracker.Clear();

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
    #endregion

    #region SignIn
    [Fact]
    public async Task given_valid_access_code_sign_in_post_request_returns_200_ok_and_token()
    {
        var email = "test@test.com";
        await IntegrationTestHelper.CreateUserAsync(_dbContext, email: email);
        var accessCode = "12345";
        await CreateAccessCode(_dbContext, email, accessCode);
        _dbContext.ChangeTracker.Clear();

        var command = new SignInByEmail(email, accessCode);
        var response = await Client.PostAsJsonAsync("users/sign-in", command);
        var token = await response.Content.ReadFromJsonAsync<JwtDto>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(token);
        Assert.False(string.IsNullOrWhiteSpace(token.AccessToken.Token));
    }

    [Fact]
    public async Task given_invalid_access_code_sign_in_post_request_returns_400_bad_request()
    {
        var email = "test@test.com";
        await IntegrationTestHelper.CreateUserAsync(_dbContext, email: email);
        var accessCode = "12345";
        await CreateAccessCode(_dbContext, email, accessCode);
        _dbContext.ChangeTracker.Clear();

        var invalidAccessCode = "67890";
        var command = new SignInByEmail(email, invalidAccessCode);
        var response = await Client.PostAsJsonAsync("users/sign-in", command);
        var error = await response.Content.ReadFromJsonAsync<Error>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Provided credentials are invalid.", error.Reason);
    }

    [Fact]
    public async Task given_expired_access_code_sign_in_post_request_returns_400_bad_request()
    {
        // FIXME: test fails when run with other tests, but succeed when run alone
        var email = "test@test.com";
        await IntegrationTestHelper.CreateUserAsync(_dbContext, email: email);
        var expiredAccessCode = "12345";
        await CreateAccessCode(_dbContext, email, expiredAccessCode, TimeSpan.FromMilliseconds(1));
        _dbContext.ChangeTracker.Clear();

        var command = new SignInByEmail(email, expiredAccessCode);
        var response = await Client.PostAsJsonAsync("users/sign-in", command);
        var error = await response.Content.ReadFromJsonAsync<Error>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Provided credentials are invalid.", error.Reason);
    }
    #endregion

    #region AuthRefresh
    [Fact]
    public async Task given_invalid_refresh_token_auth_refresh_returns_401_unauthorized()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();
        var badToken = Authorize(user.Id).AccessToken.Token;

        var command = new RefreshJWT(badToken);
        var response = await Client.PostAsJsonAsync($"users/auth/refresh", command);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task given_valid_refresh_token_auth_refresh_returns_200_with_new_access_and_refresh_tokens()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();
        var tokens = Authorize(user.Id);
        var accessToken = tokens.AccessToken.Token;
        var refreshToken = tokens.RefreshToken.Token;

        // workaround: sleep 1000 milliseconds so that newly generated token
        // is not the same as the old token
        Thread.Sleep(1000);

        var command = new RefreshJWT(refreshToken);
        var response = await Client.PostAsJsonAsync($"users/auth/refresh", command);
        var responseJson = await response.Content.ReadFromJsonAsync<JwtDto>();

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseJson.AccessToken);
        Assert.NotNull(responseJson.RefreshToken);
        Assert.NotEqual(responseJson.AccessToken.Token, accessToken);
        Assert.NotEqual(responseJson.RefreshToken.Token, refreshToken);
    }

    [Fact]
    public async Task given_valid_refresh_token_used_more_than_once_auth_refresh_returns_401_unauthorized()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();
        var tokens = Authorize(user.Id);
        var refreshToken = tokens.RefreshToken.Token;

        var command = new RefreshJWT(refreshToken);
        var response = await Client.PostAsJsonAsync($"users/auth/refresh", command);
        var secondResponse = await Client.PostAsJsonAsync($"users/auth/refresh", command);

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(secondResponse);
        Assert.Equal(HttpStatusCode.Unauthorized, secondResponse.StatusCode);
    }

    [Fact]
    public async Task given_expired_refresh_token_auth_refresh_returns_401_unauthorized()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();
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
    #endregion

    #region GetUsers
    [Fact]
    public async Task given_two_users_have_match_get_users_returns_200_ok_and_public_user()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _ = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id);
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user1.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var response = await Client.GetFromJsonAsync<PublicUserDto>($"users/{user2.Id.Value}");

        Assert.NotNull(response);
        Assert.Equal(user2.Id.Value, response.Id);
    }

    [Fact]
    public async Task given_two_users_dont_have_match_get_users_returns_403_forbidden_with_proper_error_reason()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user1.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var response =  await Client.GetAsync($"users/{user2.Id.Value}");
        var error = await response.Content.ReadFromJsonAsync<Error>();

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("You don't have permission to perform this action.", error.Reason);
    }

    [Fact]
    public async Task given_user_get_himself_get_users_returns_403_forbidden_with_proper_error_reason()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user1.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var response = await Client.GetAsync($"users/{user1.Id.Value}");
        var error = await response.Content.ReadFromJsonAsync<Error>();

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("You don't have permission to perform this action.", error.Reason);
    }

    [Fact]
    public async Task given_requested_user_not_exists_get_users_returns_404_with_proper_error_reason()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var notExistingUserId = Guid.NewGuid();
        var response = await Client.GetAsync($"users/{notExistingUserId}");
        var error = await response.Content.ReadFromJsonAsync<Error>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal($"User with id {notExistingUserId} does not exist.", error.Reason);
    }

    [Fact]
    public async Task given_missing_token_get_users_returns_401_unauthorized()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var response = await Client.GetAsync($"users/{user.Id.Value}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task given_invalid_token_get_users_returns_401_unauthorized()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();
        var token = Authorize(user.Id);
        var badToken = token.AccessToken.Token + "x";

        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {badToken}");
        var response = await Client.GetAsync($"users/{user.Id.Value}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    #endregion

    #region GetUsersMe
    [Fact]
    public async Task given_valid_refresh_token_used_to_authorize_get_me_returns_401_unauthorized()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();
        var tokens = Authorize(user.Id);
        var refreshToken = tokens.RefreshToken.Token;

        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {refreshToken}");
        var response = await Client.GetAsync($"users/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task get_users_me_returns_200_ok_and_private_user()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var response = await Client.GetFromJsonAsync<PrivateUserDto>($"users/me");

        Assert.NotNull(response);
        Assert.Equal(user.Id.Value, response.Id);
    }
    #endregion

    #region DeleteUsers
    [Fact]
    public async Task given_user_exists_delete_users_returns_204_no_content()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var response = await Client.DeleteAsync($"users/{user.Id.Value}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Empty(await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task given_user_not_exists_delete_users_returns_404_not_found_and_proper_error_reason()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");
        
        var notExistingUserId = Guid.NewGuid();
        var response = await Client.DeleteAsync($"users/{notExistingUserId}");
        var error = await response.Content.ReadFromJsonAsync<Error>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal($"User with id {notExistingUserId} does not exist.", error.Reason);
    }

    [Fact]
    public async Task given_user_was_already_deleted_delete_users_returns_410_gone()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        await IntegrationTestHelper.DeleteUserAsync(_dbContext, user);
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");
        
        var response = await Client.DeleteAsync($"users/{user.Id.Value}");
        var error = await response.Content.ReadFromJsonAsync<Error>();

        Assert.Equal(HttpStatusCode.Gone, response.StatusCode);
        Assert.Equal($"User {user.Id.Value} is deleted permanently.", error.Reason);
    }

    [Fact]
    public async Task given_user_deletes_other_user_delete_users_returns_403_forbidden_with_proper_error_reason()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user1.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");
        
        var response = await Client.DeleteAsync($"users/{user2.Id.Value}");
        var error = await response.Content.ReadFromJsonAsync<Error>();

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("You don't have permission to perform this action.", error.Reason);
    }
    #endregion

    #region GetMeRecommendations
    [Fact]
    public async Task get_recommendations_returns_200_and_list_of_public_user_dto()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");
        var response = await Client.GetFromJsonAsync<List<PublicUserDto>>($"users/me/recommendations");

        Assert.NotNull(response);
    }

    [Fact]
    public async Task get_recommendations_returns_max_10_private_user_dtos()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        for (int i=0; i<20; i++)
        {
            await IntegrationTestHelper.CreateUserAsync(_dbContext, email: $"test{i}@test.com");
        }
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");
        var response = await Client.GetFromJsonAsync<List<PublicUserDto>>($"users/me/recommendations");

        Assert.NotNull(response);
        Assert.Equal(10, response.Count);
    }
    #endregion

    #region GetMeUpdates
    [Fact]
    public async Task get_updates_returns_200_and_list_of_matches_dto()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");
        var response = await Client.GetFromJsonAsync<PaginatedDataDto<MatchDto>>("users/me/updates");

        Assert.NotNull(response);
    }

    [Fact]
    public async Task get_updates_without_lastActivityTime_specified_returns_not_displayed_messages_and_matches_as_matches_dto()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var users = new List<User>();
        for (int i=0; i<100; i++)
        {
            var tempUser = await IntegrationTestHelper.CreateUserAsync(_dbContext);
            users.Add(tempUser);
        }

        var random = new Random();
        var matches = new List<Match>();
        for (int i=0; i<100; i++)
        {
            var message = IntegrationTestHelper.CreateMessage(users[i].Id, createdAt: DateTime.UtcNow - TimeSpan.FromMinutes(random.Next(1, 1000001)));
            var messages = new List<Message>() { message };
            var match = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user.Id, users[i].Id, messages: messages, createdAt : DateTime.UtcNow - TimeSpan.FromMinutes(random.Next(1, 1000001)));
            matches.Add(match);
        }
        _dbContext.ChangeTracker.Clear();

        var defaultPageSize = 15;

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");
        var response = await Client.GetFromJsonAsync<PaginatedDataDto<MatchDto>>("users/me/updates");

        Assert.NotNull(response);
        Assert.Equal(defaultPageSize, response.Data.Count);
    }

    [Fact]
    public async Task get_updates_respects_provided_page()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var users = new List<User>();
        for (int i=0; i<100; i++)
        {
            var tempUser = await IntegrationTestHelper.CreateUserAsync(_dbContext);
            users.Add(tempUser);
        }

        var random = new Random();
        var matches = new List<Match>();
        for (int i=0; i<100; i++)
        {
            var message = IntegrationTestHelper.CreateMessage(users[i].Id, createdAt: DateTime.UtcNow - TimeSpan.FromMinutes(random.Next(1, 1000001)));
            var messages = new List<Message>() { message };
            var match = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user.Id, users[i].Id, messages: messages, createdAt : DateTime.UtcNow - TimeSpan.FromMinutes(random.Next(1, 1000001)));
            matches.Add(match);
        }
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var allMatchesDtoIds = new List<Guid>();
        var page = 1;
        while (true)
        {
            var response = await Client.GetFromJsonAsync<PaginatedDataDto<MatchDto>>($"users/me/updates?page={page}");

            if (response.Data.Count == 0) break;

            foreach (var matchDto in response.Data)
            {
                allMatchesDtoIds.Add(matchDto.Id);
            }

            page++;
        }

        Assert.Equal(matches.Select(m => m.Id.Value).OrderBy(id => id), allMatchesDtoIds.OrderBy(id => id));
    }

    [Fact]
    public async Task get_updates_respects_provided_page_size()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var users = new List<User>();
        for (int i=0; i<100; i++)
        {
            var tempUser = await IntegrationTestHelper.CreateUserAsync(_dbContext);
            users.Add(tempUser);
        }

        var random = new Random();
        var matches = new List<Match>();
        for (int i=0; i<100; i++)
        {
            var message = IntegrationTestHelper.CreateMessage(users[i].Id, createdAt: DateTime.UtcNow - TimeSpan.FromMinutes(random.Next(1, 1000001)));
            var messages = new List<Message>() { message };
            var match = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user.Id, users[i].Id, messages: messages, createdAt : DateTime.UtcNow - TimeSpan.FromMinutes(random.Next(1, 1000001)));
            matches.Add(match);
        }
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");
        var response = await Client.GetFromJsonAsync<PaginatedDataDto<MatchDto>>("users/me/updates?pageSize=5");

        Assert.Equal(5, response.Data.Count);
    }

    [Fact]
    public async Task get_updates_returns_list_of_matches_dto_since_last_activity_time_parameter()
    {
        var time = DateTime.UtcNow;
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user3 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user4 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user5 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user6 = await IntegrationTestHelper.CreateUserAsync(_dbContext);

        _ = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id, createdAt: time - TimeSpan.FromSeconds(1));
        _ = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user3.Id, createdAt: time - TimeSpan.FromSeconds(1));
        _ = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user4.Id, createdAt: time - TimeSpan.FromSeconds(1));
        var messages1 = new List<Message>() { IntegrationTestHelper.CreateMessage(user5.Id, createdAt: time - TimeSpan.FromSeconds(1)) };
        var messages2 = new List<Message>() { IntegrationTestHelper.CreateMessage(user6.Id, createdAt: time - TimeSpan.FromSeconds(1)) };
        _ = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user5.Id, messages: messages1, createdAt: time - TimeSpan.FromHours(2));
        _ = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user6.Id, messages: messages2, createdAt: time - TimeSpan.FromHours(2));
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user1.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var lastActivityTime = (time - TimeSpan.FromHours(1)).ToIso8601DateString();
        var response = await Client.GetFromJsonAsync<PaginatedDataDto<MatchDto>>($"users/me/updates?lastActivityTime={lastActivityTime}");

        Assert.NotNull(response);
        Assert.Equal(5, response.Data.Count);
    }
    #endregion

    #region PatchUsers
    [Fact]
    public async Task patch_users_with_no_changes_returns_204_no_content()
    {
        var email = "test@test.com";
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext, email: email);
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var command = new ChangeUser(user.Id);
        var payload = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");
        var response = await Client.PatchAsync($"users/{user.Id.Value}", payload);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Empty(await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task patch_users_with_changes_returns_204_no_content()
    {
        var email = "test@test.com";
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext, email: email);
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var command = new ChangeUser(user.Id, "2001-01-01");
        var content = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");
        var response = await Client.PatchAsync($"users/{user.Id.Value}", content);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Empty(await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task given_user_not_exists_patch_users_returns_404_not_found()
    {
        var email = "test@test.com";
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext, email: email);
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var command = new ChangeUser(user.Id, "2001-01-01");
        var content = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");
        var notExistingUserId = Guid.NewGuid();
        var response = await Client.PatchAsync($"users/{notExistingUserId}", content);
        var error = await response.Content.ReadFromJsonAsync<Error>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal($"User with id {notExistingUserId} does not exist.", error.Reason);
    }

    [Fact]
    public async Task given_user_patches_other_user_patch_users_returns_403_forbidden_with_proper_error_reason()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user1.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var command = new ChangeUser(user2.Id, "2001-01-01");
        var content = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");
        var response = await Client.PatchAsync($"users/{user2.Id}", content);
        var error = await response.Content.ReadFromJsonAsync<Error>();

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("You don't have permission to perform this action.", error.Reason);
    }
    #endregion

    private static async Task<AccessCodeDto> CreateAccessCode(DatingAppDbContext dbContext, string email, string accessCode = "123456", TimeSpan? expirationTime = null)
    {
        var code = new AccessCodeDto()
            {
                AccessCode = accessCode,
                EmailOrPhone = email,
                ExpirationTime = DateTime.UtcNow + (expirationTime ?? TimeSpan.FromMinutes(15)),
                Expiry = expirationTime ?? TimeSpan.FromMinutes(15)
            };
        await dbContext.AccessCodes.AddAsync(code);
        await dbContext.SaveChangesAsync();
        return code;
    }

    private readonly TestDatabase _testDb;
    private readonly DatingAppDbContext _dbContext;

    public UsersControllerTests(OptionsProvider optionsProvider) : base(optionsProvider)
    {
        _testDb = new TestDatabase(false);
        _dbContext = _testDb.DbContext;
    }

    public void Dispose()
    {
        _testDb?.Dispose();
    }
}