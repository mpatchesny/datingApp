using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Queries;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Infrastructure.DAL.Handlers;
using Moq;
using Xunit;

namespace datingApp.Tests.Unit.QueryHandlers;

public class GetMessagesHandlerTests
{
    [Fact]
    public async Task query_messages_by_existing_match_id_should_return_nonempty_collection_of_messages_dto()
    {
        var query = new GetMessages();
        query.MatchId = 1;
        var messages = await _handler.HandleAsync(query);
        Assert.Equal(1, messages.Count());
        Assert.IsType<MessageDto>(messages.First());
    }

    [Fact]
    public async Task query_messages_by_nonexisting_match_id_should_return_empty_collection()
    {
        var query = new GetMessages();
        query.MatchId = 0;
        var messages = await _handler.HandleAsync(query);
        Assert.Equal(0, messages.Count());
    }

    // Arrange
    private readonly GetMessagesHandler _handler;
    public GetMessagesHandlerTests()
    {
        var settings = new UserSettings(1, Sex.Female, 18, 21, 20, 45.5, 45.5);
        var user = new User(1, "111111111", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);
        var match = new Core.Entities.Match(1, 1, 1, null, DateTime.UtcNow);
        var message = new Message(1, 1, 1, 1, "hello", false, DateTime.UtcNow);

        var mockMessageRepository = new Mock<IMessageRepository>();
        mockMessageRepository
            .Setup(x => x.GetByMatchIdAsync(1))
            .ReturnsAsync(new List<Core.Entities.Message> { message } );
        _handler = new GetMessagesHandler(mockMessageRepository.Object);
    }
}