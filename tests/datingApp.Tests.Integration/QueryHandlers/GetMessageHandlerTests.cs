using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Queries;
using datingApp.Core.Entities;
using datingApp.Infrastructure.DAL.Handlers;
using Xunit;

namespace datingApp.Tests.Integration.QueryHandlers;

[Collection("Integration tests")]
public class GetMessageHandlerTests
{
    [Fact]
    public async Task query_messages_by_existing_message_id_should_return_nonempty_message_dto()
    {
        var query = new GetMessage();
        query.MessageId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var message = await _handler.HandleAsync(query);
        Assert.NotNull(message);
        Assert.IsType<MessageDto>(message);
    }

    [Fact]
    public async Task query_messages_by_nonexisting_message_id_should_return_null()
    {
        var query = new GetMessage();
        query.MessageId = Guid.Parse("00000000-0000-0000-0000-000000000000");
        var message = await _handler.HandleAsync(query);
        Assert.Null(message);
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly GetMessageHandler _handler;
    public GetMessageHandlerTests()
    {
        var settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000001"), Sex.Female, 18, 21, 20, 45.5, 45.5);
        var user = new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "111111111", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);
        var match = new Core.Entities.Match(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), false, false, null, DateTime.UtcNow);
        
        var messages = new List<Message>{
            new Message(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "hello", false, DateTime.UtcNow)
        };
        
        _testDb = new TestDatabase();
        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.SaveChanges();
        _testDb.DbContext.Matches.Add(match);
        _testDb.DbContext.SaveChanges();
        _testDb.DbContext.Messages.AddRange(messages);
        _testDb.DbContext.SaveChanges();
        _handler = new GetMessageHandler(_testDb.DbContext);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}