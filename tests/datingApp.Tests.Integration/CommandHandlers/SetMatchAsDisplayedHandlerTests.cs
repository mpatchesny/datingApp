using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Commands;
using datingApp.Application.Commands.Handlers;
using datingApp.Application.Security;
using datingApp.Core.Entities;
using datingApp.Infrastructure;
using datingApp.Infrastructure.DAL.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using Match = datingApp.Core.Entities.Match;

namespace datingApp.Tests.Integration.CommandHandlers;

[Collection("Controller tests")]
public class SetMatchAsDisplayedHandlerTests : ControllerTestBase, IDisposable
{
    [Fact]
    public async Task given_match_exists_SetMatchAsDisplayed_sets_match_as_displayed_and_all_messages_send_by_other_user_as_displayedAsync()
    {
        var authService = new Mock<IDatingAppAuthorizationService>();
        authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy"))
            .Returns(Task.FromResult(AuthorizationResult.Success()));
        var matchRepository = new DbMatchRepository(_dbContext);

        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var messages = new List<Message>() {
            IntegrationTestHelper.CreateMessage(user1.Id),
            IntegrationTestHelper.CreateMessage(user1.Id),
            IntegrationTestHelper.CreateMessage(user1.Id),
            IntegrationTestHelper.CreateMessage(user2.Id),
            IntegrationTestHelper.CreateMessage(user2.Id)
        };
        var match = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id, messages: messages);
        _dbContext.ChangeTracker.Clear();

        var command = new SetMatchAsDisplayed(match.Id, user1.Id);
        var handler = new SetMatchAsDisplayedHandler(matchRepository, authService.Object);
        await handler.HandleAsync(command);

        var messagesAfterCount = await _dbContext.Matches
            .Where(match => match.Id.Equals(match.Id))
            .SelectMany(match => match.Messages
                .Where(msg => msg.SendFromId.Equals(user2.Id) 
                    && msg.IsDisplayed == true))
            .CountAsync();
        Assert.Equal(2, messagesAfterCount);

        messagesAfterCount = await _dbContext.Matches
            .Where(match => match.Id.Equals(match.Id))
            .SelectMany(match => match.Messages
                .Where(msg => msg.SendFromId.Equals(user1.Id) 
                    && msg.IsDisplayed == false))
            .CountAsync();
        Assert.Equal(3, messagesAfterCount);
    }

    private readonly TestDatabase _testDb;
    private readonly DatingAppDbContext _dbContext;

    public SetMatchAsDisplayedHandlerTests(OptionsProvider optionsProvider) : base(optionsProvider)
    {
        _testDb = new TestDatabase(false);
        _dbContext = _testDb.DbContext;
    }

    public void Dispose()
    {
        _testDb?.Dispose();
    }
}