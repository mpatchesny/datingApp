using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Commands;
using datingApp.Application.Commands.Handlers;
using datingApp.Application.Exceptions;
using datingApp.Core.Consts;
using datingApp.Core.Entities;
using datingApp.Infrastructure.DAL.Repositories;
using Xunit;

namespace datingApp.Tests.Integration.CommandHandlers;


public class SetMessagesAsDisplayedHandlerTests : IDisposable
{
    [Fact]
    public async Task given_message_exists_set_messages_as_displayed_should_succeed()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);
        var message = await IntegrationTestHelper.CreateMessageAsync(_testDb, match.Id, user1.Id);

        var command = new SetMessagesAsDisplayed(message.Id, user2.Id);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
    }

    [Fact]
    public async Task given_message_not_exists_set_messages_as_displayed_should_do_nothing()
    {
        var command = new SetMessagesAsDisplayed(Guid.NewGuid(), Guid.NewGuid());
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
    }

    // Arrange
    private readonly SetMessagesAsDisplayedHandler _handler;
    private readonly TestDatabase _testDb;
    public SetMessagesAsDisplayedHandlerTests()
    {
        _testDb = new TestDatabase();
        var messageRepository = new DbMessageRepository(_testDb.DbContext);
        _handler = new SetMessagesAsDisplayedHandler(messageRepository);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}