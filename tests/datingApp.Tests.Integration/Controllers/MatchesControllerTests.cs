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

[Collection("Controller tests")]
public class MatchesControllerTests : ControllerTestBase, IDisposable
{
    [Fact]
    public async void given_match_exists_get_match_returns_200_ok_and_match_dto()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);

        var token = Authorize(user1.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var response = await Client.GetFromJsonAsync<MatchDto>($"matches/{match.Id.Value}");
        Assert.True(match.Id.Equals(response.Id));
    }

    [Fact]
    public async void given_match_not_exists_get_match_returns_404_not_found_with_match_not_exist_reason()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);

        var token = Authorize(user1.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var notExistingMatchId = Guid.NewGuid();
        var response = await Client.GetAsync($"matches/{notExistingMatchId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<Error>();
        Assert.Equal($"Match with id {notExistingMatchId} does not exist.", error.Reason);
    }
    
    [Fact]
    public async void get_matches_returns_200_ok_and_list_of_match_dto_in_paginated_data_dto()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);

        var token = Authorize(user1.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var response = await Client.GetFromJsonAsync<PaginatedDataDto>($"matches");
        Assert.Single(response.Data);
        var matchDto = JsonConvert.DeserializeObject<MatchDto>(response.Data[0].ToString());
        Assert.Equal(match.Id, matchDto.Id);
    }

    [Fact]
    public async void get_matches_respects_provided_page_size()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        for (int i=0; i<20; i++)
        {
            var tempUser = await IntegrationTestHelper.CreateUserAsync(_testDb);
            _ = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, tempUser.Id);
        }

        var token = Authorize(user1.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var response = await Client.GetFromJsonAsync<PaginatedDataDto>($"matches?pageSize={5}");
        Assert.Equal(5, response.Data.Count);
    }

    [Fact]
    public async void get_matches_respects_provided_page()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var allMatchesGuid = new List<Guid>();
        for (int i=0; i<45; i++)
        {
            var tempUser = await IntegrationTestHelper.CreateUserAsync(_testDb);
            var tempMatch = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, tempUser.Id);
            allMatchesGuid.Add(tempMatch.Id);
        }

        var token = Authorize(user1.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var allMatchesDtoIds = new List<Guid>();
        for (int i=1; i<=3; i++)
        {
            var response = await Client.GetFromJsonAsync<PaginatedDataDto>($"matches?page={i}");
            foreach (var item in response.Data)
            {
                var matchDto = JsonConvert.DeserializeObject<MatchDto>(item.ToString());
                allMatchesDtoIds.Add(matchDto.Id);
            }
        }

        foreach (var matchId in allMatchesGuid)
        {
            Assert.Contains(matchId, allMatchesDtoIds);
        }
    }

    [Fact]
    public async void get_message_returns_200_ok_and_message_dto()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);
        var message = await IntegrationTestHelper.CreateMessageAsync(_testDb, match.Id, user2.Id);

        var token = Authorize(user1.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var response = await Client.GetFromJsonAsync<MessageDto>($"matches/{match.Id.Value}/messages/{message.Id.Value}");
        Assert.True(message.Id.Equals(response.Id));
    }

    [Fact]
    public async void given_message_not_exists_get_message_returns_404_not_found_with_reason_message_does_not_exist()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);

        var token = Authorize(user1.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var notExistsingMessageId = Guid.NewGuid();
        var response = await Client.GetAsync($"matches/{match.Id.Value}/messages/{notExistsingMessageId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<Error>();
        Assert.Equal($"Message with id {notExistsingMessageId} does not exist.", error.Reason);
    }

    [Fact]
    public async void get_messages_returns_200_ok_and_list_of_message_dto_in_paginated_data_dto()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);
        var message = await IntegrationTestHelper.CreateMessageAsync(_testDb, match.Id, user2.Id);

        var token = Authorize(user1.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var response = await Client.GetFromJsonAsync<PaginatedDataDto>($"matches/{match.Id.Value}/messages");
        Assert.Single(response.Data);
        var messageDto = JsonConvert.DeserializeObject<MessageDto>(response.Data[0].ToString());
        Assert.Equal(message.Id, messageDto.Id);
    }

    [Fact]
    public async void get_messages_respects_provided_page_size()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);
        for (int i=0; i<15; i++)
        {
            _ = await IntegrationTestHelper.CreateMessageAsync(_testDb, match.Id, user2.Id);
        }

        var token = Authorize(user1.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var response = await Client.GetFromJsonAsync<PaginatedDataDto>($"matches/{match.Id.Value}/messages?pageSize=5");
        Assert.Equal(5, response.Data.Count);
    }

    [Fact]
    public async void get_messages_respects_provided_page()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);
        var allMessagesGuid = new List<Guid>();
        for (int i=0; i<45; i++)
        {
            var message = await IntegrationTestHelper.CreateMessageAsync(_testDb, match.Id, user2.Id);
            allMessagesGuid.Add(message.Id);
        }

        var token = Authorize(user1.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var allMessagesDtoIds = new List<Guid>();
        for (int i=1; i<=3; i++)
        {
            var response = await Client.GetFromJsonAsync<PaginatedDataDto>($"matches/{match.Id.Value}/messages?page={i}");
            foreach (var item in response.Data)
            {
                var messageDto = JsonConvert.DeserializeObject<MessageDto>(item.ToString());
                allMessagesDtoIds.Add(messageDto.Id);
            }
        }

        foreach (var messageId in allMessagesDtoIds)
        {
            Assert.Contains(messageId, allMessagesGuid);
        }
    }

    [Fact]
    public async void given_match_not_exists_get_messages_returns_404_not_found_with_reason_match_not_exists()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);

        var token = Authorize(user1.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var notExistingMatchId = Guid.NewGuid();
        var response = await Client.GetAsync($"matches/{notExistingMatchId}/messages");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<Error>();
        Assert.Equal($"Match with id {notExistingMatchId} does not exist.", error.Reason);
    }

    [Fact]
    public async void send_message_returns_204_created_and_message_dto()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);

        var token = Authorize(user1.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var command = new SendMessage(Guid.Empty, match.Id, user1.Id, "test");

        var response = await Client.PostAsJsonAsync($"matches/{match.Id.Value}/messages", command);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var messageDto = await response.Content.ReadFromJsonAsync<MessageDto>();
        Assert.Equal(user1.Id.Value, messageDto.SendFromId);
        Assert.Equal("test", messageDto.Text);
    }

    [Fact]
    public async void given_empty_send_from_user_id_send_message_returns_204_created_and_message_dto()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);

        var token = Authorize(user1.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var command = new SendMessage(Guid.Empty, match.Id, Guid.Empty, "test");

        var response = await Client.PostAsJsonAsync($"matches/{match.Id.Value}/messages", command);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var messageDto = await response.Content.ReadFromJsonAsync<MessageDto>();
        Assert.Equal(user1.Id.Value, messageDto.SendFromId);
        Assert.Equal("test", messageDto.Text);
    }

    [Fact]
    public async void given_match_not_exists_send_message_returns_404_not_found_with_reason_match_not_exists()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var notExistingMatchId = Guid.NewGuid();

        var token = Authorize(user1.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var command = new SendMessage(Guid.Empty, notExistingMatchId, user1.Id, "test");

        var response = await Client.PostAsJsonAsync($"matches/{notExistingMatchId}/messages", command);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<Error>();
        Assert.Equal($"Match with id {notExistingMatchId} does not exist.", error.Reason);
    }

    [Fact ]
    public async void delete_existing_match_returns_204_no_content()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);

        var token = Authorize(user1.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var response = await Client.DeleteAsync($"matches/{match.Id.Value}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async void given_match_not_exists_delete_returns_404_not_found()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);

        var token = Authorize(user1.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var notExistingMatchId = Guid.NewGuid();
        var response = await Client.DeleteAsync($"matches/{notExistingMatchId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<Error>();
        Assert.Equal($"Match with id {notExistingMatchId} does not exist.", error.Reason);
    }

    [Fact]
    public async void given_match_was_already_deleted_delete_returns_410_gone()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);

        var token = Authorize(user1.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var deletedMatch = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);
        await IntegrationTestHelper.DeleteMatchAsync(_testDb, deletedMatch);

        var response = await Client.DeleteAsync($"matches/{deletedMatch.Id.Value}");
        Assert.Equal(HttpStatusCode.Gone, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<Error>();
        Assert.Equal($"Match {deletedMatch.Id.Value} is deleted permanently.", error.Reason);
    }

    private readonly TestDatabase _testDb;
    public MatchesControllerTests(OptionsProvider optionsProvider) : base(optionsProvider)
    {
        _testDb = new TestDatabase(false);
    }

    public void Dispose()
    {
        _testDb?.Dispose();
    }
}