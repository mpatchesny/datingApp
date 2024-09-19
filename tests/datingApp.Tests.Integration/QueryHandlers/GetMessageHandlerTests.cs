using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Queries;
using datingApp.Application.Security;
using datingApp.Core.Entities;
using datingApp.Infrastructure.DAL.Handlers;
using Microsoft.AspNetCore.Authorization;
using Moq;
using Xunit;
using Match = datingApp.Core.Entities.Match;

namespace datingApp.Tests.Integration.QueryHandlers;


public class GetMessageHandlerTests : IDisposable
{
    [Fact]
    public async Task query_messages_by_existing_message_id_should_return_nonempty_message_dto()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);
        var message = await IntegrationTestHelper.CreateMessageAsync(_testDb, match.Id, user2.Id);

        var query = new GetMessage() { MessageId = message.Id };
        var messageDto = await _handler.HandleAsync(query);
        Assert.NotNull(messageDto);
        Assert.IsType<MessageDto>(messageDto);
    }

    [Fact]
    public async Task given_authorization_fail_query_messages_by_existing_message_id_should_return_UnauthorizedException()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Failed()));
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);
        var message = await IntegrationTestHelper.CreateMessageAsync(_testDb, match.Id, user2.Id);

        var query = new GetMessage() { MessageId = message.Id };
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(query));
        Assert.NotNull(exception);
        Assert.IsType<MatchNotExistsException>(exception);
    }

    [Fact]
    public async Task query_messages_by_nonexisting_message_id_should_return_message_not_exists_exception()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);

        var query = new GetMessage() { MessageId = Guid.NewGuid() };
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(query));
        Assert.NotNull(exception);
        Assert.IsType<MessageNotExistsException>(exception);
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly GetMessageHandler _handler;
    private readonly Mock<IDatingAppAuthorizationService> _authService;
    public GetMessageHandlerTests()
    {
        _testDb = new TestDatabase();
        _authService = new Mock<IDatingAppAuthorizationService>();
        _handler = new GetMessageHandler(_testDb.DbContext, _authService.Object);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}