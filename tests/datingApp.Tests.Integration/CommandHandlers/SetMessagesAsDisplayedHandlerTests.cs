using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Commands;
using datingApp.Application.Commands.Handlers;
using datingApp.Application.Exceptions;
using datingApp.Core.Consts;
using datingApp.Core.Entities;
using datingApp.Core.ValueObjects;
using datingApp.Infrastructure;
using datingApp.Infrastructure.DAL.Repositories;
using MailKit;
using Xunit;

namespace datingApp.Tests.Integration.CommandHandlers;


public class SetMessagesAsDisplayedHandlerTests : IDisposable
{
    [Fact]
    public async Task given_message_exists_set_messages_as_displayed_should_succeed()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb.CreateNewDbContext());
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb.CreateNewDbContext());
        var messages = new List<Message>() { IntegrationTestHelper.CreateMessage(user1.Id) };
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb.CreateNewDbContext(), user1.Id, user2.Id, messages: messages);
        _dbContext.ChangeTracker.Clear();

        var command = new SetMessagesAsDisplayed(messages[0].Id, user2.Id);
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
    private readonly TestDatabase _testDb;
    private readonly DatingAppDbContext _dbContext;
    private readonly SetMessagesAsDisplayedHandler _handler;
    public SetMessagesAsDisplayedHandlerTests()
    {
        _testDb = new TestDatabase();
        _dbContext = _testDb.DbContext;
        var matchRepository = new DbMatchRepository(_dbContext);
        _handler = new SetMessagesAsDisplayedHandler(matchRepository);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}