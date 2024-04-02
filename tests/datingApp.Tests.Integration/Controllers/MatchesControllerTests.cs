using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Core.Entities;
using Newtonsoft.Json;
using Xunit;

namespace datingApp.Tests.Integration.Controllers;

public class MatchesControllerTests : ControllerTestBase, IDisposable
{
    [Fact]
    public async void get_match_returns_200_ok_and_match_dto()
    {
        var user1 = await CreateUserAsync("test1@test.com");
        var user2 = await CreateUserAsync("test2@test.com");
        var match = await CreateMatchAsync(user1, user2);

        var token = Authorize(user1.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");

        var response = await Client.GetFromJsonAsync<MatchDto>($"matches/{match.Id}");
        Assert.Equal(match.Id, response.Id);
    }
    
    [Fact]
    public async void get_matches_returns_200_ok_and_list_of_match_dto_in_paginated_data_dto()
    {
        var user1 = await CreateUserAsync("test1@test.com");
        var user2 = await CreateUserAsync("test2@test.com");
        var match = await CreateMatchAsync(user1, user2);

        var token = Authorize(user1.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");

        var response = await Client.GetFromJsonAsync<PaginatedDataDto>($"matches");
        Assert.Single(response.Data);
        var matchDto = JsonConvert.DeserializeObject<MatchDto>(response.Data[0].ToString());
        Assert.Equal(match.Id, matchDto.Id);
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

    private async Task<Match> CreateMatchAsync(User user1, User user2)
    {
        var match = new Match(Guid.NewGuid(), user1.Id, user2.Id, false, false, null, DateTime.UtcNow);
        await _testDb.DbContext.Matches.AddAsync(match);
        await _testDb.DbContext.SaveChangesAsync();
        return match;
    }

    private async Task<Message> CreateMessageAsync(Match match)
    {
        var message = new Message(Guid.NewGuid(), match.Id, match.UserId1, "test", false, DateTime.UtcNow);
        await _testDb.DbContext.Messages.AddAsync(message);
        await _testDb.DbContext.SaveChangesAsync();
        return message;
    }

    private readonly TestDatabase _testDb;
    public MatchesControllerTests(OptionsProvider optionsProvider) : base(optionsProvider)
    {
        _testDb = new TestDatabase();
    }

    public void Dispose()
    {
        _testDb?.Dispose();
    }
}