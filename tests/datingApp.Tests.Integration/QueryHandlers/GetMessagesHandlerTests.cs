using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Queries;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Infrastructure.DAL.Handlers;
using Xunit;

namespace datingApp.Tests.Integration.QueryHandlers;

[Collection("Integration tests")]
public class GetMessagesHandlerTests
{
    [Fact]
    public async Task query_messages_by_existing_match_id_should_return_nonempty_collection_of_messages_dto()
    {
        var query = new GetMessages();
        query.MatchId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var messages = await _handler.HandleAsync(query);
        Assert.NotEmpty(messages);
        Assert.IsType<MessageDto>(messages.First());
    }

    [Fact]
    public async Task query_messages_by_nonexisting_match_id_should_return_empty_collection()
    {
        var query = new GetMessages();
        query.MatchId = Guid.Parse("00000000-0000-0000-0000-000000000000");
        var messages = await _handler.HandleAsync(query);
        Assert.Empty(messages);
    }

    [Fact]
    public async Task returned_messages_count_is_lower_or_equal_to_page_size()
    {
        var query = new GetMessages();
        query.MatchId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        query.SetPageSize(5);
        var matches = await _handler.HandleAsync(query);
        Assert.InRange(matches.Count(), 0, query.PageSize);
    }

    [Fact]
    public async Task returns_proper_number_of_messages_when_page_above_1()
    {
        var query = new GetMessages();
        query.MatchId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        query.SetPageSize(5);
        query.SetPage(1);
        var matches = await _handler.HandleAsync(query);
        Assert.NotEmpty(matches);
        Assert.Equal(4, matches.Count());
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly GetMessagesHandler _handler;
    public GetMessagesHandlerTests()
    {
        var settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000001"), Sex.Female, 18, 21, 20, 45.5, 45.5);
        var user = new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "111111111", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);
        var match = new Core.Entities.Match(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), false, false, null, DateTime.UtcNow);
        
        var messages = new List<Message>{
            new Message(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "hello", false, DateTime.UtcNow),
            new Message(Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "hello", false, DateTime.UtcNow),
            new Message(Guid.Parse("00000000-0000-0000-0000-000000000003"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "hello", false, DateTime.UtcNow),
            new Message(Guid.Parse("00000000-0000-0000-0000-000000000004"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "hello", false, DateTime.UtcNow),
            new Message(Guid.Parse("00000000-0000-0000-0000-000000000005"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "hello", false, DateTime.UtcNow),
            new Message(Guid.Parse("00000000-0000-0000-0000-000000000006"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "hello", false, DateTime.UtcNow),
            new Message(Guid.Parse("00000000-0000-0000-0000-000000000007"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "hello", false, DateTime.UtcNow),
            new Message(Guid.Parse("00000000-0000-0000-0000-000000000008"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "hello", false, DateTime.UtcNow),
            new Message(Guid.Parse("00000000-0000-0000-0000-000000000009"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "hello", false, DateTime.UtcNow)
        };
        
        _testDb = new TestDatabase();
        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.SaveChanges();
        _testDb.DbContext.Matches.Add(match);
        _testDb.DbContext.SaveChanges();
        _testDb.DbContext.Messages.AddRange(messages);
        _testDb.DbContext.SaveChanges();
        _handler = new GetMessagesHandler(_testDb.DbContext);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}