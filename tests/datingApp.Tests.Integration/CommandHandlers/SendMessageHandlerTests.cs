using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Commands;
using datingApp.Application.Commands.Handlers;
using datingApp.Application.Exceptions;
using datingApp.Application.Security;
using datingApp.Core.Entities;
using datingApp.Infrastructure.DAL.Repositories;
using Microsoft.AspNetCore.Authorization;
using Moq;
using Xunit;
using Match = datingApp.Core.Entities.Match;

namespace datingApp.Tests.Integration.CommandHandlers;


public class SendMessageHandlerTests : IDisposable
{
    [Fact]
    public async Task send_message_within_existsing_match_should_succeed()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);

        var command = new SendMessage(Guid.NewGuid(), match.Id, user1.Id, "hello");
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
    }

    [Fact]
    public async Task given_authorization_fail_send_message_within_existsing_match_should_throw_UnauthorizedException()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Failed()));
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);

        var command = new SendMessage(Guid.NewGuid(), match.Id, user1.Id, "hello");
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<UnauthorizedException>(exception);
    }

    [Fact]
    public async Task send_message_within_nonexistsing_match_should_throw_match_not_exsits_exception()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);

        var command = new SendMessage(Guid.NewGuid(), Guid.NewGuid(), user1.Id, "hello");
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<MatchNotExistsException>(exception);
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly SendMessageHandler _handler;
    private readonly Mock<IDatingAppAuthorizationService> _authService;
    public SendMessageHandlerTests()
    {
        _testDb = new TestDatabase();
        _authService = new Mock<IDatingAppAuthorizationService>();
        var messageRepository = new DbMessageRepository(_testDb.DbContext);
        var matchRepository = new DbMatchRepository(_testDb.DbContext);
        _handler = new SendMessageHandler(messageRepository, matchRepository, _authService.Object);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}